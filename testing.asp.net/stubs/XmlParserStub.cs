using ASPUnit20.Models;

public interface IXmlParser
{
    List<Donation> Parse(string xmlData);
}

public class XmlParserStub : IXmlParser
{
    public List<Donation> Parse(string xmlData)
    {
        // Simulating parsed donation list without real XML processing
        return new List<Donation>
        {
            new Donation("John Doe", "Independent", "Jane Smith", 100.50f),
            new Donation("Alice Johnson", "Green Party", "Bob Brown", 250.75f)
        };
    }
}