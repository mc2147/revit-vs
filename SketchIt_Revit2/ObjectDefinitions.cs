using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PW
{
    // *** OBJECT DEFINITIONS: WALL, BUILDING, LEVEL ***
    // *** COORDINATE OBJECT ***
    public class Coordinate
    {
        public double x = 0;
        public double y = 0;
        public Coordinate(double _x, double _y)
        {
            x = _x;
            y = _y;
        }
        public string logDisplay()
        {
            return x + ", " + y;
        }
    }
    // *** COLOR OBJECT ***
    public class ColorObject
    {
        public byte red = 0;
        public byte green = 0;
        public byte blue = 0;
        public ColorObject(byte _red, byte _green, byte _blue)
        {
            red = _red;
            green = _green;
            blue = _blue;
        }
    }
    // *** WALL OBJECT ***
    public class WallObject
    {
        public int level = 0;
        public List<Coordinate> coordinates;
        public WallObject(int _level, List<Coordinate> _coordinates)
        {
            level = _level;
            coordinates = _coordinates;
        }
    }
    // *** LEVEL OBJECT ***
    public class LevelObject
    {
        public double elevation = 0;
        public string category;
        public bool has_category = false;
        public bool has_color = false;
        public ColorObject color;
        public LevelObject(
            double _elevation, string _category,
            bool _has_category,
            bool _has_color,
            byte _red = 0,
            byte _green = 0,
            byte _blue = 0
        )
        {
            elevation = _elevation;
            has_category = _has_category;
            if (_has_category)
            {
                category = _category;
            }
            has_color = _has_color;
            if (_has_color)
            {
                color = new ColorObject(_red, _green, _blue);
            }
        }
    }

}