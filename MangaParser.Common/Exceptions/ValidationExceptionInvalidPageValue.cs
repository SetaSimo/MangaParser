namespace MangaParser.Common.Exceptions
{
	public class ValidationExceptionInvalidPageValue : ReadableException
	{
		public ValidationExceptionInvalidPageValue(int value, string param, Exception exception) : base($"{value} can't be used as {param}", exception) { }
		public ValidationExceptionInvalidPageValue(Exception exception) : base($"invalid input value", exception) { }
	}
}
