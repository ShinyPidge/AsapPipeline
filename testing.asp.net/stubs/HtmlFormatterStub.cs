using ASPUnit20.Models;

public interface IHtmlFormatter
{
    string FormatDonations(List<Donation> donations);
}

public class HtmlFormatterStub : IHtmlFormatter
{
    public string FormatDonations(List<Donation> donations)
    {
        // Simulated HTML output
        return "<html><body><h1>Donations</h1><ul><li>John Doe - $100.50</li><li>Alice Johnson - $250.75</li></ul></body></html>";
    }
}