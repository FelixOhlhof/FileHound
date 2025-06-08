namespace PdfSearchWPF.Model
{
  public class SearchProgress
  {
    public int Total { get; set; }
    public int Current { get; set; }
    public bool IsFinished { get; set; } = true;
  }
}
