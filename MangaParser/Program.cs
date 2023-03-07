using MangaParser.Application.Services;
using MangaParser.Common.Enums;

var imagePaths = await WebPageParser.GetPageContent("D:\\Projects\\MangaParser\\TempImages", "https://mangalib.me/goodbye-eri/v0/c0?page=", "https://img33.imgslib.link//manga/", 1, 5, new CancellationToken());

ToFileConverter.ConvertToSelectedFormat("D:\\Projects\\MangaParser\\Done", "Seon2", DocumentFormat.Pdf, imagePaths);

