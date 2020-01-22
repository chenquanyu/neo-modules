using Neo.IO.Json;
using System.Linq;

namespace Neo.Plugins
{
    public class RpcRequest
    {
        public JObject Id { get; set; }

        public string RpcVersion { get; set; }

        public string Method { get; set; }

        public JObject[] Params { get; set; }

        public static RpcRequest FromJson(JObject json)
        {
            return new RpcRequest
            {
                Id = json["id"],
                RpcVersion = json["jsonrpc"].AsString(),
                Method = json["method"].AsString(),
                Params = ((JArray)json["params"]).ToArray()
            };
        }

        public JObject ToJson()
        {
            var json = new JObject();
            json["id"] = Id;
            json["jsonrpc"] = RpcVersion;
            json["method"] = Method;
            json["params"] = new JArray(Params);
            return json;
        }
    }
}