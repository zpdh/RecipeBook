using Sqids;

namespace CommonTestUtils.Cryptography;

public class IdEncoderBuilder
{
    public static SqidsEncoder<long> Build()
    {
        return new SqidsEncoder<long>(new SqidsOptions
        {
            MinLength = 10,
            Alphabet = "ioNL4sHwSfRVp89W5PqIjaEeGmO6btdDMczvC7kB3Uyu2rhFQTX01ZJgnlKAYx"
        });
    }
}