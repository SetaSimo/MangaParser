namespace MangaParser.Common.Exceptions
{
	public class ReadableException : Exception
	{
		public string ErrorMessage { get; private set; }

		public ReadableException(string errorMessage, Exception exception) : base(errorMessage, exception)
		{
			ErrorMessage = errorMessage;
		}
	}
}
