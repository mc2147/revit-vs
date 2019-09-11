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
using static PW.BuildingObject;

namespace PW
{
    /// <summary>
    /// Implements the Revit add-in interface IExternalCommand
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class SketchItApp : IExternalDBApplication
    {
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            //Transaction newTran = null;
            // *** CODE IS RUN HERE
            Console.WriteLine("INITIALIZING. LINE 25\n");
            //
            Coordinate x1 = new Coordinate(100, 0);
            Coordinate x2 = new Coordinate(0, 100);
            Coordinate x3 = new Coordinate(0, 0);
            //
            LevelObject level_1 = new LevelObject(10, "Test 1", true, false);
            LevelObject level_2 = new LevelObject(20, "Test 2", true, false);
            LevelObject level_3 = new LevelObject(30, "Test 3", true, false);
            LevelObject level_4 = new LevelObject(40, "Lobby", true, true, 255, 165, 0);
            //
            List<Coordinate> building_xy_coords = new List<Coordinate> { x1, x2, x3 };
            WallObject wall_1 = new WallObject(0, building_xy_coords);
            // WallObject wall_2 = new WallObject(1, building_xy_coords);
            // WallObject wall_3 = new WallObject(2, building_xy_coords);
            // WallObject wall_4 = new WallObject(3, building_xy_coords);
            //
            List<WallObject> sample_building_walls = new List<WallObject> { wall_1 };
                // wall_2, wall_3, wall_4
          
            List<LevelObject> sample_building_levels = new List<LevelObject> { level_1, level_2, level_3, level_4 };
            //
            BuildingObject sample_building = new BuildingObject(
              sample_building_walls,
              sample_building_levels,
              building_xy_coords
            );
            sample_building.logDetails();
            //Transaction newTran = null;
            try
            {
                if (null == commandData)
                {
                    throw new ArgumentNullException("commandData");
                }

                Autodesk.Revit.DB.Document doc = commandData.Application.ActiveUIDocument.Document;
                sample_building.createRVTFile(doc, "sketchIt.rvt", false);
                CreateSphereDirectShape(doc);

                //newTran = new Transaction(doc);
                //newTran.Start("sketchIt");
                //string filepathJson = "c:\\test\\SketchItInput.json";
                //SketchItFunc(filepathJson, doc);
                //newTran.Commit();
                return Autodesk.Revit.UI.Result.Succeeded;
            }

            catch (Exception ex)
            {
                message = ex.ToString();
                return Autodesk.Revit.UI.Result.Failed;
            }
        }

        public static void CreateSphereDirectShape(Autodesk.Revit.DB.Document doc)
        {
            List<Curve> profile = new List<Curve>();

            // first create sphere with 2' radius
            XYZ center = XYZ.Zero;
            double radius = 2.0;
            XYZ profile00 = center;
            XYZ profilePlus = center + new XYZ(0, radius, 0);
            XYZ profileMinus = center - new XYZ(0, radius, 0);

            profile.Add(Line.CreateBound(profilePlus, profileMinus));
            profile.Add(Arc.Create(profileMinus, profilePlus, center + new XYZ(radius, 0, 0)));

            CurveLoop curveLoop = CurveLoop.Create(profile);
            SolidOptions options = new SolidOptions(ElementId.InvalidElementId, ElementId.InvalidElementId);

            Frame frame = new Frame(center, XYZ.BasisX, -XYZ.BasisZ, XYZ.BasisY);
            Solid sphere = GeometryCreationUtilities.CreateRevolvedGeometry(frame, new CurveLoop[] { curveLoop }, 0, 2 * Math.PI, options);
            using (Transaction t = new Transaction(doc, "Create sphere direct shape"))
            {
                t.Start();
                // create direct shape and assign the sphere shape
                //properties to set application id and application data id.")]
                //DirectShape.ApplicationId = "Application id",
                //DirectShape.ApplicationDataId ="Geometry object id"
                //public static DirectShape CreateElement();
                
                DirectShape ds = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));                                               
                ds.SetShape(new GeometryObject[] { sphere });
                t.Commit();
            }
        }
       
    }
}
