using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Autodesk.Revit.DB;
//using Autodesk.Revit.UI;
//using Autodesk.Revit.Creation;
using Autodesk.Revit.ApplicationServices;
using DesignAutomationFramework;

using static PW.MainUtils;

namespace PW
{
    /// <summary>
    /// Implements the Revit add-in interface IExternalCommand
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]

    //public class SketchItApp : IExternalCommand
    public class SketchItApp : IExternalDBApplication
    {
        //public string sampleJSONPath = "C:\\test\\sample.json";
        public string sampleJSONPath = "sample.json";

        public ExternalDBApplicationResult OnStartup(Autodesk.Revit.ApplicationServices.ControlledApplication app)
        {
            DesignAutomationBridge.DesignAutomationReadyEvent += HandleDesignAutomationReadyEvent;
            return ExternalDBApplicationResult.Succeeded;
        }

        public ExternalDBApplicationResult OnShutdown(Autodesk.Revit.ApplicationServices.ControlledApplication app)
        {
            return ExternalDBApplicationResult.Succeeded;
        }

        public void HandleDesignAutomationReadyEvent(object sender, DesignAutomationReadyEventArgs e)
        {
            // Run the application logic.
            
            DesignAutomationData data = e.DesignAutomationData;
            if (data == null)
                throw new InvalidDataException(nameof(data));

            Application rvtApp = data.RevitApp;
            if (rvtApp == null)
                throw new InvalidDataException(nameof(rvtApp));

            //check json if metric or imperial and use different template
            Document newDoc = rvtApp.NewProjectDocument(UnitSystem.Imperial);
            if (newDoc == null)
                throw new InvalidOperationException("Could not create new document.");
            
            MassformerJsonToRvt(newDoc, sampleJSONPath);

            e.Succeeded = true;
        }                    
    }
}