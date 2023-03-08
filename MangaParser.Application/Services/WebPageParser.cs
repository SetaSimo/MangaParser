using MangaParser.Common.Dto;
using MangaParser.Common.Exceptions;
using PuppeteerSharp;
using System.Text.RegularExpressions;

namespace MangaParser.Application.Services
{
	public class WebPageParser : BaseFileService
	{
		private const string EndOfRegex = "[^>]*)(\")";

		private const int ImageGroup = 1;
		private const int TasksPerChunk = 10;

		public static async Task<IEnumerable<ParsingResultDetails>> GetPageContentAsync(string path, string baseUrl, string beginingOfLink, int startPage, int endPage, CancellationToken cancellationToken)
		{
			if (startPage > endPage)
				throw new ValidationExceptionInvalidPageValue(new Exception());

			var totalPages = endPage - startPage;

			await CheckForInstalledDependenciesAsync();

			var imageUrlsTasks = new List<Task<ParsingResultDetails>>();

			for (int i = startPage; i < totalPages; i++)
				imageUrlsTasks.Add(GetPageImageAsync(baseUrl, beginingOfLink, i));

			var parsingResult = await ExecuteTasksAsync(imageUrlsTasks);

			try
			{
				var pathToImagesTasks = parsingResult.Select(r => DownloadImageAsync(r, path, cancellationToken));

				return await ExecuteTasksAsync(pathToImagesTasks);
			}
			catch (Exception ex)
			{
				DeleteTempFiles(path, parsingResult.Select(r => new KeyValuePair<string, string>(r.Id.ToString(), GetFileExtention(r.Url))));
				throw;
			}
		}

		/// <summary>
		/// Executes chunk tasks to avoid throttling
		/// </summary>
		private static async Task<IEnumerable<T>> ExecuteTasksAsync<T>(IEnumerable<Task<T>> tasksToExecute)
		{
			return tasksToExecute
				.Chunk(TasksPerChunk)
				.Select(async c =>
				{
					var result = await Task.WhenAll(c);
					return result.Select(r => r);
				})
				.SelectMany(a => a.Result)
				.ToList();
		}

		private static async Task<ParsingResultDetails> GetPageImageAsync(string baseUrl, string beginingOfLink, int pageNumber)
		{
			using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true }))
			{
				using (var page = await browser.NewPageAsync())
				{
					await page.GoToAsync($"{baseUrl}{pageNumber}");
					//TODO delete const and change to WaitForSelectorAsync
					await page.WaitForTimeoutAsync(200);

					var regexPattern = $"({beginingOfLink}{EndOfRegex}";
					var imageUrl = Regex.Match(await page.GetContentAsync(), regexPattern).Groups[ImageGroup].Value;
					return new ParsingResultDetails(imageUrl, pageNumber);
				}
			}
		}

		private static async Task<BrowserFetcher> CheckForInstalledDependenciesAsync()
		{
			var browserFetcher = new BrowserFetcher();
			await browserFetcher.DownloadAsync();
			return browserFetcher;
		}

		private static async Task<ParsingResultDetails> DownloadImageAsync(ParsingResultDetails parsingResultDetails, string path, CancellationToken cancellationToken)
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
