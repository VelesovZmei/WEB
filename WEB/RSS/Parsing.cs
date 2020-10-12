using System.Linq;
using HtmlAgilityPack;

namespace WEB.RSS
{
    public static class Parsing
    {
        public static string TutByParseNews(string newsUrl)
        {

            var web = new HtmlWeb();
            var doc = web.Load(newsUrl);
            var docNode = doc.DocumentNode;
            return docNode.Descendants("div")
                .Where(d => d.Id == "article_body")
                .FirstOrDefault()?
                .InnerHtml;
        }

        public static string OnlinerParseNews(string newsUrl)
        {
            var web = new HtmlWeb();
            var doc = web.Load(newsUrl);
            var docNode = doc.DocumentNode;
            return docNode.Descendants()
                .Where(d => d.Name == "div")
                .Where(d => d.Attributes.FirstOrDefault().Name == "class")
                .Where(d => d.Attributes.FirstOrDefault().Value == "news-text")
                .FirstOrDefault()?
                .InnerHtml;
        }

        //public static string S13ParseNews(string newsUrl)
        //{

        //    var web = new HtmlWeb();
        //    var doc = web.Load(newsUrl);
        //    var docNode = doc.DocumentNode;
        //    return docNode.Descendants("div")
        //        .Where(d => d.Attributes["class"].Value == "adcd1194b61eedc58d59107552c8674a1 b880x90")
        //        .FirstOrDefault()?
        //        .InnerHtml;
        //}
    }
}
