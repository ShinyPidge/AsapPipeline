namespace ASPUnit20.Models
{
    public interface IXmlParser
    {
        List<Donation> Parse(string filePath);
    }
}