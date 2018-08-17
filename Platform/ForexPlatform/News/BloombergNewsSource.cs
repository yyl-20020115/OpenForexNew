using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace ForexPlatform
{
    /// <summary>
    /// Implements a site-specific news source, parsing dataDelivery from the bloomberg.com
    /// </summary>
    [EventItemType(typeof(RssNewsEvent))]
    public class BloombergNewsSource : EventSource
    {
        volatile bool _upading = false;

        /// <summary>
        /// Cache if items for the last few days, with titles.
        /// </summary>
        Dictionary<string, RssNewsEvent> _latestNewsItemsTitles = new Dictionary<string, RssNewsEvent>();

        const string BaseAddress = "http://www.bloomberg.com/";

        /// <summary>
        /// Constructor.
        /// </summary>
        public BloombergNewsSource()
        {
            this.Name = "Bloomberg News";
            this.Address = BaseAddress;
            Description = "Bloomberg economic news source.";

            _channels.Clear();

            AddChannel("Exclusive", "http://www.bloomberg.com/news/exclusive/");
            AddChannel("Worldwide", "http://www.bloomberg.com/news/worldwide/");

            AddChannel("Markets Stocks", "http://www.bloomberg.com/news/markets/stocks.html");
            AddChannel("Markets Bonds", "http://www.bloomberg.com/news/markets/bonds.html");
            AddChannel("Markets Commodities", "http://www.bloomberg.com/news/markets/commodities.html");
            AddChannel("Markets Currencies", "http://www.bloomberg.com/news/markets/currencies.html");
            AddChannel("Markets Emerging", "http://www.bloomberg.com/news/markets/emerging_markets.html");
            AddChannel("Markets Energy", "http://www.bloomberg.com/news/markets/energy.html");
            AddChannel("Markets Funds", "http://www.bloomberg.com/news/markets/funds.html");
            AddChannel("Markets Municipal Bonds", "http://www.bloomberg.com/news/markets/muni_bonds.html");

            AddChannel("Industries Consumer", "http://www.bloomberg.com/news/industries/consumer.html");
            AddChannel("Industries Energy", "http://www.bloomberg.com/news/industries/energy.html");
            AddChannel("Industries Finance", "http://www.bloomberg.com/news/industries/finance.html");
            AddChannel("Industries Health Care", "http://www.bloomberg.com/news/industries/health_care.html");
            AddChannel("Industries Insurance", "http://www.bloomberg.com/news/industries/insurance.html");
            AddChannel("Industries Real Estate", "http://www.bloomberg.com/news/industries/real_estate.html");
            AddChannel("Industries Technology", "http://www.bloomberg.com/news/industries/technology.html");
            AddChannel("Industries Transportation", "http://www.bloomberg.com/news/industries/transportation.html");

            AddChannel("Economy", "http://www.bloomberg.com/news/economy/");
            AddChannel("Politics", "http://www.bloomberg.com/news/politics/politics.html");
            AddChannel("Investment", "http://www.bloomberg.com/news/moreinvest.html");
        }

        /// <summary>
        /// Helper. Not thread safe.
        /// </summary>
        void AddChannel(string name, string uri)
        {
            EventSourceChannel channel = new EventSourceChannel(name, true);
            channel.Initialize(this);
            channel.Address = uri;
            base.AddChannel(channel);
        }

        /// <summary>
        /// Intercept call to gaether locally items for filtering.
        /// </summary>
        protected override void channel_ItemsAddedEvent(EventSource source, EventSourceChannel channel, IEnumerable<EventBase> items)
        {
            foreach (RssNewsEvent item in items)
            {
                if ((DateTime.Now - item.DateTime) < TimeSpan.FromDays(7)
                    && _latestNewsItemsTitles.ContainsKey(item.Title) == false)
                {// Gather items from the last 3 days.
                    lock (this)
                    {
                        _latestNewsItemsTitles.Add(item.Title, item);
                    }
                }
            }

            base.channel_ItemsAddedEvent(source, channel, items);
        }


        /// <summary>
        /// Helper. Download HTML and generate a HTMLAgilityPack document from it.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        static HtmlDocument GenerateDocument(string uri)
        {
            HtmlDocument document = null;
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(uri);

                webRequest.Timeout = 25000;
                webRequest.MaximumAutomaticRedirections = 10;
                webRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0b; Windows NT5.0)";
                // Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.9.1.8) Gecko/20100202 Firefox/3.5.8 GTB6 (.NET CLR 3.5.30729)

                using (WebResponse response = webRequest.GetResponse())
                {
                    if (response.ContentLength == 0)
                    {
                        SystemMonitor.OperationError("Failed to obtain web page stream [" + uri + "].");
                        return null;
                    }

                    using (Stream receiveStream = response.GetResponseStream())
                    {// Load document to HtmlAgilityPack document structure.
                        document.Load(receiveStream, Encoding.UTF8);
                    }
                }
            }
            catch (WebException ex)
            {
                SystemMonitor.OperationError("Failed to generate document [" + uri + "]", ex);
                document = null;
            }

            return document;
        }

        /// <summary>
        /// Result is new items found on page.
        /// Page corresponds to a channel.
        /// </summary>
        /// <param name="uri"></param>
        List<EventBase> ProcessPage(EventSourceChannel channel)
        {
            List<EventBase> result = new List<EventBase>();

            HtmlDocument document = GenerateDocument(channel.Address);
            if (document == null)
            {
                return result;
            }
            
            foreach (HtmlNode node in document.DocumentNode.SelectNodes("//a[@class]"))
            {
                if (node.ParentNode.Name == "p" &&
                    node.ParentNode.Attributes["class"] != null
                    && node.ParentNode.Attributes["class"].Value == "summ")
                {
                    string itemTitle = node.ChildNodes[0].InnerText;

                    lock (this)
                    {
                        if (_latestNewsItemsTitles.ContainsKey(itemTitle))
                        {// News already listed.
                            continue;
                        }

                        RssNewsEvent item = CreateNewsItem(node, true);
                        if (item != null)
                        {
                            _latestNewsItemsTitles.Add(itemTitle, item);
                            item.Initialize(channel);
                            result.Add(item);
                        }
                    }
                }
            }

            return result;
        }


        
        /// <summary>
        /// Helper. node.ChildNodes[0].InnerText + ">>" + node.Attributes["href"].Value + "; " + Environment.NewLine;
        /// </summary>
        /// <param name="node"></param>
        /// <param name="fetchDate"></param>
        /// <returns></returns>
        RssNewsEvent CreateNewsItem(HtmlNode node, bool fetchDateAndDetails)
        {
            RssNewsEvent item = new RssNewsEvent();
            item.Author = "Bloomberg";
            item.Comments = string.Empty;

            if (node.ParentNode.Name == "p" && node.ParentNode.ChildNodes[2].Name == "#text")
            {// Description available in parent.
                item.Description = GeneralHelper.RepairHTMLString(node.ParentNode.ChildNodes[2].InnerText);
                item.Description = item.Description.Replace("\n", " ");
            }
            else
            {
                item.Description = "";
            }

            item.Link = new Uri(BaseAddress + node.Attributes["href"].Value).ToString();
            item.Title = node.ChildNodes[0].InnerText.Trim();

            if (fetchDateAndDetails)
            {
                HtmlDocument document = GenerateDocument(item.LinkUri.AbsoluteUri);
                if (document == null)
                {
                    return null;
                }

                HtmlNodeCollection nodes = document.DocumentNode.SelectNodes("//i");

                foreach (HtmlNode iNode in nodes)
                {
                    string dateTimeInfo = iNode.ChildNodes[0].InnerText;
                    DateTime time = GeneralHelper.ParseDateTimeWithZone(dateTimeInfo.Replace("Last Updated:", ""));
                    item.DateTime = time;
                }
            }
            
            return item;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnUpdate()
        {
            if (_upading)
            {
                return;
            }

            int channelId = 0;
            foreach (EventSourceChannel channel in base.Channels)
            {
                if (channel.Enabled)
                {
                    List<EventBase> items = ProcessPage(channel);
                    channel.AddItems(items);
                }
                channelId++;
            }
        }
    }
}
