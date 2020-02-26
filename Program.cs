using Desafio02.Model;
using Desafio02.Utils;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Desafio02
{
    class Program
    {
        static bool multipleFiles = false;

        static void Produce(int pages, ITargetBlock<Pokemon> target)
        {
            Parallel.For(1, pages + 1, (i) =>
            {
                HtmlWeb web = new HtmlWeb();

                var htmlDoc = web.Load(Helper.GetLink(i));

                var nodes = htmlDoc.DocumentNode.QuerySelectorAll("#cardResults > li a");

                Parallel.ForEach(nodes, (item) =>
                {
                    var htmlItem = web.Load(Helper.GetLink(0, item.GetAttributeValue("href", "")));
                    string description = htmlItem.DocumentNode.QuerySelector(".card-description h1").InnerText;
                    string expansion = htmlItem.DocumentNode.QuerySelector(".stats-footer a").InnerText;
                    string expansionNumber = htmlItem.DocumentNode.QuerySelector(".stats-footer span").InnerText;
                    string urlCardImage = htmlItem.DocumentNode.QuerySelector(".card-image img").GetAttributeValue("src", "");
                    string base64CardImage = Convert.ToBase64String(new HttpClient().GetByteArrayAsync(urlCardImage).Result);

                    target.Post(new Pokemon(description, expansionNumber, expansion, base64CardImage));
                });
            });

            target.Complete();
        }

        static async Task ConsumeAsync(ISourceBlock<Pokemon> source)
        {
            BlockingCollection<Pokemon> bag = new BlockingCollection<Pokemon>();

            while (await source.OutputAvailableAsync())
            {
                Pokemon data = (Pokemon)source.Receive();
                bag.Add(data);
            }

            if (!multipleFiles)
                await CreateSingleFile(bag);
            else
                await CreateMultipleFile(bag);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("------------------------- Pokemon TCG Extract Data -------------------------");
            Console.Write("Enter the number of pages to be extracted (1 -845): ");
            int pagesNumber = int.Parse(Console.ReadLine());
            Console.Write("Do you want to save cards in multiple files? (true/false): ");
            multipleFiles = bool.Parse(Console.ReadLine());

            var buffer = new BufferBlock<Pokemon>();
            var consumer = ConsumeAsync(buffer);

            Produce(pagesNumber, buffer);

            consumer.Wait();
        }

        static async Task CreateSingleFile(BlockingCollection<Pokemon> list)
        {
            using (StreamWriter file = File.CreateText(Helper.GetPath("single_file_pokemons.json")))
            {
                var json = JsonSerializer.Serialize(list);
                await file.WriteAsync(json);
            }
        }

        static async Task CreateMultipleFile(BlockingCollection<Pokemon> list)
        {
            foreach (var pokemon in list)
            {
                using (StreamWriter file = File.CreateText(Helper.GetPath("multiple_file_" + pokemon.Description + ".json")))
                {
                    var json = JsonSerializer.Serialize(pokemon);
                    await file.WriteAsync(json);
                }
            }
        }
    }
}
