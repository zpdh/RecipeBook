using System.Collections;
using CommonTestUtils.Requests;

namespace UseCases.Tests.Recipe.InlineData;

public class ImageTypesInlineData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        var images = FormFileBuilder.CreateCollection();

        foreach (var image in images)
        {
            yield return [image];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}