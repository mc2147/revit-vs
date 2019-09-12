using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Autodesk.Revit.DB;
// using Autodesk.Revit.Creation;
using static PW.RevitUtil;
using static PW.WallUtils;
using static PW.CurveUtils;
using static PW.BuildingObject;

using Autodesk.Revit.ApplicationServices;
using DesignAutomationFramework;

namespace PW
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    // addition from step 2 here --> https://forge.autodesk.com/en/docs/design-automation/v3/tutorials/revit/step1-convert-addin/
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class SketchItApp : IExternalDBApplication
    {
        public ExternalDBApplicationResult OnStartup(Autodesk.Revit.ApplicationServices.ControlledApplication app)
        {
           DesignAutomationBridge.DesignAutomationReadyEvent += HandleDesignAutomationReadyEvent;
           return ExternalDBApplicationResult.Succeeded;
        }

        public ExternalDBApplicationResult OnShutdown(Autodesk.Revit.ApplicationServices.ControlledApplication app)
        {
           return ExternalDBApplicationResult.Succeeded;
        }

        public void HandleDesignAutomationReadyEvent(object sender, DesignAutomationReadyEventArgs e)
        {
           // Run the application logic.
           string message = "";
           SketchItFunc(e.DesignAutomationData, ref message);
           e.Succeeded = true;
        }

        private static void SketchItFunc(DesignAutomationData data, ref string message)
        {
            // <from_sketch_it>
            if (data == null)
                throw new InvalidDataException(nameof(data));
            Application rvtApp = data.RevitApp;
            if (rvtApp == null)
                throw new InvalidDataException(nameof(rvtApp));
            Document newDoc = rvtApp.NewProjectDocument(UnitSystem.Imperial);
            if (newDoc == null)
                throw new InvalidOperationException("Could not create new document.");
            string filePath = "sketchIt.rvt";
            // string filepathJson = "SketchItInput.json";
            // SketchItParams jsonDeserialized = SketchItParams.Parse(filepathJson);
            //  </from_sketch_it>


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
                /*
                if (null == data)
                {
                    throw new ArgumentNullException("data");
                }
                */


                sample_building.createRVTFile(newDoc, "sketchIt.rvt", false);
                CreateSphereDirectShape(newDoc);

                //newTran = new Transaction(doc);
                //newTran.Start("sketchIt");
                //string filepathJson = "c:\\test\\SketchItInput.json";
                //SketchItFunc(filepathJson, doc);
                //newTran.Commit();

                // return Autodesk.Revit.UI.Result.Succeeded;

                newDoc.SaveAs(filePath);
            }

            catch (Exception ex)
            {
                message = ex.ToString();
                // return Autodesk.Revit.UI.Result.Failed;
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
