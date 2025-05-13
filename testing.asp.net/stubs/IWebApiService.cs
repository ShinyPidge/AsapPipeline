public interface IWebApiService
{
    string GetDonationData();
}

public class WebApiStub : IWebApiService
{
    public string GetDonationData()
    {
        // Simulating API response with fixed test data
        return @"<Donations>
                    <Donation Name='John Doe' Party='Independent' Donor='Jane Smith' Amount='100.50' />
                    <Donation Name='Alice Johnson' Party='Green Party' Donor='Bob Brown' Amount='250.75' />
                 </Donations>";
    }
}