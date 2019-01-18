using System;

namespace ProjectAnswerSearch
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Net;
    using System.IO;
    using MovieMarvel;

    namespace Answers_csharp
    {
        class Program
        {
            // Replace the accessKey string value with your valid access key.
            const string accessKey = "eb9b089541dc4cb28f2404a8216c82c6";

            const string uriBase = "https://api.labs.cognitive.microsoft.com/answerSearch/v7.0/search";

            const string searchTerm = "what is the third law of calculus";

            // Used to return news search results including relevant headers
            struct SearchResult
            {
                public String jsonResult;
                public Dictionary<String, String> relevantHeaders;
            }

            static void Main()
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                Console.WriteLine("Searching locally for: " + searchTerm);

                SearchResult result = BingLocalSearch(searchTerm);

                Console.WriteLine("\nRelevant HTTP Headers:\n");
                foreach (var header in result.relevantHeaders)
                    Console.WriteLine(header.Key + ": " + header.Value);

                Console.WriteLine("\nJSON Response:\n");
                Console.WriteLine(JsonPrettyPrint(result.jsonResult));

                Console.Write("\nPress Enter to exit ");
                Console.ReadLine();
            }

            /// <summary>
            /// Does Bing knowledge search and returns the results as a SearchResult.
            /// </summary>
            static SearchResult BingLocalSearch(string searchQuery)
            {
                // Construct the URI of the search request
                var uriQuery = uriBase + "?q=" + Uri.EscapeDataString(searchQuery) + "&mkt=en-us";

                // Send the Web request and get the response.
                WebRequest request = HttpWebRequest.Create(uriQuery);
                request.Headers["Ocp-Apim-Subscription-Key"] = accessKey;

                HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
                string json = new StreamReader(response.GetResponseStream()).ReadToEnd();

                // Create result object for return
                var searchResult = new SearchResult();
                searchResult.jsonResult = json;
                searchResult.relevantHeaders = new Dictionary<String, String>();

                // Extract Bing HTTP headers
                foreach (String header in response.Headers)
                {
                    if (header.StartsWith("BingAPIs-") || header.StartsWith("X-MSEdge-"))
                        searchResult.relevantHeaders[header] = response.Headers[header];
                }

                return searchResult;
            }

            /// <summary>
            /// Formats the given JSON string by adding line breaks and indents.
            /// </summary>
            /// <param name="json">The raw JSON string to format.</param>
            /// <returns>The formatted JSON string.</returns>
            static string JsonPrettyPrint(string json)
            {
                if (string.IsNullOrEmpty(json))
                    return string.Empty;

                json = json.Replace(Environment.NewLine, "").Replace("\t", "");

                StringBuilder sb = new StringBuilder();
                bool quote = false;
                bool ignore = false;
                int offset = 0;
                int indentLength = 3;

                foreach (char ch in json)
                {
                    switch (ch)
                    {
                        case '"':
                            if (!ignore) quote = !quote;
                            break;
                        case '\'':
                            if (quote) ignore = !ignore;
                            break;
                    }

                    if (quote)
                        sb.Append(ch);
                    else
                    {
                        switch (ch)
                        {
                            case '{':
                            case '[':
                                sb.Append(ch);
                                sb.Append(Environment.NewLine);
                                sb.Append(new string(' ', ++offset * indentLength));
                                break;
                            case '}':
                            case ']':
                                sb.Append(Environment.NewLine);
                                sb.Append(new string(' ', --offset * indentLength));
                                sb.Append(ch);
                                break;
                            case ',':
                                sb.Append(ch);
                                sb.Append(Environment.NewLine);
                                sb.Append(new string(' ', offset * indentLength));
                                break;
                            case ':':
                                sb.Append(ch);
                                sb.Append(' ');
                                break;
                            default:
                                if (ch != ' ') sb.Append(ch);
                                break;
                        }
                    }
                }

                return sb.ToString().Trim();
            }

        }
    }
}
