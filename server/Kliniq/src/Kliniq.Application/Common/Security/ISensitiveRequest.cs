namespace Kliniq.Application.Common.Security
{
    /// <summary>
    /// Marks requests whose payload must not be written to application logs.
    /// </summary>
    public interface ISensitiveRequest
    {
    }
}
