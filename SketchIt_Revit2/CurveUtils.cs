using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace PW
{
    public class CurveUtils
    {
        public static CurveArray curveArrayFromCoords(
           List<Coordinate> xy_coords,
           double elevation = 0.0,
           bool threeDimensional = false
       )
        {
            CurveArray curve_array = new CurveArray();
            //new List<Curve>();
            // 
            int coordIndex = 0;
            int coordCount = xy_coords.Count;
            int maxCoordIndex = coordCount - 1;
            // 
            foreach (Coordinate xy in xy_coords)
            {
                Coordinate startCoord = new Coordinate(0, 0);
                Coordinate endCoord = new Coordinate(0, 0);
                if (coordIndex < maxCoordIndex)
                {
                    startCoord = xy_coords[coordIndex];
                    endCoord = xy_coords[coordIndex + 1];
                }
                else if (coordIndex == maxCoordIndex && coordCount > 1)
                {
                    startCoord = xy_coords[maxCoordIndex];
                    endCoord = xy_coords[0];
                }
                coordIndex++;
                // *** CREATE START AND END XYZ OBJECT ***
                // *** REQUIRES REVIT:
                // decimal x = startCoord.x;
                // decimal y = startCoord.y;
                // 
                if (threeDimensional)
                {
                    XYZ startLow = new XYZ(startCoord.x, startCoord.y, 0.0);
                    XYZ endLow = new XYZ(endCoord.x, endCoord.y, 0.0);
                    // 
                    XYZ startHigh = new XYZ(startCoord.x, startCoord.y, elevation);
                    XYZ endHigh = new XYZ(endCoord.x, endCoord.y, elevation);
                    // 
                    curve_array.Append(Line.CreateBound(startLow, endLow));
                    curve_array.Append(Line.CreateBound(endLow, endHigh));
                    curve_array.Append(Line.CreateBound(endHigh, startHigh));
                    curve_array.Append(Line.CreateBound(startHigh, startLow));
                }
                else
                {
                    XYZ start = new XYZ(startCoord.x, startCoord.y, elevation);
                    XYZ end = new XYZ(endCoord.x, endCoord.y, elevation);
                    // ADD TO curve_array ***
                    curve_array.Append(Line.CreateBound(start, end));
                }
            }
            return curve_array;
        }
    }
}
