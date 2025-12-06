namespace Lab5.Models
{
    public class ExternalLinkModel
    {
        public required string Url { get; set; }
        public required string Text { get; set; }

        public ExternalLinkModel() { }

        public ExternalLinkModel(string url, string text)
        {
            Url = url;
            Text = text;
        }
    }
}