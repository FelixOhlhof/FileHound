using System.Windows.Controls;

namespace PdfSearchWPF.Views
{
  /// <summary>
  /// Interaction logic for SearchBar.xaml
  /// </summary>
  public partial class SearchBar : UserControl
  {
    public SearchBar()
    {
      InitializeComponent();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (string.IsNullOrEmpty(SearchString.Text))
      {
        SearchStringOverlay.Visibility = System.Windows.Visibility.Visible;
        DeleteSearchString.Visibility = System.Windows.Visibility.Hidden;
      }
      else
      {
        SearchStringOverlay.Visibility = System.Windows.Visibility.Hidden;
        DeleteSearchString.Visibility = System.Windows.Visibility.Visible;
      }
    }

    private void DeleteSearchString_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      SearchString.Focus();
    }
  }
}
