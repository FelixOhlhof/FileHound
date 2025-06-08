namespace PdfSearchWPF.Exceptions
{
  public class FileTypeNotSupportedException : SearchResultException
  {
    public FileTypeNotSupportedException(string fileName, string? fileType = null)
      : base(fileName, $"Type {fileType ?? ""} of file {fileName} not supported!".Replace("  ", " ")) { }

    public FileTypeNotSupportedException(string fileName, Exception innerException, string? fileType = null) : base(fileName, $"Type {fileType ?? ""} of file {fileName} not supported!".Replace("  ", " "), innerException) { }
  }
}
