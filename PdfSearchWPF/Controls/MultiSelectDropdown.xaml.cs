using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace PdfSearchWPF.Controls
{
  public partial class MultiSelectDropdown : UserControl, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<string> ItemsSource
    {
      get => (ObservableCollection<string>)GetValue(ItemsSourceProperty);
      set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(
            nameof(ItemsSource),
            typeof(ObservableCollection<string>),
            typeof(MultiSelectDropdown),
            new PropertyMetadata(new ObservableCollection<string>()));

    public ObservableCollection<string> SelectedItems
    {
      get => (ObservableCollection<string>)GetValue(SelectedItemsProperty);
      set => SetValue(SelectedItemsProperty, value);
    }

    public static readonly DependencyProperty SelectedItemsProperty =
        DependencyProperty.Register(
            nameof(SelectedItems),
            typeof(ObservableCollection<string>),
            typeof(MultiSelectDropdown),
            new PropertyMetadata(new ObservableCollection<string>(), OnSelectedItemsChanged));

    private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = (MultiSelectDropdown)d;

      if (e.OldValue is ObservableCollection<string> oldCollection)
        oldCollection.CollectionChanged -= control.SelectedItems_CollectionChanged;

      if (e.NewValue is ObservableCollection<string> newCollection)
        newCollection.CollectionChanged += control.SelectedItems_CollectionChanged;

      control.OnPropertyChanged(nameof(SelectedItems));
      control.OnPropertyChanged(nameof(SelectedItemsDisplay));

      // Update backgrounds on collection replacement
      control.UpdateBackgroundBindings();
    }

    private void SelectedItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
      OnPropertyChanged(nameof(SelectedItems));
      OnPropertyChanged(nameof(SelectedItemsDisplay));

      // Update backgrounds on item add/remove
      UpdateBackgroundBindings();
    }

    private void UpdateBackgroundBindings()
    {
      if (ItemsControlList == null)
        return;

      foreach (var item in ItemsControlList.Items)
      {
        var container = ItemsControlList.ItemContainerGenerator.ContainerFromItem(item) as ContentPresenter;
        if (container != null)
        {
          var border = FindVisualChild<Border>(container);
          if (border != null)
          {
            var expression = BindingOperations.GetMultiBindingExpression(border, Border.BackgroundProperty);
            expression?.UpdateTarget();
          }
        }
      }
    }

    private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
    {
      if (parent == null)
        return null;

      int count = VisualTreeHelper.GetChildrenCount(parent);
      for (int i = 0; i < count; i++)
      {
        var child = VisualTreeHelper.GetChild(parent, i);
        if (child is T tChild)
          return tChild;

        var result = FindVisualChild<T>(child);
        if (result != null)
          return result;
      }
      return null;
    }

    public string PlaceholderText
    {
      get => (string)GetValue(PlaceholderTextProperty);
      set => SetValue(PlaceholderTextProperty, value);
    }

    public static readonly DependencyProperty PlaceholderTextProperty =
        DependencyProperty.Register(
            nameof(PlaceholderText),
            typeof(string),
            typeof(MultiSelectDropdown),
            new PropertyMetadata("Select...", OnPlaceholderTextChanged));

    private static void OnPlaceholderTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = (MultiSelectDropdown)d;
      control.OnPropertyChanged(nameof(SelectedItemsDisplay));
    }

    public string SelectedItemsDisplay =>
        SelectedItems != null && SelectedItems.Count > 0
            ? string.Join(", ", SelectedItems)
            : PlaceholderText;

    public MultiSelectDropdown()
    {
      InitializeComponent();
    }

    private void Item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (sender is Border border && border.Tag is string item)
      {
        if (SelectedItems.Contains(item))
          SelectedItems.Remove(item);
        else
          SelectedItems.Add(item);
      }
    }

    protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


    private void Button_Click(object sender, RoutedEventArgs e)
    {

      string addExt = addExtension.Text;

      if (addExt == string.Empty)
        return;

      if (!addExt.StartsWith("."))
        addExt = $".{addExt}";

      ItemsSource.Add(addExt);
      addExtension.Text = "";
    }

    private void DropdownButton_Click(object sender, RoutedEventArgs e)
    {
      if (DropdownButton.IsChecked == true)
        DropdownPopup.IsOpen = !DropdownPopup.IsOpen;
    }
  }
}

