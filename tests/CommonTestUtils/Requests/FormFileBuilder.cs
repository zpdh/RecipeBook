using Microsoft.AspNetCore.Http;

namespace CommonTestUtils.Requests;

public class FormFileBuilder
{
    public static IList<IFormFile> CreateCollection()
    {
        return
        [
            Png(),
            Jpg()
        ];
    }

    private static FormFile Png()
    {
        var stream = File.OpenRead("Files/image.png");

        var file = new FormFile(
            baseStream: stream,
            baseStreamOffset: 0,
            length: stream.Length,
            name: "File",
            fileName: "file.png")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };

        return file;
    }

    private static FormFile Jpg()
    {
        var stream = File.OpenRead("Files/image.jpg");

        var file = new FormFile(
            baseStream: stream,
            baseStreamOffset: 0,
            length: stream.Length,
            name: "File",
            fileName: "file.jpg")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpg"
        };

        return file;
    }

    public static FormFile Txt()
    {
        var stream = File.OpenRead("Files/notimage.txt");

        var file = new FormFile(
            baseStream: stream,
            baseStreamOffset: 0,
            length: stream.Length,
            name: "File",
            fileName: "file.txt")
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/plain"
        };

        return file;
    }
}