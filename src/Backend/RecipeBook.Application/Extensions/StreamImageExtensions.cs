using FileTypeChecker.Extensions;
using FileTypeChecker.Types;

namespace RecipeBook.Application.Extensions;

public static class StreamImageExtensions
{
    public static (bool isValid, string extension) ValidateAndGetImageExtension(this Stream stream)
    {
        // Validates stream and checks whether it's a PNG or a JPEG file.
        // If it is, returns true and the file extension to store it
        // in the cloud.
        
        var result = (false, string.Empty);

        if (stream.Is<PortableNetworkGraphic>())
        {
            result = (true, Normalize(PortableNetworkGraphic.TypeExtension));
        }
        else if (stream.Is<JointPhotographicExpertsGroup>())
        {
            result = (true, Normalize(JointPhotographicExpertsGroup.TypeExtension));
        }

        stream.Position = 0;

        return result;
    }

    private static string Normalize(string extension)
    {
        // This method just adds a dot at the start of the extension.
        // Why? In case the library decides they want to add a dot
        // To the end of the TypeExtension const, the program won't break.

        return extension.StartsWith('.')
            ? extension
            : $".{extension}";
    }
}