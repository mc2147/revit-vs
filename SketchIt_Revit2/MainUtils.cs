using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Autodesk.Revit.DB;
//using Autodesk.Revit.UI;
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
            MassformerBuilding buildingData = JsonPathToMFO(jsonPath);
            // *** For each buildingData object:
            CreateRVTBuilding(doc, buildingData);
            // *** Save toggle option
            if (saveFile)
            {
                Transaction saveTrans = new Transaction(doc);
                saveTrans.Start("sketchIt");
                doc.SaveAs(saveFilePath);
                saveTrans.Commit();
            }
        }

        public static void CreateRVTBuilding(
            Autodesk.Revit.DB.Document doc,
            MassformerBuilding buildingData
        )
        {
            //DebugLog("buildingData: " + buildingData);
            int floors = buildingData.floors;
            double groundFloorHeight = buildingData.groundFloorHeight;
            double floorHeight = buildingData.floorHeight;
            double buildingHeight = groundFloorHeight + floorHeight * (floors - 1);
            List<List<double>> xy_coords = buildingData.xycoordinates;
            List<Program> buildingPrograms = buildingData.programs;
            List<XYZ> XYZList = CoordsToXYZ(buildingData.xycoordinates);
            List<Curve> curveList = curvesFromXYZList(XYZList);
            //DebugLog("XYZList: " + XYZList);
            //DebugLog("curveList: " + curveList);
            Autodesk.Revit.DB.ElementId baseLevelId = new ElementId(BuiltInCategory.OST_Levels);
            // *** Create first floor with first floor height
            // *** Create subsequent floors and shapes with floorHeight
            //CurveLoop curveLoop = CurveLoop.Create(curveList);
            //CreateFloor(doc, xy_coords, 0);
            //CreateShape(doc, xy_coords, 0, groundFloorHeight);
            for (int floorIndex = 0; floorIndex < floors; floorIndex++)
            {
                //*** 0 for ground, gfH for 1st index = 1, gFH + fI * fH for index > 1
                double zOffset = floorIndex == 0 ? 0
                    :
                    floorIndex == 1 ?
                        groundFloorHeight
                        :
                        groundFloorHeight + (floorIndex - 1) * floorHeight;
                double shapeHeight = floorIndex == 0 ? groundFloorHeight : floorHeight;
                CreateFloor(doc, xy_coords, zOffset);
                CreateShape(doc, xy_coords, zOffset, shapeHeight);
                ElementId levelId = CreateLevel(doc, zOffset, "Matt's Level " + floorIndex);
                if (floorIndex == 0) { baseLevelId = levelId; }
            }
            // *** Create walls
            createRVTWalls(doc, curveList, baseLevelId, buildingHeight);
        }
    }
}
