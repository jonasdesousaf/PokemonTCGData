using System.Text;

namespace Desafio02.Utils
{
    class Helper
    {
        public static string GetLink(int page = 0, string link = "")
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(@"https://www.pokemon.com");

            if (page > 0)
            {
                sb.Append(@"/us/pokemon-tcg/pokemon-cards/").Append(page)
                .Append(@"?cardName=&cardText=&evolvesFrom=&simpleSubmit=&format=unlimited&hitPointsMin=0&hitPointsMax=340&retreatCostMin=0&retreatCostMax=5&totalAttackCostMin=0&totalAttackCostMax=5&particularArtist=");
            }
            else
            {
                sb.Append(link);
            }

            return sb.ToString();
        }

        public static string GetPath(string fileName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"C:\Temp\")
                .Append(fileName);

            return sb.ToString();
        }
    }
}
