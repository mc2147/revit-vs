using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
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
            MassformerData MFData = JsonPathToMFO(jsonPath);
            // *** For each MFData object:
            CreateRVTModels(doc, MFData);
            // *** Save toggle option
            if (saveFile)
            {
                Transaction saveTrans = new Transaction(doc);
                saveTrans.Start("sketchIt");
                doc.SaveAs(saveFilePath);
                saveTrans.Commit();
            }
        }

        public static void CreateRVTModels(
            Autodesk.Revit.DB.Document doc,
            MassformerData buildingData
        )
        {
            Dictionary<string, MFB> buildings = buildingData.buildings;
            // ***
            List<MassformerFloor> floorsList = buildingData.floors;
            List<MassformerWall> walls = buildingData.walls;
            string units = buildingData.units;
            // ***
            Autodesk.Revit.DB.ElementId baseLevelId = new ElementId(BuiltInCategory.OST_Levels);
            // *** CREATING FLOORS AND MASSES:
            foreach (MassformerFloor _floor in floorsList) {
                // 
                double floorZOffset = _floor.zOffset;
                List<List<double>> floorXYCoords = _floor.xycoordinates;
                double massHeight = _floor.massHeight;
                int floorNumber = _floor.floorNumber;
                // ***
                CreateFloor(doc, floorXYCoords, floorZOffset);
                CreateShape(doc, floorXYCoords, floorZOffset, massHeight);
                //
                ElementId levelId = CreateLevel(doc, floorZOffset, "My Level " + (floorNumber - 1));
                if (floorNumber == 1) {
                    baseLevelId = levelId;
                }
            }
            // *** CREATING WALLS:
            foreach (MassformerWall _wall in walls) {
                double wallZOffset = _wall.zOffset;
                List<List<double>> wallXYCoords = _wall.xycoordinates;
                double wallHeight = _wall.height;
                List<XYZ> wallXYZList = CoordsToXYZ(wallXYCoords, wallZOffset);
                List<Curve> wallCurveList = curvesFromXYZList(wallXYZList);                
                createRVTWalls(doc, wallCurveList, baseLevelId, wallHeight, wallZOffset);
            }
            // // 
            // //DebugLog("buildingData: " + buildingData);
            // int floors = buildingData.floors;
            // double groundFloorHeight = buildingData.groundFloorHeight;
            // double floorHeight = buildingData.floorHeight;
            // double buildingHeight = groundFloorHeight + floorHeight * (floors - 1);
            // List<List<double>> xy_coords = buildingData.xycoordinates;
            // List<Program> buildingPrograms = buildingData.programs;
            // List<XYZ> XYZList = CoordsToXYZ(buildingData.xycoordinates);
            // List<Curve> curveList = curvesFromXYZList(XYZList);
            // //DebugLog("XYZList: " + XYZList);
            // //DebugLog("curveList: " + curveList);
            // // *** Create first floor with first floor height
            // // *** Create subsequent floors and shapes with floorHeight
            // //CurveLoop curveLoop = CurveLoop.Create(curveList);
            // //CreateFloor(doc, xy_coords, 0);
            // //CreateShape(doc, xy_coords, 0, groundFloorHeight);
            // for (int floorIndex = 0; floorIndex < floors; floorIndex++)
            // {
            //     //*** 0 for ground, gfH for 1st index = 1, gFH + fI * fH for index > 1
            //     double zOffset = floorIndex == 0 ? 0
            //         :
            //         floorIndex == 1 ?
            //             groundFloorHeight
            //             :
            //             groundFloorHeight + (floorIndex - 1) * floorHeight;
            //     double shapeHeight = floorIndex == 0 ? groundFloorHeight : floorHeight;
            //     CreateFloor(doc, xy_coords, zOffset);
            //     CreateShape(doc, xy_coords, zOffset, shapeHeight);
            //     ElementId levelId = CreateLevel(doc, zOffset, "Matt's Level " + floorIndex);
            //     if (floorIndex == 0) { baseLevelId = levelId; }
            // }
            // // CreateWalls(doc, xy_coords, zOffset, height)
            // // *** Create walls
            // createRVTWalls(doc, curveList, baseLevelId, buildingHeight);
        }
    }
}
