using System.Text;

namespace Desafio02.Model
{
    class Pokemon
    {
        public string Description { get; set; }
        public string NumberExpansion { get; set; }
        public string Expansion { get; set; }
        public string UrlCardImage { get; set; }
        public string Base64CardImage { get; set; }

        public Pokemon(string description, string numberExpansion, string expansion, string urlCardImage, string base64CardImage)
        {
            Description = description;
            NumberExpansion = numberExpansion;
            Expansion = expansion;
            UrlCardImage = urlCardImage;
            Base64CardImage = base64CardImage;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"- Description: {Description}")
                .AppendLine($"- Expansion: {Expansion}")
                .AppendLine($"- Number Expansion: {NumberExpansion}")
                .AppendLine($"- Url Card Image: {UrlCardImage}")
                .AppendLine($"- Base64 Card Image: {Base64CardImage}");

            return sb.ToString();
        }
    }
}
