using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace K180239_Q2
{
    public class NewsFeedService
    {
        private Thread _thread;
        private readonly System.Timers.Timer _timer;

        public NewsFeedService()
        {
            _timer = new System.Timers.Timer(25000) { AutoReset = true };
            _timer.Elapsed += TimerEvent;
        }

        private void TimerEvent(object sender, ElapsedEventArgs e)
        {
            _thread = new System.Threading.Thread(RssFeedNews);

            _thread.Start();

            Console.WriteLine("Execution Start...");


        }

        private void RssFeedNews()
        {
            XmlSerializer xml = new XmlSerializer(typeof(List<NewsItem>));
            List<NewsItem> news = new List<NewsItem>();
            FileStream file = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "RssFeedNews.xml", FileMode.Append, FileAccess.Write,FileShare.None);
            Console.WriteLine("Data Store in the RssFeedNews.xml and file is at the below Location of the Directory");
            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory + "RssFeedNews.xml");
            String[] urls = {"https://www.express.pk/feed/" , "https://www.samaa.tv/urdu/feed/"};

            foreach (var url in urls)
            {
                XDocument doc = XDocument.Load(url);
                var NewsFeeds = (from feed in doc.Descendants("item")
                             from ch in doc.Descendants("channel")  
                             select new NewsItem
                             {
                                 Title = feed.Element("title").Value,
                                 Description = feed.Element("description").Value,
                                 PublishedDate = DateTime.Parse(feed.Element("pubDate").Value),
                                 NewsChannel = ch.Element("title").Value, 

                             }).ToList();  

                news.AddRange(NewsFeeds);
            }
            
            List<NewsItem> SortedNewsList = news.OrderByDescending(order => order.PublishedDate).ToList();
            xml.Serialize(file, SortedNewsList);

            file.Close();
            }


        public void Start()
        {
            _timer.Start();
        }
        public void Stop()
        {
            _timer.Stop();
        }
    }
}