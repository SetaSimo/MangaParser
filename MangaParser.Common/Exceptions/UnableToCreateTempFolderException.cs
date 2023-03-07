namespace MangaParser.Common.Exceptions
{
	public class UnableToCreateTempFolderException : ReadableException
	{
		public UnableToCreateTempFolderException(string path, Exception exception) : base($"Unable to create file {path}", exception) { }
	}
}
