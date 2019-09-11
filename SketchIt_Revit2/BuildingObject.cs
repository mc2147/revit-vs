using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using static PW.RevitUtil;
using static PW.WallUtils;
using static PW.CurveUtils;

namespace PW
{
    // *** BUILDING OBJECT ***
    public class BuildingObject
    {
        public List<WallObject> walls;
        public List<LevelObject> levels;
        public List<Coordinate> xy_coords;
        public BuildingObject(
          List<WallObject> _walls,
          List<LevelObject> _levels,
          List<Coordinate> _xy_coords
          )
        {
            levels = _levels;
            walls = _walls;
            xy_coords = _xy_coords;
        }

        public void logDetails()
        {
            Console.WriteLine("Walls: ");
            int wallIndex = 0;
            foreach (var _wall in walls)
            {
                Console.WriteLine("   Wall index: " + wallIndex);
                Console.WriteLine("      Level: " + _wall.level);
                Console.WriteLine("      Coordinates: " + _wall.coordinates.Count);
                foreach (var _coord in _wall.coordinates)
                {
                    Console.WriteLine("         " + _coord.x + ", " + _coord.y);
                }
                wallIndex++;
            }

            Console.WriteLine("Levels: ");
            int levelIndex = 0;

            foreach (var _level in levels)
            {
                Console.WriteLine("   Level index: " + levelIndex);
                Console.WriteLine("      Elevation: " + _level.elevation);
                // 
                Console.WriteLine("      Category: " + _level.category);
                Console.WriteLine("      has_category: " + _level.has_category);
                // 
                Console.WriteLine("      has_color: " + _level.has_color);
                if (_level.has_color)
                {
                    Console.WriteLine("      Color: (" + _level.color.red + ", " + _level.color.green + ", " + _level.color.blue + ")");
                }
                levelIndex++;
            }
        }

        public void createRVTFile(Autodesk.Revit.DB.Document newDoc, string filePath, bool saveFile)
        {
            // *** LOOP THROUGH BUILDING LEVELS ***
            int levelIndex = 0;
            var level_IDs = new Dictionary<int, Autodesk.Revit.DB.ElementId>();
            // 
            var colored_level_IDs = new Dictionary<Autodesk.Revit.DB.ElementId, ColorObject>();
            // 
            //FamilyItemFactory factory = newDoc.FamilyCreate;
            // 
            var extrusionCurveArray = curveArrayFromCoords(xy_coords, 50, true);
            CurveArrArray extrusionCurveArrArray = new CurveArrArray();
            extrusionCurveArrArray.Append(extrusionCurveArray);

            SketchPlane sketch = FindElement(newDoc, typeof(Level), "Ref. Level", null) as SketchPlane;

            //factory.NewExtrusion(true,extrusionCurveArrArray,sketch,25);
            // 
            foreach (LevelObject _level in levels)
            {
                double _elevation = _level.elevation;
                Console.WriteLine("_elevation: " + _elevation);
                // *** CREATING FLOORS FOR EACH LEVEL
                CurveArray floor_curves = curveArrayFromCoords(xy_coords, _elevation);
                using (Transaction floorTrans = new Transaction(newDoc, "Create a floor"))
                {
                    floorTrans.Start();
                    newDoc.Create.NewFloor(floor_curves, false);
                    floorTrans.Commit();
                }
                // *** REQUIRES REVIT:
                using (Transaction levelTrans = new Transaction(newDoc, "Creating levels"))
                {
                    levelTrans.Start();
                    Level revitLevel = Level.Create(newDoc, _level.elevation);
                    if (revitLevel == null)
                    {
                        throw new Exception("Create a new level failed.");
                    }

                    if (_level.has_category)
                    {
                        string _category = _level.category;
                        Console.WriteLine("_category: " + _category);
                        revitLevel.Name = _level.category + " " + levelIndex;
                    }
                    // *** SAVE LEVEL COLOR
                    if (_level.has_color)
                    {
                        colored_level_IDs[revitLevel.Id] = _level.color;
                    }
                    // *** SAVE LEVEL IDS
                    level_IDs[levelIndex] = revitLevel.Id;
                    levelIndex++;
                    levelTrans.Commit();
                }
            }

            // *** LOOP THROUGH BUILDING WALLS ***
            int wallIndex = 0;
            foreach (WallObject _wall in walls)
            {
                Console.WriteLine("wallIndex: " + wallIndex);
                int coordIndex = 0;
                List<Coordinate> wall_coords = _wall.coordinates;
                int coordCount = wall_coords.Count;
                int maxCoordIndex = coordCount - 1;
                // *** DEFINE LIST OF CURVES FOR EACH WALL ***
                List<Curve> wall_curves = new List<Curve>();
                int wallLevelIndex = _wall.level;
                //
                foreach (Coordinate wall_coord in wall_coords)
                {
                    Coordinate startCoord = new Coordinate(0, 0);
                    Coordinate endCoord = new Coordinate(0, 0);
                    if (coordIndex < maxCoordIndex)
                    {
                        startCoord = wall_coords[coordIndex];
                        endCoord = wall_coords[coordIndex + 1];
                    }
                    else if (coordIndex == maxCoordIndex && coordCount > 1)
                    {
                        startCoord = wall_coords[maxCoordIndex];
                        endCoord = wall_coords[0];
                    }
                    Console.WriteLine("      startCoord: " + startCoord.logDisplay());
                    Console.WriteLine("      endCoord: " + endCoord.logDisplay());
                    Console.WriteLine("      ");
                    coordIndex++;
                    // *** CREATE START AND END XYZ OBJECT ***
                    // *** REQUIRES REVIT:
                    XYZ start = new XYZ(startCoord.x, startCoord.y, 0.0);
                    XYZ end = new XYZ(endCoord.x, endCoord.y, 0.0);
                    // *** ADD TO WALL CURVES LIST ***
                    wall_curves.Add(Line.CreateBound(start, end));
                }
                wallIndex++;
                // *** CREATE WALLS USING CURVES ***
                // *** REQUIRES REVIT:
                using (Transaction wallTrans = new Transaction(newDoc, "Create some walls"))
                {
                    wallTrans.Start();
                    createRVTWalls(newDoc, wall_curves, level_IDs[wallLevelIndex]);
                    wallTrans.Commit();
                }
                // *** CREATE FLOOR USING CURVES ***
                /*
                using (Transaction floorTrans = new Transaction(newDoc, "Create a floor"))
                {
                    floorTrans.Start();
                    newDoc.Create.NewFloor(wall_curves, false);
                    floorTrans.Commit();
                }
                */
            }

            // *** SET COLORS TO LEVELS BY ID IN DOC ***
            foreach (var kvp in colored_level_IDs)
            {
                ColorObject _colorObject = kvp.Value;
                Autodesk.Revit.DB.ElementId _levelId = kvp.Key;
                Color _color = new Color(_colorObject.red, _colorObject.green, _colorObject.blue);
                // newDoc.ActiveView.set_ProjColorOverrideByElement(_levelId, _color);
            }

            // *** SAVE NEW DOC
            if (saveFile)
            {
                newDoc.SaveAs(filePath);
            }
        }
    }
}
