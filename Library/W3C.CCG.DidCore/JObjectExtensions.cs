using System;
using Newtonsoft.Json.Linq;

namespace W3C.CCG.DidCore
{
    public static class JObjectExtensions
    {
        public static void EnhanceContext(this DidDocument obj, JToken context)
        {
            if (obj.Context is null)
            {
                obj.Context = context;
                return;
            }

            switch (obj.Context)
            {
                case JValue _:
                case JObject _:
                    obj.Context = new JArray
                    {
                        obj.Context,
                        context
                    };
                    break;
                case JArray jarr:
                    jarr.Add(context);
                    break;
                default:
                    throw new Exception("Unknown context type");
            }
        }
    }
}
