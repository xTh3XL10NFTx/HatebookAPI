namespace Hatebook.Common
{
    public class ErrorResponse
    {
        public ErrorResponse()
        {
            Errors = new List<string>();
        }
        public int StatusCode      { get; set; }
        public string? StatusPhrase { get; set; }
        public List<string> Errors { get; set; }
        public DateTime Timestamp  { get; set; }
    }
}
