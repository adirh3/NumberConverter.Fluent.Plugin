using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Blast.Core;
using Blast.Core.Interfaces;
using Blast.Core.Objects;
using Blast.Core.Results;

namespace NumberConverter.Fluent.Plugin
{
    public class NumberConversionSearchApp : ISearchApplication
    {
        private const string SearchAppName = "NumberConvertor";
        private readonly List<SearchTag> _searchTags;
        private readonly SearchApplicationInfo _applicationInfo;
        private readonly List<ISearchOperation> _supportedOperations;

        public NumberConversionSearchApp()
        {
            // For icon glyphs look at https://docs.microsoft.com/en-us/windows/uwp/design/style/segoe-ui-symbol-font
            _searchTags = new List<SearchTag>
            {
                new SearchTag
                    {Name = ConversionType.Hex.ToString(), IconGlyph = "\uE8EF", Description = "Convert to hex"},
                new SearchTag
                    {Name = ConversionType.Binary.ToString(), IconGlyph = "\uE8EF", Description = "Convert to binary"}
            };
            _supportedOperations = new List<ISearchOperation>();
            _applicationInfo = new SearchApplicationInfo(SearchAppName,
                "This apps converts hex to decimal", _supportedOperations)
            {
                MinimumSearchLength = 1,
                IsProcessSearchEnabled = false,
                IsProcessSearchOffline = false,
                ApplicationIconGlyph = "\uE8EF",
                SearchAllTime = ApplicationSearchTime.Fast,
                DefaultSearchTags = _searchTags
            };
        }

        public ValueTask LoadSearchApplicationAsync()
        {
            // This is used if you need to load anything asynchronously on Fluent Search startup
            return ValueTask.CompletedTask;
        }

        public SearchApplicationInfo GetApplicationInfo()
        {
            return _applicationInfo;
        }

        public async IAsyncEnumerable<ISearchResult> SearchAsync(SearchRequest searchRequest,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested || searchRequest.SearchType == SearchType.SearchProcess)
                yield break;
            string searchedTag = searchRequest.SearchedTag;
            string searchedText = searchRequest.SearchedText;
            
            // Check that the search tag is something this app can handle and the searched text is a number
            if (string.IsNullOrWhiteSpace(searchedTag) || !int.TryParse(searchedText, out int number) ||
                !Enum.TryParse(searchedTag, true, out ConversionType conversionType))
            {
                yield break;
            }
            
            var convertedNumber = conversionType switch
            {
                ConversionType.Hex => number.ToString("X"),
                ConversionType.Binary => Convert.ToString(number, 2),
                _ => throw new ArgumentOutOfRangeException()
            };

            yield return new NumberConversionSearchResult(number, SearchAppName, convertedNumber,
                $"{conversionType} {searchedText} is {convertedNumber}", searchedText, conversionType.ToString(), 2,
                _supportedOperations, _searchTags);
        }

        public ValueTask<ISearchResult> GetSearchResultForId(string serializedSearchObjectId)
        {
            // This is used to calculate a search result after Fluent Search has been restarted
            // This is only used by the custom search tag feature
            return new();
        }

        public ValueTask<IHandleResult> HandleSearchResult(ISearchResult searchResult)
        {
            return new(new HandleResult(true, false));
        }
    }
}