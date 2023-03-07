using MangaParser.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaParser.Application.Services
{
	public class BaseFileService
	{
		protected static string GetFilePath(string path, string fileName, string extention)
		{
			try
			{
				string imagePath = GetPathPathWithoutFolderValidation(path, fileName, extention);
				Directory.CreateDirectory(path);

				return imagePath;
			}
			catch (Exception ex)
			{
				throw new UnableToCreateTempFolderException(path, ex);
			}
		}

		protected static string GetPathPathWithoutFolderValidation(string path, string fileName, string extension) => $"{Path.Combine(path, fileName)}{extension}";

		//TODO improve method
		protected static string GetFileExtention(string url) => Path.GetExtension(url);

		protected static void DeleteTempFiles(string basePath, IEnumerable<KeyValuePair<string , string>> imagesIdsAndExtentions)
		{
			foreach (var imageIdAndExtention in imagesIdsAndExtentions)
				File.Delete(GetPathPathWithoutFolderValidation(basePath, imageIdAndExtention.Key, imageIdAndExtention.Value));
		}

	}
}
