using Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Serializer
{
    public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; }
        public Selector Parent { get; set; }
        public Selector Child { get; set; }
        public static Selector Casting(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("Selector string cannot be null or empty.");
            Selector root = null;
            Selector current = null;
            List<string> selectors = str.Split(' ').ToList();

            foreach (string s in selectors)
            {
                Selector temp = new Selector();
                var attributes = new Regex("^(.*?)?(#.*?)?(\\..*?)?$").Matches(s);
                for (int i = 1; i < attributes[0].Groups.Count; i++)
                {
                    if (HtmlHelper.Instance.AllTags.Contains(attributes[0].Groups[i].Value))
                        temp.TagName = attributes[0].Groups[1].Value;
                    else if (attributes[0].Groups[i].Value.StartsWith('#'))
                        temp.Id = attributes[0].Groups[i].Value.Substring(1);
                    else if (attributes[0].Groups[i].Value.StartsWith('.'))
                    {
                        temp.Classes = new List<string>();
                        temp.Classes = attributes[0].Groups[i].Value.Split('.').ToList();
                        temp.Classes.Remove("");
                    }
                }
                if (root == null)
                {
                    root = temp;
                    current = root;
                }
                else
                {
                    temp.Parent = current;
                    current.Child = temp;
                    current = temp;
                }

            }
            return root;
        }
    }
}

