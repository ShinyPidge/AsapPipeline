using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using ASPUnit20.Models;
using ASPUnit20.Models.Services;
public class DonationsController : Controller
{
    private readonly IXmlParser _xmlParser;
    public DonationsController()
    {
        _xmlParser = new XmlParser(new PartySourceFactory().Create());
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new ModelResults());
    }
    [HttpPost]
    public IActionResult Index(IFormFile file)
    {
        ModelResults result = new ModelResults();
        if (file == null || file.Length == 0)
        {
            // if no file is uploaded, display an error message
            ViewBag.ErrorMessage = "Please upload a valid XML file.";
            return View(result);
        }

        try
        {
            result.Data = this._xmlParser.Parse(new StreamReader(file.OpenReadStream()).ReadToEnd());
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"Error parsing the file: {ex.Message}";
            return View(result);
        }
        return View(result);
    }
}