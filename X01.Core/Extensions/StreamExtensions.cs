using System.Text;

namespace X01.Core.Extensions;

public static class StreamExtensions
{
    public static byte[] ToArray(this Stream stream)
    {
        if (stream is MemoryStream memoryStream)
        {
            return memoryStream.ToArray();
        }

        using (memoryStream = new MemoryStream())
        {
            stream.CopyTo(memoryStream);
            memoryStream.Position = 0;
            return memoryStream.ToArray();
        }
    }

    public static async Task<string> ReadToEndAsync(this Stream stream)
    {
        using (var streamReader = new StreamReader(stream, Encoding.Default, false, 1024, true))
        {
            return await streamReader.ReadToEndAsync();
        }
    }
}