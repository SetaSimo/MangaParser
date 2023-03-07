namespace MangaParser.Common.Exceptions
{
	public class ValidationExceptionUnknownFormat : ReadableException
	{
		public ValidationExceptionUnknownFormat(string DocumentFormatValue, Exception exception) : base($"unable to convert {DocumentFormatValue} to DocumentFormatValue", exception) { }
	}
}
