using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebScraping
{
    public class ScrappingLogic
    {
        public const string BaseUrl = "https://www.314e.com/";
        public List<string> VisitedUrls;
        public WordCountDetails WordCountDetails = new WordCountDetails();
        public ScrappingLogic()
        {
            VisitedUrls = new List<string>();
        }

        public WordCountDetails GetWordCountDetails(string url, int top)
        {
            //Start crawling
            Crawl(url, 1);
            var countDetails = new WordCountDetails();
            //Get top records only sorted by count
            var result = WordCountDetails.WordFreqency.OrderByDescending(x => x.Value).Take(top).ToList();
            countDetails.WordFreqency = result.ToDictionary(x => x.Key, x => x.Value);

            result = WordCountDetails.PairWordFreqency.OrderByDescending(x => x.Value).Take(top).ToList();
            countDetails.PairWordFreqency = result.ToDictionary(x => x.Key, x => x.Value);

            return countDetails;
        }

        public void Crawl(string url, int level)
        {
            //If already visted, exit recursion
            if (!VisitedUrls.Contains(url))
            {
                VisitedUrls.Add(url);
            }
            else
            {
                return;
            }

            //If more than 5 , exit recursion
            if (level <= 4)
            {
                var html = GetHTML(url).Result;
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                var wordCount = ProcessWords(html);
                Console.WriteLine($"Word count in {url} is {wordCount}");

                //Process each link
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
                {
                    HtmlAttribute att = link.Attributes["href"];
                    if (!string.IsNullOrWhiteSpace(att.Value) && att.Value.StartsWith(BaseUrl))
                    {
                        //Console.WriteLine(att.Value);

                        Crawl(att.Value, level + 1);
                    }
                }
            }
        }
        public async Task<string> GetHTML(string fullUrl)
        {
            HttpClient client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
            var response = client.GetStringAsync(fullUrl);
            return await response;
        }

        public int ProcessWords(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            char[] delimiter = new char[] { ' ' };
            int count = 0;

            foreach (string text in doc.DocumentNode
                .SelectNodes("//body//text()[not(parent::script|parent::style)]")
                .Select(node => node.InnerText))
            {
                //Remove non alphabetic chars
                Regex rgx = new Regex("[^a-zA-Z ]");
                var result = rgx.Replace(text, "");
                var words = result.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)
                    .Where(s => Char.IsLetter(s[0])).ToList();

                //Add individual word
                foreach (var word in words)
                {
                    if (WordCountDetails.WordFreqency.ContainsKey(word))
                    {
                        WordCountDetails.WordFreqency[word]++;
                    }
                    else
                    {
                        WordCountDetails.WordFreqency.Add(word, 1);
                    }
                }

                int wordCount = words.Count();

                //Add pairs
                //The words seperated by white spaces are considered as consecutive words
                if (wordCount > 1)
                {
                    int i = 0;
                    int j = 1;
                    for (; j < wordCount; i++, j++)
                    {
                        if (WordCountDetails.PairWordFreqency.ContainsKey(words[i] + " " + words[j]))
                        {
                            WordCountDetails.PairWordFreqency[words[i] + " " + words[j]]++;
                        }
                        else
                        {
                            WordCountDetails.PairWordFreqency.Add(words[i] + " " + words[j], 1);
                        }
                        //Console.WriteLine($"Pair : {words[i] + " " + words[j]}");
                    }
                }

                if (wordCount > 0)
                {
                    //Console.WriteLine(String.Join(" ", words));
                    count += wordCount;
                }
            }

            return count;
        }
    }
}
