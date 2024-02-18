using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Serializer
{
    public class HtmlHelper
    {
        private static readonly HtmlHelper _instance = new HtmlHelper();
        public static HtmlHelper Instance => _instance;
        public string[] AllTags { get; set; }
        public string[] SelfClosingTags { get; set; }
        private HtmlHelper()
        {
            AllTags = JsonSerializer.Deserialize<string[]>(File.ReadAllText("AllTags.json"));
            SelfClosingTags = JsonSerializer.Deserialize<string[]>(File.ReadAllText("SelfClosingTags.json"));
        }
    }
}
