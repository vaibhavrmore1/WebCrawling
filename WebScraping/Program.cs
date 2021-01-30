using System;

namespace WebScraping
{
    class Program
    {
        public static void Main(string[] args)
        {
            var url = "https://www.314e.com/";
            var scraper = new ScrappingLogic();
            var wordCountDetails = scraper.GetWordCountDetails(url, 10);
            int i = 1;
            //Every single letter seperated by space is assumed as a word.
            Console.WriteLine("Top 10 frequent words are:");
            foreach (var item in wordCountDetails.WordFreqency)
            {
                Console.WriteLine($"{i++}. {item.Key} : {item.Value}");
            }

            i = 1;
            Console.WriteLine("Top 10 frequent word pairs are:");
            foreach (var item in wordCountDetails.PairWordFreqency)
            {
                Console.WriteLine($"{i++}. {item.Key} : {item.Value}");
            }

        }
    }
}
