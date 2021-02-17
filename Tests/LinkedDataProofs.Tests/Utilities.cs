using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace LinkedDataProfss.Tests
{
    public class Utilities
    {
        public static JObject LoadJson(string filename) => JObject.Parse(File.ReadAllText(filename));
    }
}
