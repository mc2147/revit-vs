using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Creation;
using static PW.MainUtils;

using RvtApplication = Autodesk.Revit.ApplicationServices.Application;
using RvtDocument = Autodesk.Revit.DB.Document;
using static PW.DebugUtils;

namespace PW
{
    /// <summary>
    /// Implements the Revit add-in interface IExternalCommand
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]

    public class SketchItApp : IExternalCommand
    {
        public string sampleJSONPath = "C:\\Users\\matth\\Desktop\\RevitJSONs\\9-30-19-16-54-21\\sample.json";
        //C:\Users\matth\Desktop\RevitJSONs\9-30-19-16-02-39
        //9-30-19-16-04-33
        //9-30-19-16-06-10
        //9-30-19-16-08-30
        //9-30-19-16-09-11
        //9-30-19-16-09-54
        //9-30-19-16-28-08
        //9-30-19-16-34-04
        //9-30-19-16-45-30
        //9-30-19-16-54-21
        //9-30-19-16-58-24

        public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            DebugLog("\nSKETCHIT.CS LINE 33\n");
            try
            {
                DebugLog("\nSKETCHIT.CS LINE 38\n");
                if (null == commandData)
                {
                    throw new ArgumentNullException("commandData");
                }
                DebugLog("\nSKETCHIT.CS LINE 41\n");
                RvtDocument doc = commandData.Application.ActiveUIDocument.Document;
                MassformerJsonToRvt(doc, sampleJSONPath);
                // *** Sample Code:
                //CreateSphereDirectShape(doc);
                DebugLog("\nSKETCHIT.CS LINE 46\n");

                return Autodesk.Revit.UI.Result.Succeeded;
            }

            catch (Exception ex)
            {
                message = ex.ToString();
                return Autodesk.Revit.UI.Result.Failed;
            }
        }              
    }
}