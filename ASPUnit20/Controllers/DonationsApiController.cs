using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using ASPUnit20.Models;
using ASPUnit20.Models.Services;
using System.Collections.Generic;

namespace ASPUnit20.Controllers
{
    [ApiController]
    [Route("api/donations")]
    public class DonationsApiController : ControllerBase
    {
        private readonly IXmlParser _xmlParser;
        public DonationsApiController()
        {
            _xmlParser = new XmlParser(new PartySourceFactory().Create());
        }

        // Existing endpoint for parsing donation data from XML
        [HttpPost]
        public IActionResult PostDonations([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Please upload a valid XML file.");
            }

            try
            {
                var content = new StreamReader(file.OpenReadStream()).ReadToEnd();
                var donationData = _xmlParser.Parse(content);
                return Ok(donationData);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error parsing the file: {ex.Message}");
            }
        }
        [HttpPost("summary")]
        public IActionResult GetDonationSummary([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Please upload a valid XML file.");
            }

            try
            {
                // Read and parse the XML file content.
                var content = new StreamReader(file.OpenReadStream()).ReadToEnd();
                List<Donation> donationList = _xmlParser.Parse(content);

                // Calculate summary metrics.
                var totalDonations = donationList.Sum(d => d.Amount);
                var averageDonation = donationList.Any() ? donationList.Average(d => d.Amount) : 0;
                var donationCount = donationList.Count;

                // Group donations by party (if available) for extra insight.
                var donationsByParty = donationList
                                        .GroupBy(d => d.Party)
                                        .Select(g => new { Party = g.Key, TotalAmount = g.Sum(d => d.Amount), Count = g.Count() })
                                        .ToList();

                // Return the summarized data as JSON.
                return Ok(new
                {
                    TotalDonations = totalDonations,
                    AverageDonation = averageDonation,
                    DonationCount = donationCount,
                    DonationsByParty = donationsByParty
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error processing the file: {ex.Message}");
            }
        }
    }
}