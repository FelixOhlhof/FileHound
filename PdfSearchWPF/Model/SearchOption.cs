﻿namespace PdfSearchWPF.Model
{
  [Flags]
  public enum SearchOption
  {
    None = 0,
    Regex = 1 << 0,
    MatchWholeWord = 1 << 1,
    MatchCase = 1 << 2,
    Recursive = 1 << 3,
  }
}
