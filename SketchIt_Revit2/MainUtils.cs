using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Autodesk.Revit.DB;
//using Autodesk.Revit.UI; //No UI for Forge auotmation
using Autodesk.Revit.Creation;
using static PW.RevitUtil;
using static PW.WallUtils;
using static PW.CurveUtils;
using static PW.MassformerUtils;
using static PW.FloorUtils;
using static PW.ShapeUtils;
using static PW.LevelUtils;
using static PW.DebugUtils;
using Newtonsoft.Json;

namespace PW
{
    public class MainUtils
    {
        public static void MassformerJsonToRvt(
           Autodesk.Revit.DB.Document doc,
           string jsonPath,
           bool saveFile = false,
           string saveFilePath = "newBuilding.rvt"
       )
        {
            // *** Extracting data from JSON:
            DebugLog("\n\nLINE 33\n\n ");
            MassformerData MFData = JsonPathToMFO(jsonPath);
            // *** For each MFData object:
            DebugLog("\n\nLINE 36\n\n ");
            CreateRVTModels(doc, MFData);
            // *** Save toggle option
            if (saveFile)
            {
                DebugLog("\n\nSTARTING SAVE\n\n ");
                Transaction saveTrans = new Transaction(doc);
                saveTrans.Start("sketchIt");
                saveTrans.Commit();
                doc.SaveAs(saveFilePath);
                DebugLog("\n\nENDING SAVE\n\n ");
            }
        }

        public static void CreateRVTModels(
           Autodesk.Revit.DB.Document doc,
           MassformerData buildingData
       )
        {
            DebugLog("\n\nLINE 52\n\n ");
            FillPattern newFillPattern = new FillPattern(
                "SolidFill", 
                FillPatternTarget.Drafting, 
                FillPatternHostOrientation.ToView,
                0.5, 0.5, 0.5
            );
            DebugLog("newFillPattern.IsSolidFill: " + newFillPattern.IsSolidFill);
            using (Transaction t = new Transaction(doc, "Create fill pattern"))
            {
                t.Start();
                FillPatternElement patternElement = FillPatternElement.Create(doc, newFillPattern);
                t.Commit();
            }

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<Element> fillPatternElements = collector.OfClass(typeof(FillPatternElement)).ToElements();
            ElementId fillPatternId = fillPatternElements[0].Id;
            foreach (Element fill_pattern in fillPatternElements) {
                DebugLog("fill_pattern.Name: " + fill_pattern.Name + " " + fill_pattern.Id);
                if (fill_pattern.Name == "<Solid fill>")
                {
                    DebugLog("Solid fill found " + fill_pattern.Id);
                    fillPatternId = fill_pattern.Id;
                }
            }
            DebugLog("fillPatternId: " + fillPatternId);

            List<MassformerContext> context = buildingData.context;
            foreach (MassformerContext _context in context) {
                double contextHeight = _context.height;
                List<List<double>> contextXYCoords = _context.xycoordinates;
                if (contextXYCoords.Count > 0)
                {
                    try
                    {
                        // ***
                        CreateShape(doc, contextXYCoords, 0, contextHeight, fillPatternId);
                    }
                    catch(Exception contextError)
                    {
                        DebugLog("contextError: " + contextError + " coords: " + contextXYCoords);
                        string coords_log = "";
                        foreach (List<double> xy_coord in contextXYCoords)
                        {
                            DebugLog("coords: " + xy_coord[0] + " " + xy_coord[1]);
                            coords_log = coords_log + "[" + xy_coord[0] + ", " + xy_coord[1] + "],";
                        }
                        DebugLog("coords_log: " + coords_log);
                    }
                }
            }
            DebugLog("\n\nLINE 80\n\n ");
            // ***
            Dictionary<string, MFB> buildings = buildingData.buildings;
            // ***
            List<MassformerFloor> floorsList = buildingData.floors;
            List<MassformerWall> walls = buildingData.walls;
            string units = buildingData.units;
            // ***
            Autodesk.Revit.DB.ElementId baseLevelId = new ElementId(BuiltInCategory.OST_Levels);
            // *** CREATING FLOORS AND MASSES:
            DebugLog("\n\nLINE 90\n\n ");
            foreach (MassformerFloor _floor in floorsList)
            {
                // 
                double floorZOffset = _floor.zOffset;
                List<List<double>> floorXYCoords = _floor.xycoordinates;
                double massHeight = _floor.massHeight;
                int floorNumber = _floor.floorNumber;
                // ***
                CreateFloor(doc, floorXYCoords, floorZOffset);
                CreateShape(doc, floorXYCoords, floorZOffset, massHeight, fillPatternId);
                //
                ElementId levelId = CreateLevel(doc, floorZOffset);
                if (floorNumber == 1)
                {
                    baseLevelId = levelId;
                }
            }
            DebugLog("\n\nLINE 108\n\n ");
            // *** CREATING WALLS:
            foreach (MassformerWall _wall in walls)
            {
                double wallZOffset = _wall.zOffset;
                List<List<double>> wallXYCoords = _wall.xycoordinates;
                double wallHeight = _wall.height;
                List<XYZ> wallXYZList = CoordsToXYZ(wallXYCoords, wallZOffset);
                List<Curve> wallCurveList = curvesFromXYZList(wallXYZList);
                createRVTWalls(doc, wallCurveList, baseLevelId, wallHeight, wallZOffset);
            }
            DebugLog("\n\nLINE 119\n\n ");
            Color color = new Color(30, 250, 52); // RGB (0, 255, 255)

            OverrideGraphicSettings ogs = new OverrideGraphicSettings();
            //
            ogs.SetCutBackgroundPatternId(fillPatternId);
            ogs.SetCutBackgroundPatternColor(color);
            ogs.SetCutBackgroundPatternVisible(true);
            //
            //ogs.SetProjectionLinePatternId();
            ogs.SetProjectionLineColor(color);
            //
            ogs.SetSurfaceForegroundPatternId(fillPatternId);
            ogs.SetSurfaceForegroundPatternColor(color);
            ogs.SetSurfaceForegroundPatternVisible(true);
            //
            ogs.SetSurfaceBackgroundPatternId(fillPatternId);
            ogs.SetSurfaceBackgroundPatternColor(color);
            ogs.SetSurfaceBackgroundPatternVisible(true);
            //
            ogs.SetCutForegroundPatternId(fillPatternId);
            ogs.SetCutForegroundPatternColor(color);
            ogs.SetCutForegroundPatternVisible(true);
            //
            //ogs.SetCutLinePatternId(fillPatternId);
            ogs.SetCutLineColor(color);
            //
            ogs.SetSurfaceTransparency(0);

            using (Transaction t = new Transaction(doc, "Set Fill Pattern Walls"))
            {
                t.Start();

                foreach (Element wall in new FilteredElementCollector(doc, doc.ActiveView.Id).OfClass(typeof(Wall)))
                {
                    doc.ActiveView.SetElementOverrides(wall.Id, ogs);
                }

                t.Commit();
            }

            using (Transaction t = new Transaction(doc, "Set Fill Pattern Floors"))
            {
                t.Start();

                foreach (Element floor in new FilteredElementCollector(doc, doc.ActiveView.Id).OfClass(typeof(Floor)))
                {
                    doc.ActiveView.SetElementOverrides(floor.Id, ogs);
                }

                t.Commit();
            }

        }
    }
}
