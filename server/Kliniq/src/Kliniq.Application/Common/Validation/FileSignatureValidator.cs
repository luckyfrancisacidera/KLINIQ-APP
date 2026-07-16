namespace Kliniq.Application.Common.Validation
{
    public static class FileSignatureValidator
    {
        private static readonly Dictionary<string, List<byte[]>> Signatures = new()
        {
            { ".pdf",  [ [0x25, 0x50, 0x44, 0x46] ] },
            { ".png",  [ [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A] ] },
            { ".jpg",  [ [0xFF, 0xD8, 0xFF, 0xE0],
                         [0xFF, 0xD8, 0xFF, 0xE1],
                         [0xFF, 0xD8, 0xFF, 0xDB] ] },
            { ".jpeg", [ [0xFF, 0xD8, 0xFF, 0xE0],
                         [0xFF, 0xD8, 0xFF, 0xE1],
                         [0xFF, 0xD8, 0xFF, 0xDB] ] },
        };

        public static bool IsValidSignature(Stream stream, string extension)
        {
            if (!Signatures.TryGetValue(extension, out var signatures) || !stream.CanRead || !stream.CanSeek)
                return false;

            var maxLen = signatures.Max(s => s.Length);
            var header = new byte[maxLen];
            var originalPosition = stream.Position;

            try
            {
                var bytesRead = stream.Read(header, 0, maxLen);
                if (bytesRead < 4) return false;

                return signatures.Any(sig => header.Take(sig.Length).SequenceEqual(sig));
            }
            finally
            {
                stream.Seek(originalPosition, SeekOrigin.Begin);
            }
        }
    }
}