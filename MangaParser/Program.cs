using MangaParser.Application.Services;
using MangaParser.Common.Enums;

var imagePaths = await WebPageParser.GetPageContent("", "", "", 1, 5, new CancellationToken());

ToFileConverter.ConvertToSelectedFormat("", "", DocumentFormat.Pdf, imagePaths);

