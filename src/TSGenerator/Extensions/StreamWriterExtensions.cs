using System.IO;

namespace TSGenerator.Extensions
{
    public static class StreamWriterExtensions
    {
        public static void WriteIdent(this StreamWriter streamWriter, int ident, string data)
            => streamWriter.WriteLine($"{new string(' ', ident)}{data}");
    }
}
