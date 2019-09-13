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

    public class MassformerData
    {
        public Dictionary<string, MFB> buildings { get; set; }
        public List<MassformerWall> walls { get; set; }
        public List<MassformerFloor> floors { get; set; }
        public string units { get; set; }
    }

    public class MFB {
        public string buildingCode { get; set; }
        public string name { get; set; }
        public double groundFloorHeight { get; set; }
        public double floorHeight { get; set; }
        public int option { get; set; }
        public int floorCount { get; set; }
        public double totalHeight { get; set; }
    }
    public class MassformerFloor {
        public List<List<double>> xycoordinates { get; set; }
        public int floorNumber { get; set; }
        public double zOffset { get; set; }
        public double massHeight { get; set; }
        public string buildingCode { get; set; }
        public string buildingName { get; set; }
        public int option { get; set; }
        public string program { get; set; }
    }

    public class MassformerWall {
        public List<List<double>> xycoordinates { get; set; }
        public double zOffset { get; set; }
        public double height { get; set; }
    }
    public class Program
    {
        public int floor { get; set; }
        public string name { get; set; }
        public string color { get; set; }
    }

    public class MassformerUtils
    {
        public static MassformerData JsonPathToMFO(string jsonPath)
        {
            string jsonContents = File.ReadAllText(jsonPath);
            //DebugLog("jsonContents: " + jsonContents);
            MassformerData MFData = JsonConvert.DeserializeObject<MassformerData>(jsonContents);
            return MFData;
        }

    }

}
