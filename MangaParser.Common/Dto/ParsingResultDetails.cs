using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaParser.Common.Dto
{
	public class ParsingResultDetails
	{
		public Guid Id { get; private set; }
		public string Url { get; set; }
		public bool IsParsedCorrectly { get; private set; }
		public string Description { get; private set; }
		public int Page { get; set; }

		private const string EmptyUrlErrorText = "page image wasn't found";
		private readonly IEnumerable<string> _invalidSymbols = new List<string>() { "\"" };

		public ParsingResultDetails(string url, int page)
		{
			Id = Guid.NewGuid();
			Url = url;
			Page = page;
			if (string.IsNullOrWhiteSpace(url) || _invalidSymbols.Contains(url))
			{
				Description = $"{page} {EmptyUrlErrorText}";
				IsParsedCorrectly = false;
			}
			else
				IsParsedCorrectly = true;
		}


	}
}
