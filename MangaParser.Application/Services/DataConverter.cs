using iTextSharp.text;
using iTextSharp.text.pdf;
using MangaParser.Common.Dto;
using MangaParser.Common.Enums;
using MangaParser.Common.Exceptions;

namespace MangaParser.Application.Services
{
	public class DataConverter : BaseFileService
	{
		public static void ConvertToSelectedFormat(string documentPath, string fileName, DocumentFormat documentFormat, IEnumerable<ParsingResultDetails> parsingResultDetails)
		{
			switch (documentFormat)
			{
				case DocumentFormat.Pdf:
					ToPdfConverter(GetFilePath(documentPath, fileName, $".{documentFormat}"), parsingResultDetails);
					break;
				default:
					throw new ValidationExceptionUnknownFormat(documentFormat.ToString(), new Exception());
			}
		}

		private static void ToPdfConverter(string documentPath, IEnumerable<ParsingResultDetails> parsingResultDetails)
		{
			var document = new Document();
			try
			{
				PdfWriter.GetInstance(document, new FileStream(documentPath, FileMode.Create));

				document.Open();

				foreach (var parsingResult in parsingResultDetails)
				{
					if (parsingResult.IsParsedCorrectly)
						document.Add(Image.GetInstance(parsingResult.Url));
					else
						Console.WriteLine(parsingResult.Description);
				}
			}
			catch (Exception ex)
			{
				throw new UnableToCreatePdfFileException(documentPath, ex);
			}
			finally
			{
				document.Close();
			}

		}
	}
}
