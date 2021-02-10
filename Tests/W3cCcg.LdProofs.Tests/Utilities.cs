using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace W3cCcg.LdProofs.Tests
{
    public class Utilities
    {
        public static JObject LoadJson(string filename) => JObject.Parse(File.ReadAllText(filename));
    }
}
