using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using static PW.DebugUtils;

namespace PW
{
    public class CurveUtils
    {
        public static dynamic curvesFromXYZList(
            List<XYZ> xyzList
        )
        {
            dynamic output = new List<Curve>();

            int coordIndex = 0;
            int coordCount = xyzList.Count;
            int maxCoordIndex = coordCount - 1;
              
            foreach (XYZ _xyz in xyzList)
            {
                XYZ start = new XYZ(0, 0, 0);
                XYZ end = new XYZ(0, 0, 0);
                if (coordIndex < maxCoordIndex)
                {
                    start = xyzList[coordIndex];
                    end = xyzList[coordIndex + 1];
                }
                else if (coordIndex == maxCoordIndex && coordCount > 1)
                {
                    start = xyzList[maxCoordIndex];
                    end = xyzList[0];
                }
                coordIndex++;
                output.Add(Line.CreateBound(start, end));
            }
            return output;
        }
        public static List<XYZ> CoordsToXYZ(
            List<List<double>> coords_array,
            double z_offset = 0.0,
            bool coords_have_z = false)
        {
            List<XYZ> XYZList = new List<XYZ>() { };
            foreach (List<double> coords in coords_array)
            {
                DebugLog("Adding XYZ to list: " + coords);
                if (coords_have_z)
                {
                    XYZList.Add(new XYZ(coords[0], coords[1], coords[2]));
                }
                else
                {
                    XYZList.Add(new XYZ(coords[0], coords[1], z_offset));
                }
            }
            return XYZList;
        }
    }
}
