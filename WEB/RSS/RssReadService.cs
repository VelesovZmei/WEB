using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;

using Serilog;

namespace WEB.RSS
{
    public class RssReadService
    {
        public List<SyndicationItem> ReadRss(string rssUrl, double pastHours)
        {
            var newsItems = new List<SyndicationItem>();

            using (XmlReader reader = XmlReader.Create(rssUrl))
            {
                var feed = SyndicationFeed.Load(reader);
                var now = DateTimeOffset.Now;
                newsItems.AddRange(feed.Items
                    .Where(x => (now - x.PublishDate).TotalHours <= pastHours));
            }

            return newsItems;
        }
    }
}
