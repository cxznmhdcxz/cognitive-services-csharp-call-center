﻿// 
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

using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CallCenterSample.Helpers
{
    public class TextAnalyticsHelper
    {
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

        private static void InitializeTextAnalyticsClient()
        {
            if (!string.IsNullOrEmpty(ApiKey) && !string.IsNullOrEmpty(EndPoint))
            {
                AnalyticsClient = new TextAnalyticsClient(
                    new Uri(EndPoint),
                    new AzureKeyCredential(ApiKey));
            }
        }

        public static async Task<DetectLanguageResult> GetDetectedLanguageAsync(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                DetectLanguageResultCollection result = await AnalyticsClient.DetectLanguageBatchAsync(
                new List<string>() { input });

                if (!result[0].HasError)
                {
                    return result[0];
                }   
            }

            return null;
        }

        public static async Task<DocumentSentiment> GetTextSentimentAsync(string input, string language = "en")
        {
            if (!string.IsNullOrEmpty(input))
            {
                AnalyzeSentimentResultCollection result = await AnalyticsClient.AnalyzeSentimentBatchAsync(
                    new List<string>() { input }, language);

                if (!result[0].HasError)
                {
                    return result[0].DocumentSentiment;
                }
            }

            return null;
        }

        public static async Task<ExtractKeyPhrasesResult> GetKeyPhrasesAsync(string input, string language = "en")
        {
            if (!string.IsNullOrEmpty(input))
            {
                ExtractKeyPhrasesResultCollection result = await AnalyticsClient.ExtractKeyPhrasesBatchAsync(
                    new List<string>() { input }, language);

                if (!result[0].HasError)
                {
                    return result[0];
                }
            }

            return null;
        }
    }
}
