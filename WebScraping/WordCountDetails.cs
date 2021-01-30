
using System.Collections.Generic;

namespace WebScraping
{
    public class WordCountDetails
    {
        public WordCountDetails()
        {
            WordFreqency = new Dictionary<string, int>();
            PairWordFreqency = new Dictionary<string, int>();
        }
        public Dictionary<string, int> WordFreqency { get; set; }
        public Dictionary<string, int> PairWordFreqency { get; set; }
    }
}
