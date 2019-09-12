using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace PW
{
    public class WallUtils
    {
        public static void createRVTWalls(
        Autodesk.Revit.DB.Document targetDoc,
        List<Curve> curves_list,
        Autodesk.Revit.DB.ElementId levelId,
        double height = 40
    )
        {
            using (Transaction wallTrans = new Transaction(targetDoc, "Creating walls"))
            {
                wallTrans.Start();
                foreach (Curve _curve in curves_list)
                {
                    // *** REQUIRES REVIT:
                    Wall newWall = Wall.Create(targetDoc, _curve, levelId, false);

                    IList<Parameter> UnconnectedHeigth_Params = newWall.GetParameters("Unconnected Height");
                    if (UnconnectedHeigth_Params.Count > 0)
                    {
                        if (!UnconnectedHeigth_Params[0].IsReadOnly)
                        {
                            // can set, but the result is not 100
                            UnconnectedHeigth_Params[0].Set(height);
                        }
                    }
                }
                wallTrans.Commit();
            }
        }
    }
}
