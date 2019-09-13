using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
//using Autodesk.Revit.UI;
//using Autodesk.Revit.Creation;

// *** CAN DELETE
namespace PW
{
    public class SampleCode
    {
        public static void CreateSquareGeometry(Autodesk.Revit.DB.Document doc)
        {
            List<Curve> curve_list = new List<Curve>();
            // Create square with height of 200 from (200, 200) to (400, 400)
            XYZ corner1 = new XYZ(200, 200, 100); //Bottom-left
            XYZ corner2 = new XYZ(400, 200, 100); //Bottom-right
            XYZ corner3 = new XYZ(400, 400, 100); //Top-right
            XYZ corner4 = new XYZ(200, 400, 100); //Top-left
            //
            XYZ corner1b = new XYZ(200, 200, 200); //Bottom-left
            XYZ corner2b = new XYZ(400, 200, 200); //Bottom-right
            XYZ corner3b = new XYZ(400, 400, 200); //Top-right
            XYZ corner4b = new XYZ(200, 400, 200); //Top-left
            //
            XYZ a = new XYZ(0, 0, 0);
            XYZ b = new XYZ(-153.01315727621798, 95.14743703616917, 0);
            XYZ c = new XYZ(-66.44622459008964, 177.50571874251622, 0);
            XYZ d = new XYZ(-51.62604874132977, 156.06047555378493, 0);
            XYZ e = new XYZ(-76.36582853322017, 118.77469525023317, 0);
            XYZ f = new XYZ(-11.878075205469083, 79.04100048170243, 0);
            XYZ g = new XYZ(17.916635523231175, 106.95779446862221, 0);
            XYZ h = new XYZ(50.89222228435639, 86.43525817008826, 0);
            //
            curve_list.Add(Line.CreateBound(a, b));
            curve_list.Add(Line.CreateBound(b, c));
            curve_list.Add(Line.CreateBound(c, d));
            curve_list.Add(Line.CreateBound(d, e));
            curve_list.Add(Line.CreateBound(e, f));
            curve_list.Add(Line.CreateBound(f, g));
            curve_list.Add(Line.CreateBound(g, h));
            curve_list.Add(Line.CreateBound(h, a));
            //
            //curve_list.Add(Line.CreateBound(corner1, corner2));
            //curve_list.Add(Line.CreateBound(corner2, corner3));
            //curve_list.Add(Line.CreateBound(corner3, corner4));
            //curve_list.Add(Line.CreateBound(corner4, corner1));
            //
            //            curve_list.Add(Line.CreateBound(corner1b, corner2b));
            //            curve_list.Add(Line.CreateBound(corner2b, corner3b));
            //            curve_list.Add(Line.CreateBound(corner3b, corner4b));
            //            curve_list.Add(Line.CreateBound(corner4b, corner1b));
            //
            //            curve_list.Add(Line.CreateBound(corner1, corner1b));
            //            curve_list.Add(Line.CreateBound(corner2, corner2b));
            //            curve_list.Add(Line.CreateBound(corner3, corner3b));
            //            curve_list.Add(Line.CreateBound(corner4, corner4b));
            //
            CurveLoop curveLoop = CurveLoop.Create(curve_list);
            //

            Solid newSquare = GeometryCreationUtilities.CreateExtrusionGeometry(new CurveLoop[] { curveLoop }, new XYZ(0, 0, 1), 500);
            ElementId squareId;

            using (Transaction t = new Transaction(doc, "Create square geometry"))
            {
                t.Start();
                // create direct shape and assign the sphere shape
                //properties to set application id and application data id.")]
                //DirectShape.ApplicationId = "Application id",
                //DirectShape.ApplicationDataId ="Geometry object id"
                //public static DirectShape CreateElement();

                DirectShape ds = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));
                ds.SetShape(new GeometryObject[] { newSquare });
                squareId = ds.Id;
                t.Commit();
            }
            using (Transaction t = new Transaction(doc, "Change square Color"))
            {
                t.Start();

                Color color = new Color(0, 165, 255); // RGB (0, 255, 255)
                OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                ogs.SetProjectionLineColor(color);
                // ogs.SetProjectionFillColor(color);
                ogs.SetSurfaceForegroundPatternColor(color);
                //ogs.SetCutFillColor(color);
                ogs.SetCutLineColor(color);
                doc.ActiveView.SetElementOverrides(squareId, ogs);
                t.Commit();
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
