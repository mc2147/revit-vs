using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using static PW.DebugUtils;

namespace PW
{
    public class WallUtils
    {
        public static void createRVTWalls(
        Autodesk.Revit.DB.Document targetDoc,
        List<Curve> curves_list,
        Autodesk.Revit.DB.ElementId levelId,
        double height = 40,
        double zOffset = 0.0
    )
        {
            using (Transaction wallTrans = new Transaction(targetDoc, "Creating walls"))
            {
                try
                {
                    wallTrans.Start();
                    foreach (Curve _curve in curves_list)
                    {
                        var wallTypeId = targetDoc.GetDefaultElementTypeId(ElementTypeGroup.WallType);
                        // *** REQUIRES REVIT:
                        //Wall newWall = Wall.Create(targetDoc, _curve, levelId, false);
                        Wall newWall = Wall.Create(targetDoc, _curve, wallTypeId, levelId, height, zOffset, false, false);

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
                catch (Exception wallCreationError)
                {
                    DebugLog("wallCreationError: " + wallCreationError);
                }
            }
        }
    }
}
