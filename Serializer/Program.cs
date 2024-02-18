// See https://aka.ms/new-console-template for more information
using Serializer;
using System;
using System.Text.RegularExpressions;

var html = await Load("https://learn.malkabruk.co.il/");
var cleanHtml = new Regex("\\s+(?=<[^<>]+>)", RegexOptions.Singleline).Replace(html, "");
var finalCleanHtml = new Regex("\\s+", RegexOptions.Singleline).Replace(cleanHtml, " ");
var htmlLines = new Regex("<(.*?)>").Split(finalCleanHtml).Where(s => s.Length > 0);

HtmlElement htmlTree = CreateTree();
//PrintTree(htmlTree, 0);

Selector selectorTree = new Selector();
string strToSearch = "div .home-container header.home-navbar-interactive  #profile-menu a";
//string strToSearch = "div#profile-menu";
//string strToSearch = ".home-hero-heading";
//string strToSearch = " div .home-hero-heading.heading1";

selectorTree = Selector.Casting(strToSearch);
IEnumerable<HtmlElement> res = htmlTree.MathElements(selectorTree);
//res.ToList().ForEach(e => PrintHtmlElement(e));
//PrintSelector(selectorTree);

Console.ReadLine();
HtmlElement CreateTree()
{
    HtmlElement root = null;
    HtmlElement currentElement = null;
    htmlLines = htmlLines.Skip(1);
    foreach (var line in htmlLines)
    {
        string firstWord = line.Split(' ')[0];
        if (firstWord == "/html")
            break;
        if (root != null && firstWord != "" && firstWord[0] == '/')
        {
            if (HtmlHelper.Instance.AllTags.Contains(firstWord.Substring(1)))
                currentElement = currentElement.Parent;
        }
        else if (HtmlHelper.Instance.AllTags.Contains(firstWord))
        {
            HtmlElement myElement = new HtmlElement();
            myElement.Name = firstWord;
            myElement.Parent = currentElement;
            var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(line);
            foreach (Match attribute in attributes)
            {
                if (attribute.Groups[1].Value == "id")
                    myElement.Id = attribute.Groups[2].Value;
                else if (attribute.Groups[1].Value == "class")
                    myElement.Classes = new List<string>(attribute.Groups[2].Value.Split(" "));
                else
                    myElement.Attributes.Add(attribute.ToString());
            }
            if (root == null)
            {
                root = myElement;
                currentElement = root;
            }
            else
                currentElement.Childrens.Add(myElement);
            if (!HtmlHelper.Instance.SelfClosingTags.Contains(firstWord))
                currentElement = myElement;

        }
        else
        {
            currentElement.InnerHtml = line;
        }

    }
    return root;
}
async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}
static void PrintTree(HtmlElement element, int depth)
{
    Console.WriteLine(new string(' ', depth * 2) + element.Name);

    foreach (var child in element.Childrens)
    {
        PrintTree(child, depth + 1);
    }
}
static void PrintSelector(Selector s)
{
    while (s != null)
    {
        if (s.TagName != null)
            Console.WriteLine("tagName={0} ", s.TagName);
        if (s.Id != null)
            Console.WriteLine("id: " + s.Id);
        if (s.Classes != null)
            foreach (var c in s.Classes)
                Console.Write(" : " + c);
        Console.WriteLine();
        s = s.Child;
        Console.WriteLine("----------------------");
    }
}
static void PrintHtmlElement(HtmlElement e)
{
    if (e.Name != null)
        Console.WriteLine("name : " + e.Name);
    if (e.Id != null)
        Console.WriteLine("id: " + e.Id);
    if (e.Classes != null)
        foreach (var c in e.Classes)
            Console.Write(" : " + c);
    Console.WriteLine();
    Console.WriteLine("---------------");
}