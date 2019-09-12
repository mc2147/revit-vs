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
        public string sampleJSONPath = "C:\\Users\\chanmat\\Desktop\\Work\\revit-vs\\sample.json";
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {            
            try
            {
                if (null == commandData)
                {
                    throw new ArgumentNullException("commandData");
                }
                Autodesk.Revit.DB.Document doc = commandData.Application.ActiveUIDocument.Document;

                MassformerJsonToRvt(doc, sampleJSONPath);
                // *** Sample Code:
                //CreateSphereDirectShape(doc);

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