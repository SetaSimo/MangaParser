using MangaParser.Common.Dto;
using MangaParser.Common.Enums;
using MangaParser.Common.Exceptions;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Text;

namespace MangaParser.Application.Services
{
	public class ToFileConverter : BaseFileService
	{
		private const int PdfImageHeight = 600;
		private const int PdfImageWidth = 600;

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

		//TODO improve image size for pdf
		private static void ToPdfConverter(string documentPath, IEnumerable<ParsingResultDetails> parsingResultDetails)
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

			using (var document = new PdfDocument(documentPath))
			{
				try
				{
					foreach (var parsingResult in parsingResultDetails)
					{
						CreatePageIfParsed(document, parsingResult);
					}

				}
				catch (Exception ex)
				{
					throw new UnableToCreatePdfFileException(documentPath, ex);
				}
				document.Close();
			}

			DeleteTempFiles(parsingResultDetails.Select(r => r.Url));
		}

		private static void CreatePageIfParsed(PdfDocument document, ParsingResultDetails parsingResult)
		{
			var page = document.AddPage();
			if (parsingResult.IsParsedCorrectly)
			{
				using (var gfx = XGraphics.FromPdfPage(page))
				{
					gfx.DrawImage(XImage.FromFile(parsingResult.Url), 0, 0, PdfImageWidth, PdfImageHeight);
				}
			}
			else
				Console.WriteLine(parsingResult.Description);
		}
	}
}
