using Microsoft.VisualStudio.TestTools.UnitTesting;
using ASPUnit20.Models;
using System;
using System.Xml.Linq;
using System.Globalization;
using System.Diagnostics;

namespace testing.asp.net
{
    [TestClass]
    public class DonationTests
    {
        [TestMethod]
        public void Donation_Constructor_ShouldInitializeCorrectly()
        {
            string expectedName = "John Doe";
            string expectedParty = "Independent";
            string expectedDonor = "Jane Smith";
            float expectedAmount = 100.50f;

            var donation = new Donation(expectedName, expectedParty, expectedDonor, expectedAmount);

            Assert.AreEqual(expectedName, donation.Name);
            Assert.AreEqual(expectedParty, donation.Party);
            Assert.AreEqual(expectedDonor, donation.Donor);
            Assert.AreEqual(expectedAmount, donation.Amount);
        }

        [TestMethod]
        public void SerializeDonationData_ValidXml()
        {
            var donations = new List<Donation>
    {
        new Donation("John Doe", "Independent", "Jane Smith", 100.50f),
        new Donation("Alice Johnson", "Green Party", "Bob Brown", 250.75f)
    };

            var xmlDoc = new XDocument(new XElement("Donations",
                donations.Select(d => new XElement("Donation",
                    new XAttribute("Name", d.Name),
                    new XAttribute("Party", d.Party),
                    new XAttribute("Donor", d.Donor),
                    new XAttribute("Amount", d.Amount)))));

            string xmlOutput = xmlDoc.ToString();

            Assert.IsTrue(xmlOutput.Contains("John Doe"));
            Assert.IsTrue(xmlOutput.Contains("Independent"));
        }

        [TestMethod]
        public void ParseDonationXml_rightdata()
            {
             
            string xml = @"<Donations>
                       <Donation Name='John Doe' Party='Independent' Donor='Jane Smith' Amount='100.50' />
                       <Donation Name='Alice Johnson' Party='Green Party' Donor='Bob Brown' Amount='250.75' />
                           </Donations>";

                var xDocument = XDocument.Parse(xml);
                List<Donation> parsedDonations = xDocument.Root.Elements("Donation")
                    .Select(d => new Donation(
                        name: d.Attribute("Name")?.Value ?? "Unknown",
                        party: d.Attribute("Party")?.Value ?? "Unknown",
                        donor: d.Attribute("Donor")?.Value ?? "Unknown",
                        amount: float.Parse(d.Attribute("Amount")?.Value ?? "0", CultureInfo.InvariantCulture)
                    )).ToList();

                Assert.AreEqual(2, parsedDonations.Count);
                Assert.AreEqual("John Doe", parsedDonations[0].Name);
                Assert.AreEqual(100.50f, parsedDonations[0].Amount);
        }
        [TestMethod]
        public void ParseDonationXml_MissingAttributes()
        {
            string xml = @"<Donations>
                        <Donation Name='John Doe' Amount='100.50' />
                   </Donations>";
            var xDocument = XDocument.Parse(xml);

            var parsedDonations = xDocument.Root.Elements("Donation")
                .Select(d => new Donation(
                    name: d.Attribute("Name")?.Value ?? "Unknown",
                    party: d.Attribute("Party")?.Value ?? "Unknown",
                    donor: d.Attribute("Donor")?.Value ?? "Unknown",
                    amount: float.Parse(d.Attribute("Amount")?.Value ?? "0", CultureInfo.InvariantCulture)
                )).ToList();

            Assert.AreEqual(1, parsedDonations.Count);
            Assert.AreEqual("John Doe", parsedDonations[0].Name);
            Assert.AreEqual("Unknown", parsedDonations[0].Party, "Missing Party attribute should default to 'Unknown'.");
            Assert.AreEqual("Unknown", parsedDonations[0].Donor, "Missing Donor attribute should default to 'Unknown'.");
            Assert.AreEqual(100.50f, parsedDonations[0].Amount);
        }
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ParseDonationXml_InvalidAmount()
        {
            
            string xml = @"<Donations>
                        <Donation Name='John Doe' Party='Independent' Donor='Jane Smith' Amount='NotANumber' />
                   </Donations>";
            var xDocument = XDocument.Parse(xml);

            var parsedDonations = xDocument.Root.Elements("Donation")
                .Select(d => new Donation(
                    name: d.Attribute("Name")?.Value ?? "Unknown",
                    party: d.Attribute("Party")?.Value ?? "Unknown",
                    donor: d.Attribute("Donor")?.Value ?? "Unknown",
                    amount: float.Parse(d.Attribute("Amount")?.Value, CultureInfo.InvariantCulture)
                )).ToList();
        }
        [TestMethod]
        public void ParseDonationXml_MoreWhiteSpace()
        {
            string xml = @"<Donations>
                        <Donation Name='  John Doe  ' Party='  Independent  ' Donor='  Jane Smith  ' Amount=' 100.50 ' />
                   </Donations>";
            var xDocument = XDocument.Parse(xml);
            
            var parsedDonations = xDocument.Root.Elements("Donation")
                .Select(d => new Donation(
                    name: d.Attribute("Name")?.Value.Trim() ?? "Unknown",
                    party: d.Attribute("Party")?.Value.Trim() ?? "Unknown",
                    donor: d.Attribute("Donor")?.Value.Trim() ?? "Unknown",
                    amount: float.Parse(d.Attribute("Amount")?.Value.Trim() ?? "0", CultureInfo.InvariantCulture)
                )).ToList();

            Assert.AreEqual(1, parsedDonations.Count);
            Assert.AreEqual("John Doe", parsedDonations[0].Name);
            Assert.AreEqual("Independent", parsedDonations[0].Party);
            Assert.AreEqual("Jane Smith", parsedDonations[0].Donor);
            Assert.AreEqual(100.50f, parsedDonations[0].Amount);
        }
        [TestMethod]
        public void XmlParser_LoadWithoutErrors()
        {
            // Arrange
            string xml = @"<Donations>
                       <Donation Name='John Doe' Party='Independent' Donor='Jane Smith' Amount='100.50' />
                   </Donations>";

            var xDocument = XDocument.Parse(xml);

            Assert.IsNotNull(xDocument, "Failed to load XML document.");
            Assert.IsNotNull(xDocument.Root, "Root element is missing.");
            Assert.AreEqual("Donations", xDocument.Root.Name.LocalName, "Root element name does not match expected value.");
        }

            [TestMethod]
            public void ParsedDonationInfo_ShouldBeFormattedCorrectly()
            {
                string xml = @"<Donations>
                                <Donation Name='  John Doe  ' Party='  Independent  ' Donor='  Jane Smith  ' Amount='  100.50  ' />
                           </Donations>";

                var xDocument = XDocument.Parse(xml);
                List<Donation> donations = xDocument.Root.Elements("Donation")
                    .Select(d => new Donation(
                        name: d.Attribute("Name")?.Value.Trim() ?? "Unknown",
                        party: d.Attribute("Party")?.Value.Trim() ?? "Unknown",
                        donor: d.Attribute("Donor")?.Value.Trim() ?? "Unknown",
                        amount: float.Parse(d.Attribute("Amount")?.Value.Trim() ?? "0", CultureInfo.InvariantCulture)
                    ))
                    .ToList();

                Assert.AreEqual(1, donations.Count, "Expected one donation to be parsed.");

                Donation donation = donations[0];
                Assert.AreEqual("John Doe", donation.Name, "The donation name was not formatted correctly.");
                Assert.AreEqual("Independent", donation.Party, "The donation party was not formatted correctly.");
                Assert.AreEqual("Jane Smith", donation.Donor, "The donation donor was not formatted correctly.");

                string formattedAmount = donation.Amount.ToString("F2", CultureInfo.InvariantCulture);
                Assert.AreEqual("100.50", formattedAmount, "The donation amount was not formatted to two decimals.");
            }

            [TestMethod]
            public void ParseDonationXml_EmptyFile()
            {
                string xml = "<Donations></Donations>";

                var xDocument = XDocument.Parse(xml);
                List<Donation> donations = xDocument.Root.Elements("Donation")
                    .Select(d => new Donation(
                        name: d.Attribute("Name")?.Value.Trim() ?? "Unknown",
                        party: d.Attribute("Party")?.Value.Trim() ?? "Unknown",
                        donor: d.Attribute("Donor")?.Value.Trim() ?? "Unknown",
                        amount: float.Parse(d.Attribute("Amount")?.Value.Trim() ?? "0", CultureInfo.InvariantCulture)
                    ))
                    .ToList();
                Assert.AreEqual(0, donations.Count, "Expected an empty donation list for an empty XML file.");
            }
        public class XmlParser : IXmlParser
        {
            public IPartySource _partySource;

            public XmlParser(IPartySource partySource)
            {
                _partySource = partySource;
            }

            public List<Donation> Parse(string filePath)
            {
                var donations = new List<Donation>();
                var twfy = XDocument.Load(filePath).Root;

                foreach (var regmem in twfy.Elements("regmem"))
                {
                    string memberName = regmem.Attribute("membername").Value;
                    // Assuming the personid attribute contains a value like "1234", or a URL ending in "/1234"
                    string id = regmem.Attribute("personid").Value.Split('/').Last();

                    // Use the party source to get the party string
                    string memberparty = _partySource.GetParty(id);

                    foreach (var category in regmem.Elements("category"))
                    {
                        foreach (var record in category.Elements("record"))
                        {
                            foreach (var item in record.Elements("item"))
                            {
                                foreach (var div in item.Elements("div"))
                                {
                                    this.Deatils(donations, memberName, memberparty, "Unkown", div);
                                }
                            }
                        }
                    }
                }

                return donations;
            }


            public void Deatils(List<Donation> donations, string membername, string memberparty, string potentialDonor, XElement detailsDiv)
            {
                string donor = potentialDonor;
                float amount = float.NegativeInfinity;
                foreach (var ul in detailsDiv.Elements("ul"))
                {
                    foreach (var li in ul.Elements("li"))
                    {
                        string name = "";
                        string value = "";
                        foreach (var span in li.Elements("span"))
                        {
                            if (span.Attribute("class").Value.Equals("interest-detail-name"))
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
                            Debug.Assert(float.TryParse(value, out amount), "Inaccessible case, value not number?");
                        }
                    }
                }
                foreach (var child in detailsDiv.Elements("div"))
                {
                    if (child.Attribute("class").Value != "interest-child-items") continue;
                    this.Deatils(donations, membername, memberparty, donor, child);
                }
                if (donor != "" && !float.IsNegativeInfinity(amount))
                {
                    donations.Add(new Donation(membername, memberparty, donor, amount));
                }
            }
        }
        [TestMethod]
        public void WebApiStub_ShouldReturnFixedTestData()
        {
            IWebApiService apiStub = new WebApiStub();
            string response = apiStub.GetDonationData();

            Assert.IsTrue(response.Contains("<Donations>"), "API stub did not return expected XML format.");
        }
        [TestMethod]
        public void XmlParserStub_ShouldReturnFixedDonationList()
        {
            IXmlParser parserStub = new XmlParserStub();
            List<Donation> donations = parserStub.Parse("<FakeXml>");

            Assert.AreEqual(2, donations.Count, "Parser stub did not return expected number of donations.");
            Assert.AreEqual("John Doe", donations[0].Name, "Parsed donation data is incorrect.");
        }
        [TestMethod]
        public void HtmlFormatterStub_ShouldReturnExpectedHtml()
        {
            IHtmlFormatter formatterStub = new HtmlFormatterStub();
            string htmlOutput = formatterStub.FormatDonations(new List<Donation>());

            Assert.IsTrue(htmlOutput.Contains("<html>"), "HTML stub did not return expected format.");
        }   
    }
}

