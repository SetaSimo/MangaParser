using MangaParser.Application.Services;
using MangaParser.Common.Enums;
using MangaParser.Common.Exceptions;

var imagePaths = await WebPageParser.GetPageContentAsync("D:\\Projects\\MangaParser\\TempImages", "https://mangalib.me/goodbye-eri/v0/c0?page=", "https://img33.imgslib.link//manga/", 1, 5, new CancellationToken());

try
{
	ToFileConverter.ConvertToSelectedFormat("D:\\Projects\\MangaParser\\Done", "Seon2", DocumentFormat.Pdf, imagePaths);
}
catch (ReadableException ex)
{
	Console.WriteLine(ex.Message);
}
catch (Exception ex)
{
	throw;
}
finally
{
	//ToFileConverter.DeleteTempFiles(imagePaths.Select(r => r.Url));
}


