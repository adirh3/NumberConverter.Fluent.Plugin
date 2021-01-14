using System.Collections.Generic;
using Blast.Core.Interfaces;
using Blast.Core.Objects;
using Blast.Core.Results;

namespace NumberConverter.Fluent.Plugin
{
    public sealed class NumberConversionSearchResult : SearchResultBase
    {
        public NumberConversionSearchResult(int number, string searchAppName, string convertedNumber, string resultName, string searchedText,
            string resultType, double score, IList<ISearchOperation> supportedOperations, ICollection<SearchTag> tags,
            ProcessInfo processInfo = null) : base(searchAppName,
            resultName, searchedText, resultType, score,
            supportedOperations, tags, processInfo)
        {
            Number = number;
            ConvertedNumber = convertedNumber;
            // You can add Machine Learning features to improve search predictions
            MlFeatures = new Dictionary<string, string>
            {
                ["ConvertedNumber"] = ConvertedNumber
            };
        }

        public int Number { get; }
        
        public string ConvertedNumber { get; set; }

        protected override void OnSelectedSearchResultChanged()
        {
        }
    }
}