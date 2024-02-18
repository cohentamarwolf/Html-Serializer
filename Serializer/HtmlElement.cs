using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Serializer
{
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Attributes { get; set; } = new List<string>();
        public List<string> Classes { get; set; } = new List<string>();
        public string InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Childrens { get; set; } = new List<HtmlElement>();
        public IEnumerable<HtmlElement> Descendants(HtmlElement root)
        {
            Queue<HtmlElement> Elements = new Queue<HtmlElement>();
            HtmlElement element;
            Elements.Enqueue(root);
            while (Elements.Count > 0)
            {
                element = Elements.Dequeue();
                foreach (HtmlElement child in element.Childrens)
                {
                    Elements.Enqueue(child);
                }
                yield return element;
            }
        }
        public IEnumerable<HtmlElement> Ancestors(HtmlElement root)
        {
            while (root.Parent != null)
            {
                yield return root.Parent;
                root = root.Parent;
            }
        }
        private bool IsSame(HtmlElement d, Selector selector)
        {
            if (selector.TagName != null && selector.TagName != "" && selector.TagName != d.Name)
                return false;
            if (selector.Id != null && selector.Id != "" && selector.Id != d.Id)
                return false;
            if (selector.Classes != null)
                if (!selector.Classes.All(c => d.Classes.Contains(c)))
                    return false;
            if (selector.Classes != null)
            {
                if (d.Classes != null)
                {
                    if (!selector.Classes.All(s => d.Classes.Contains(s)))
                        return false;
                }
                else
                    return false;
            }
            return true;
        }
        private void SearchElement(HtmlElement element, Selector selector, List<HtmlElement> elementsList)
        {
            if (selector == null)
                return;
            IEnumerable<HtmlElement> descendants = Descendants(element);
            foreach (HtmlElement d in descendants)
            {
                if (IsSame(d, selector))
                {
                    if (selector.Child == null)
                        elementsList.Add(d);
                    else
                        SearchElement(d, selector.Child, elementsList);
                }
            }
        }
        public IEnumerable<HtmlElement> MathElements(Selector selector)
        {
            List<HtmlElement> result = new List<HtmlElement>();
            if (IsSame(this, selector))
            {
                selector = selector.Child;
                if (selector == null)
                    result.Add(this);
            }
            SearchElement(this, selector, result);
            var setResult = new HashSet<HtmlElement>(result);
            return setResult;
        }

    }
}
