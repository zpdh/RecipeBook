using Bogus;

namespace CommonTestUtils.Requests;

public class StringGeneratorRequest
{
    public static string LongString(int minCharacters)
    {
        var faker = new Faker();

        var text = faker.Lorem.Paragraphs();

        while (text.Length < minCharacters)
        {
            text += faker.Lorem.Paragraphs();
        }

        return text;
    }
}