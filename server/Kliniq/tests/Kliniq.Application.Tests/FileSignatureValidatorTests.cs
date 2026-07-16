using Kliniq.Application.Common.Validation;

namespace Kliniq.Application.Tests;

public sealed class FileSignatureValidatorTests
{
    [Fact]
    public void ValidPdfSignature_IsAcceptedAndStreamPositionIsRestored()
    {
        using var stream = new MemoryStream([0x25, 0x50, 0x44, 0x46, 0x2D]);
        stream.Position = 0;

        var result = FileSignatureValidator.IsValidSignature(stream, ".pdf");

        Assert.True(result);
        Assert.Equal(0, stream.Position);
    }

    [Fact]
    public void MismatchedSignature_IsRejected()
    {
        using var stream = new MemoryStream([0x4D, 0x5A, 0x90, 0x00]);
        Assert.False(FileSignatureValidator.IsValidSignature(stream, ".pdf"));
    }

    [Fact]
    public void NonSeekableStream_IsRejectedWithoutReading()
    {
        using var stream = new NonSeekableStream([0x25, 0x50, 0x44, 0x46]);
        Assert.False(FileSignatureValidator.IsValidSignature(stream, ".pdf"));
    }

    private sealed class NonSeekableStream(byte[] content) : MemoryStream(content)
    {
        public override bool CanSeek => false;
        public override long Position
        {
            get => base.Position;
            set => throw new NotSupportedException();
        }
        public override long Seek(long offset, SeekOrigin loc) => throw new NotSupportedException();
    }
}
