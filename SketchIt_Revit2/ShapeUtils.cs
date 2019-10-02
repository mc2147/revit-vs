using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
//using Autodesk.Revit.UI;
using static PW.CurveUtils;
using static PW.MassformerUtils;
using static PW.DebugUtils;

namespace PW
{
    public class ShapeUtils
    {
        public static Solid CreateShape(
            Autodesk.Revit.DB.Document doc,
            List<List<double>> xy_coords, 
            double offset, 
            double height,
            ElementId fillPatternId
        )
        {
            List<XYZ> XYZList = CoordsToXYZ(xy_coords, offset);
            List<Curve> curveList = curvesFromXYZList(XYZList);
            CurveLoop curveLoop = CurveLoop.Create(curveList);
            //
            Solid newShape = GeometryCreationUtilities.CreateExtrusionGeometry(
               new CurveLoop[] { curveLoop },
               new XYZ(0, 0, 1),
               height
           );
            ElementId squareId = newShape.GraphicsStyleId;
            //         
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
            //ogs.SetProjectionFillColor(color);
            //ogs.SetCutFillColor(color);


            using (Transaction t = new Transaction(doc, "Create square geometry"))
            {
                t.Start();
                DirectShape ds = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));
                ds.SetShape(new GeometryObject[] { newShape });
                squareId = ds.Id;
                t.Commit();
            }
            using (Transaction y = new Transaction(doc, "Shape color"))
            {
                y.Start();
                doc.ActiveView.SetElementOverrides(squareId, ogs);
                y.Commit();
            }
            return newShape;
        }
    }
}
