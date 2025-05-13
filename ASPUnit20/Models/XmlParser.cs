using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;
using ASPUnit20.Models;
using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;

namespace ASPUnit20.Models
{
    public class XmlParser : IXmlParser
    {
        private readonly IPartySource _partySource;

        public XmlParser(IPartySource partySource)
        {
            _partySource = partySource;
        }

        public List<Donation> Parse(string filePath)
        {
            var donations = new List<Donation>();
            var twfy = XDocument.Load(new StringReader(filePath)).Root; // Only the Godess can forgive my sins here

            foreach (var regmem in twfy.Elements("regmem"))
            {
                string memberName = regmem.Attribute("membername")?.Value ?? "Unknown";
                string id = regmem.Attribute("personid")?.Value.Split('/').Last() ?? "0";
                string memberParty = _partySource.GetParty(id);

                foreach (var category in regmem.Elements("category"))
                {
                    foreach (var record in category.Elements("record"))
                    {
                        foreach (var item in record.Elements("item"))
                        {
                            foreach (var div in item.Elements("div"))
                            {
                                Details(donations, memberName, memberParty, "Unknown", div);
                            }
                        }
                    }
                }
            }

            return donations;
        }

        public void Details(List<Donation> donations, string memberName, string memberParty, string potentialDonor, XElement detailsDiv)
        {
            string donor = potentialDonor;
            float amount = float.NaN;

            foreach (var ul in detailsDiv.Elements("ul"))
            {
                foreach (var li in ul.Elements("li"))
                {
                    string name = "";
                    string value = "";

                    foreach (var span in li.Elements("span"))
                    {
                        string className = span.Attribute("class")?.Value ?? "";
                        if (className.Equals("interest-detail-name"))
                        {
                            name = span.Value;
                        }
                        else
                        {
                            value = span.Value;
                        }
                    }

                    if (name == "Payer Name: " || name == "Donor Name: ")
                    {
                        donor = value;
                    }

                    if (name == "Value: ")
                    {
                        if (float.TryParse(value, out float parsedAmount))
                        {
                            amount = parsedAmount;
                        }
                    }
                }
            }

            foreach (var child in detailsDiv.Elements("div"))
            {
                if (child.Attribute("class")?.Value != "interest-child-items") continue;
                var wrapper = child?.Element("div");
                if (wrapper == null) continue;
                Details(donations, memberName, memberParty, donor, wrapper);
            }

            if (!string.IsNullOrWhiteSpace(donor) && !float.IsNaN(amount))
            {
                donations.Add(new Donation(memberName, memberParty, donor, amount));
            }
        }
    }
}