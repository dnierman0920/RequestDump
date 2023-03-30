namespace RequestDump;

public class UnitTest1
{
    [Fact]
    public void Test2()
    {
        Uri oldBaseApiUrl = new("https://rickandmortyapi.com/api/character/");
        ApiReference oldApiReference = new(true, oldBaseApiUrl);

        IEnumerable<string> productIds = new string[] { "1", "2", "3" };
        string responsesFolderPath = "../../../Responses";
        RequestDump.writeResponsesToFile(oldApiReference, productIds, responsesFolderPath);
    }

}