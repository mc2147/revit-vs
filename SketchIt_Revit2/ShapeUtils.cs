using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
//using Autodesk.Revit.UI;
using static PW.CurveUtils;
using static PW.MassformerUtils;

namespace PW
{
    public class ShapeUtils
    {
        public static Solid CreateShape(
            Autodesk.Revit.DB.Document doc,
            List<List<double>> xy_coords, 
            double offset, 
            double height
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
            ElementId squareId;
            using (Transaction t = new Transaction(doc, "Create square geometry"))
            {
                t.Start();
                DirectShape ds = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));
                ds.SetShape(new GeometryObject[] { newShape });
                squareId = ds.Id;
                t.Commit();
            }
            return newShape;
        }
    }
}
