using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchEngineSystem
{
    public class SearchRequest
    {
        public SearchUser OriginatingSearchUser { get; set; }

        public List<string> Keywords = new List<string>();

        public string GetFormattedKeywordString() { return AssembleSearchPhrase(Keywords); }
        public string PriceMin = string.Empty;
        public string PriceMax = string.Empty;

        public string SearchResultsHtml = string.Empty;
        public List<SearchResult> SearchResultsFound = new List<SearchResult>();

        private string AssembleSearchPhrase(List<string> keywordsToSearchFor)
        {
            string searchString = string.Join("+", keywordsToSearchFor);
            return searchString;
        }
    }
}
