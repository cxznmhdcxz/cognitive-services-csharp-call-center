// 
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
// 
// Microsoft Cognitive Services: http://www.microsoft.com/cognitive
// 
// Microsoft Cognitive Services Github:
// https://github.com/Microsoft/Cognitive
// 
// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Rest;
using Azure.AI.TextAnalytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Azure;

namespace CallCenterSample.Helpers
{
    public class TextAnalyticsHelper
    {
        //private static ITextAnalyticsClient AnalyticsClient { get; set; }
        private static Azure.AI.TextAnalytics.TextAnalyticsClient AnalyticsClient { get; set; }

        private static string apiKey;

        public static string ApiKey
        {
            get { return apiKey; }
            set
            {
                var changed = apiKey != value;
                apiKey = value;
                if (changed)
                {
                    InitializeTextAnalyticsClient();
                }
            }
        }

        private static string apiKeyRegion;
        public static string ApiKeyRegion
        {
            get { return apiKeyRegion; }
            set
            {
                var changed = apiKeyRegion != value;
                apiKeyRegion = value;
                if (changed)
                {
                    InitializeTextAnalyticsClient();
                }
            }
        }

        private static string endPoint;
        public static string EndPoint
        {
            get { return endPoint; }
            set
            {
                var changed = endPoint != value;
                endPoint = value;
                if (changed)
                {
                    InitializeTextAnalyticsClient();
                }
            }
        }

        //private static void InitializeTextAnalyticsClient()
        //{
        //    if (!string.IsNullOrEmpty(ApiKey) && !string.IsNullOrEmpty(ApiKeyRegion))
        //    {
        //        AnalyticsClient = new Microsoft.Azure.CognitiveServices.Language.TextAnalytics.TextAnalyticsClient(new ApiKeyServiceClientCredentials())
        //        {
        //            Endpoint = string.Format("https://{0}.api.cognitive.microsoft.com", ApiKeyRegion)
        //        };
        //    }
        //}

        private static void InitializeTextAnalyticsClient()
        {
            if (!string.IsNullOrEmpty(ApiKey) && !string.IsNullOrEmpty(EndPoint) && !EndPoint.Contains("west") &&!EndPoint.Contains("asia"))
            {
                AnalyticsClient = new Azure.AI.TextAnalytics.TextAnalyticsClient(
                    new Uri(EndPoint),
                    new AzureKeyCredential(ApiKey));
            }
        }

        /// <summary>
        /// Container for subscription credentials
        /// </summary>
        class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Add("Ocp-Apim-Subscription-Key", ApiKey);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                return base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }

        public static async Task<DetectLanguageResult> GetDetectedLanguageAsync(string input)
        {
            DetectLanguageResult languageResult = new DetectLanguageResult() { Language = new Dictionary<string, string>() };

            if (!string.IsNullOrEmpty(input))
            {
                DetectLanguageResultCollection result = await AnalyticsClient.DetectLanguageBatchAsync(
                new List<string>() { input });

                if (!result[0].HasError)
                {
                    languageResult.Language.Add("iso6391Name", result[0].PrimaryLanguage.Iso6391Name);
                    languageResult.Language.Add("name", result[0].PrimaryLanguage.Name);
                    languageResult.Language.Add("score", result[0].PrimaryLanguage.ConfidenceScore.ToString());
                }   
            }

            return languageResult;
        }

        //public static async Task<DetectLanguageResult> GetDetectedLanguageAsync(string input)
        //{
        //    DetectLanguageResult languageResult = new DetectLanguageResult() { Language = new Dictionary<string, string>() };

        //    if (!string.IsNullOrEmpty(input))
        //    {
        //        LanguageBatchResult result = await AnalyticsClient.DetectLanguageAsync(new BatchInput(
        //            new List<Input>()
        //            {
        //                new Input("0", input)
        //            }));

        //        if (result.Documents != null)
        //        {
        //            languageResult.Language.Add("iso6391Name", result.Documents[0].DetectedLanguages[0].Iso6391Name);
        //            languageResult.Language.Add("name", result.Documents[0].DetectedLanguages[0].Name);
        //            languageResult.Language.Add("score", result.Documents[0].DetectedLanguages[0].Score.ToString());
        //        }

        //        if (result.Errors != null)
        //        {
        //            // Just return the empty Dictionary
        //        }
        //    }

        //    return languageResult;
        //}

        public static async Task<SentimentResult> GetTextSentimentAsync(string input, string language = "en")
        {
            SentimentResult sentimentResult = new SentimentResult()
            {
                Sentiment = "null",
                Negative = 0.5,
                Neutral = 0.5,
                Positive = 0.5
            };

            if (!string.IsNullOrEmpty(input))
            {
                AnalyzeSentimentResultCollection result = await AnalyticsClient.AnalyzeSentimentBatchAsync(
                    new List<string>() { input }, language);

                if (!result[0].HasError)
                {
                    sentimentResult.Sentiment = result[0].DocumentSentiment.Sentiment.ToString();
                    sentimentResult.Negative = (double)result[0].DocumentSentiment.ConfidenceScores.Negative;
                    sentimentResult.Neutral = (double)result[0].DocumentSentiment.ConfidenceScores.Neutral;
                    sentimentResult.Positive = (double)result[0].DocumentSentiment.ConfidenceScores.Positive;
                }
            }

            return sentimentResult;
        }

        //public static async Task<SentimentResult> GetTextSentimentAsync(string input, string language = "en")
        //{
        //    SentimentResult sentimentResult = new SentimentResult() { Negative = 0, Positive = 0, Neutral = 0 };

        //    if (!string.IsNullOrEmpty(input))
        //    {
        //        SentimentBatchResult result = await AnalyticsClient.SentimentAsync(new MultiLanguageBatchInput(
        //            new List<MultiLanguageInput>()
        //            {
        //                new MultiLanguageInput(language, "0", input)
        //            }));

        //        if (result.Documents != null)
        //        {
        //            sentimentResult.Score = (double) result.Documents[0].Score;
        //        }

        //        if (result.Errors != null)
        //        {
        //            // Just return the neutral value
        //        }
        //    }

        //    return sentimentResult;
        //}

        public static async Task<KeyPhrasesResult> GetKeyPhrasesAsync(string input, string language = "en")
        {
            KeyPhrasesResult keyPhrasesResult = new KeyPhrasesResult() { KeyPhrases = Enumerable.Empty<string>() };

            if (!string.IsNullOrEmpty(input))
            {
                ExtractKeyPhrasesResultCollection result = await AnalyticsClient.ExtractKeyPhrasesBatchAsync(
                    new List<string>() { input }, language);

                if (!result[0].HasError)
                {
                    List<string> phrases = new List<string>();
                    foreach (string keyPhrase in result[0].KeyPhrases)
                    {
                        phrases.Add(keyPhrase);
                    }

                    keyPhrasesResult.KeyPhrases = phrases;
                }
            }

            return keyPhrasesResult;
        }

        //public static async Task<KeyPhrasesResult> GetKeyPhrasesAsync(string input, string language = "en")
        //{
        //    KeyPhrasesResult keyPhrasesResult = new KeyPhrasesResult() { KeyPhrases = Enumerable.Empty<string>() };

        //    if (!string.IsNullOrEmpty(input))
        //    {
        //        KeyPhraseBatchResult result = await AnalyticsClient.KeyPhrasesAsync(new MultiLanguageBatchInput(
        //            new List<MultiLanguageInput>()
        //            {
        //                new MultiLanguageInput(language, "0", input)
        //            }));

        //        if (result.Documents != null)
        //        {
        //            List<string> phrases = new List<string>();

        //            foreach (string keyPhrase in result.Documents[0].KeyPhrases)
        //            {
        //                phrases.Add(keyPhrase);
        //            }

        //            keyPhrasesResult.KeyPhrases = phrases;
        //        }

        //        if (result.Errors != null)
        //        {
        //            // Just return the empty IEnumerable
        //        }
        //    }

        //    return keyPhrasesResult;
        //}
    }

    public class SentimentResult
    {
        public string Sentiment { get; set; }
        public double Negative { get; set; }
        public double Neutral { get; set; }
        public double Positive { get; set; }
    }

    public class DetectLanguageResult
    {
        public Dictionary<string, string> Language { get; set; }
    }

    public class KeyPhrasesResult
    {
        public IEnumerable<string> KeyPhrases { get; set; }
    }

}
