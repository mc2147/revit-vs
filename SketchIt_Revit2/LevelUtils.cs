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
    public class LevelUtils
    {
        public static ElementId CreateLevel(
            Autodesk.Revit.DB.Document doc,
            double elevation,
            string name = ""
        )
        {
            using (Transaction levelTrans = new Transaction(doc, "Creating levels"))
            {
                levelTrans.Start();
                Level newLevel = Level.Create(doc, elevation);
                if (name != "") { newLevel.Name = name; }
                levelTrans.Commit();
                return newLevel.Id;
            }
        }
    }
}
