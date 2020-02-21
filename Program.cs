using Desafio02.Model;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Desafio02
{
    class Program
    {
        static string linkBase = @"https://www.pokemon.com";
        static List<Pokemon> pokemons = new List<Pokemon>();
        static bool usingParallel = true;
        static Stopwatch sw = new Stopwatch();

        static void Main(string[] args)
        {
            Console.WriteLine("1 - Acesse o site https://www.pokemon.com/us/pokemon-tcg/pokemon-cards/");
            Console.WriteLine("2 - Realize uma consulta sem preencher nenhum campo (clicando em Search)");
            Console.WriteLine("3 - Informe abaixo a quantidade de páginas.");
            int pagesNumber = int.Parse(Console.ReadLine());

            if (!usingParallel)
            {
                sw.Start();
                for (int i = 1; i <= pagesNumber; i++)
                {
                    HtmlWeb web = new HtmlWeb();

                    var htmlDoc = web.Load(getLink(i));

                    var nodes = htmlDoc.DocumentNode.QuerySelectorAll("#cardResults > li a");

                    foreach (var item in nodes)
                    {
                        var htmlItem = web.Load(linkBase + item.GetAttributeValue("href", ""));
                        string description = htmlItem.DocumentNode.QuerySelector(".card-description h1").InnerText;
                        string urlCardImage = htmlItem.DocumentNode.QuerySelector(".card-image img").GetAttributeValue("src", "");
                        string expansion = htmlItem.DocumentNode.QuerySelector(".stats-footer a").InnerText;
                        string expansionNumber = htmlItem.DocumentNode.QuerySelector(".stats-footer span").InnerText;

                        WebClient wc = new WebClient();
                        string base64CardImage = Convert.ToBase64String(wc.DownloadData(urlCardImage));

                        pokemons.Add(new Pokemon(description, expansionNumber, expansion, urlCardImage, base64CardImage));
                    }
                }
                sw.Stop();
            }
            else
            {
                sw.Start();
                Parallel.For(1, pagesNumber + 1, (i) =>
                {
                    HtmlWeb web = new HtmlWeb();

                    var htmlDoc = web.Load(getLink(i));

                    var nodes = htmlDoc.DocumentNode.QuerySelectorAll("#cardResults > li a");

                    Parallel.ForEach(nodes, (item) =>
                    {
                        var htmlItem = web.Load(getLink(0, item.GetAttributeValue("href", "")));
                        string description = htmlItem.DocumentNode.QuerySelector(".card-description h1").InnerText;
                        string urlCardImage = htmlItem.DocumentNode.QuerySelector(".card-image img").GetAttributeValue("src", "");
                        string expansion = htmlItem.DocumentNode.QuerySelector(".stats-footer a").InnerText;
                        string expansionNumber = htmlItem.DocumentNode.QuerySelector(".stats-footer span").InnerText;

                        WebClient wc = new WebClient();
                        string base64CardImage = Convert.ToBase64String(wc.DownloadData(urlCardImage));

                        pokemons.Add(new Pokemon(description, expansionNumber, expansion, urlCardImage, base64CardImage));
                    });
                });
                sw.Stop();
            }

            Console.WriteLine("Consulta realizada em " + sw.Elapsed);

            string fileName = @"C:\Temp\CSharpAuthors.txt";
            try
            {
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    foreach (var pokemon in pokemons)
                    {
                        writer.WriteLine(pokemon.ToString());
                    }
                }
            }
            catch (Exception exp)
            {
                Console.Write(exp.Message);
            }
        }

        static string getLink(int page = 0, string link = "")
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(linkBase);

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
    }
}
