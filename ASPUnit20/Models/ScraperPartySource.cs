using System;
using System.Net.Http;
using System.Diagnostics;
using HtmlAgilityPack;
using ASPUnit20.Models;

namespace ASPUnit20.Models
{
    internal class ScraperPartySource : IPartySource
    {
        private readonly HttpClient _client = new HttpClient();

        public string GetParty(string memberId)
        {
            string url = "https://www.theyworkforyou.com/mp/" + memberId;

            var reqTask = _client.GetAsync(url);
            reqTask.Wait();
            if (!reqTask.Result.IsSuccessStatusCode)
            {
                Debug.WriteLine("Failed to send GET request to " + url);
                return "Error retrieving data";
            }

            var htmlTask = reqTask.Result.Content.ReadAsStringAsync();
            htmlTask.Wait();
            var html = htmlTask.Result;
            if (string.IsNullOrWhiteSpace(html))
            {
                Debug.WriteLine("HTML is empty");
                return "No HTML retrieved";
            }

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var partyNode = htmlDoc.DocumentNode.SelectSingleNode("//span[contains(@class, 'person-header__about__position__role')]");
            if (partyNode != null)
            {
                string partyAffiliation = partyNode.InnerText.Trim();

                if (partyAffiliation.EndsWith("MP", StringComparison.OrdinalIgnoreCase))
                {
                    partyAffiliation = partyAffiliation.Substring(0, partyAffiliation.Length - 2).Trim();
                }
                return partyAffiliation;
            }
            return "Party affiliation not found";
        }
    }
}