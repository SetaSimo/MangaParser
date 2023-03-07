using MangaParser.Common.Dto;
using MangaParser.Common.Exceptions;
using PuppeteerSharp;
using System.Text.RegularExpressions;

namespace MangaParser.Application.Services
{
	public class WebPageParser : BaseFileService
	{
		private static readonly string _endOfRegex = "[^>]*)(\")";

		private static readonly int _imageGroup = 1;

		public static async Task<IEnumerable<ParsingResultDetails>> GetPageContent(string path, string baseUrl, string beginingOfLink, int startPage, int endPage, CancellationToken cancellationToken)
		{
			if (startPage > endPage)
				throw new ValidationExceptionInvalidPageValue(new Exception());

			var totalPages = endPage - startPage;

			await CheckForInstalledDependencies();

			var imageUrlsTasks = new List<Task<ParsingResultDetails>>();

			for (int i = startPage; i < totalPages; i++)
				imageUrlsTasks.Add(GetPageImage(baseUrl, beginingOfLink, i));

			var parsingResult = await Task.WhenAll(imageUrlsTasks);

			try
			{
				var pathToImagesTasks = parsingResult.Select(r => DownloadImage(r, path, cancellationToken));

				return await Task.WhenAll(pathToImagesTasks);
			}
			catch (Exception ex)
			{
				DeleteTempFiles(path, parsingResult.Select(r => new KeyValuePair<string, string>(r.Id.ToString(), GetFileExtention(r.Url))));
				throw;
			}
		}

		private static async Task<ParsingResultDetails> GetPageImage(string baseUrl, string beginingOfLink, int pageNumber)
		{
			using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true }))
			{
				using (var page = await browser.NewPageAsync())
				{
					await page.GoToAsync($"{baseUrl}{pageNumber}");
					//TODO delete const
					await page.WaitForTimeoutAsync(200);

					var regexPattern = $"({beginingOfLink}{_endOfRegex}";
					var imageUrl = Regex.Match(await page.GetContentAsync(), regexPattern).Groups[_imageGroup].Value;
					return new ParsingResultDetails(imageUrl, pageNumber);
				}
			}
		}

		private static async Task<BrowserFetcher> CheckForInstalledDependencies()
		{
			var browserFetcher = new BrowserFetcher();
			await browserFetcher.DownloadAsync();
			return browserFetcher;
		}

		private static async Task<ParsingResultDetails> DownloadImage(ParsingResultDetails parsingResultDetails, string path, CancellationToken cancellationToken)
		{
			if (!parsingResultDetails.IsParsedCorrectly)
				return parsingResultDetails;

			var pathToImage = GetFilePath(path, parsingResultDetails.Id.ToString(), GetFileExtention(parsingResultDetails.Url));
			using (var client = new HttpClient())
			{
				var rawImage = await client.GetStreamAsync(new Uri(parsingResultDetails.Url));
				using (var fileStream = File.Create(pathToImage))
				{
					rawImage.CopyTo(fileStream);
				}
			}

			return new ParsingResultDetails(pathToImage, parsingResultDetails.Page);
		}

	}
}
