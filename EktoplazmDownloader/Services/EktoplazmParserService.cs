using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace EktoplazmExtractor.Services
{
    internal sealed class EktoplazmParserService
    {
        private readonly Regex pageNumberParser = new Regex("^(?<url>(https?://(www\\.)?ektoplazm.com/.+?/page/))(?<page>([0-9]+))$", RegexOptions.Compiled);

        public async Task<List<Album>> ParseAlbums(string url)
        {
            int? pageNumber = this.FindPageNumber(url);

            HttpClient client = new HttpClient();
            HtmlDocument document = new HtmlDocument();

            try
            {
                using (var stream = await client.GetStreamAsync(url))
                {
                    document.Load(stream);
                }
            }
            catch (Exception)
            {
                return Enumerable.Empty<Album>().ToList();
            }

            var result = document.DocumentNode
                .SelectNodes("//div[@id='main']/div[@class='post']")
                .Select(node =>
                {
                    var titleNode = node.SelectSingleNode(".//h1/a");
                    return new Album(
                        name: titleNode.InnerText,
                        url: titleNode.GetAttributeValue("href", String.Empty),
                        downloads: node
                            .SelectNodes(".//span[@class='dll']/a[@href]")
                            .ToDictionary(x => x.InnerText.Replace("Download", String.Empty).Trim(), x => x.GetAttributeValue("href", String.Empty))
                    );
                });

#if DEBUG_SINGLE_PAGE
#else
            if (result.Any() == true)
            {
                result = result.Concat(await this.ParseAlbums(this.SetPageNumber(url, (pageNumber ?? 1) + 1)));
            }
#endif

            return result.ToList();
        }

        private int? FindPageNumber(string url)
        {
            var match = this.pageNumberParser.Match(url);
            int result = 0;

            if (match.Success == true && Int32.TryParse(match.Groups["page"].Value, out result) == true)
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        private string SetPageNumber(string url, int pageNumber)
        {
            if (this.pageNumberParser.IsMatch(url) == true)
            {
                return this.pageNumberParser.Replace(url, x => String.Concat(x.Groups["url"].Value, pageNumber));
            }
            else
            {
                return String.Concat(url, "/page/", pageNumber);
            }
        }
    }
}