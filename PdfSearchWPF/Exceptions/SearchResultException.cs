namespace PdfSearchWPF.Exceptions
{
  public class SearchResultException : Exception
  {
    public string FileName { get; }

    public SearchResultException(string fileName, string message) : base(message)
    { FileName = fileName; }

    public SearchResultException(string fileName, string message, Exception innerException) : base(message, innerException)
    { FileName = fileName; }
  }
}
