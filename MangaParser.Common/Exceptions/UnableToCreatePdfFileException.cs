namespace MangaParser.Common.Exceptions
{
	public class UnableToCreatePdfFileException : ReadableException
	{
		public UnableToCreatePdfFileException(string path, Exception exception) : base($"Unable to create pdf file {path}", exception) { }
	}
}
