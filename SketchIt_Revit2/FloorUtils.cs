using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using static PW.CurveUtils;
using static PW.MassformerUtils;

namespace PW
{
    public class FloorUtils
    {
        public static void CreateFloor(
            Autodesk.Revit.DB.Document doc,
            List<List<double>> xy_coords,
            double zOffset
        )
        {
            List<XYZ> floorXYZList = CoordsToXYZ(xy_coords, zOffset);
            List<Curve> floorCurvesList = curvesFromXYZList(floorXYZList);
            using (Transaction floorTrans = new Transaction(doc, "Create a floor"))
            {
                floorTrans.Start();
                CurveArray floorCurveArray = new CurveArray();
                foreach (Curve _curve in floorCurvesList)
                {
                    floorCurveArray.Append(_curve);
                }
                doc.Create.NewFloor(floorCurveArray, false);
                floorTrans.Commit();
            }
        }
    }
}
