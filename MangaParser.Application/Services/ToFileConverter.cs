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
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

			var document = new PdfDocument(documentPath);
			try
			{

				foreach (var parsingResult in parsingResultDetails)
				{
					var page = document.AddPage();
					if (parsingResult.IsParsedCorrectly)
					{
						var gfx = XGraphics.FromPdfPage(page);
						gfx.DrawImage(XImage.FromFile(parsingResult.Url), 0, 0);
					}
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
