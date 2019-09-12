using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace PW
{
    public class MassformerBuilding
    {
        public List<List<double>> xycoordinates { get; set; }
        public int floors { get; set; }
        public double floorHeight { get; set; }
        public double groundFloorHeight { get; set; }
        public List<Program> programs { get; set; }
        public string units { get; set; }

    }

    public class Program
    {
        public int floor { get; set; }
        public string name { get; set; }
        public string color { get; set; }
    }

    public class MassformerUtils
    {
        public static MassformerBuilding JsonPathToMFO(string jsonPath)
        {
            string jsonContents = File.ReadAllText(jsonPath);
            //DebugLog("jsonContents: " + jsonContents);
            MassformerBuilding massFormerObject = JsonConvert.DeserializeObject<MassformerBuilding>(jsonContents);
            return massFormerObject;
        }

    }

}
