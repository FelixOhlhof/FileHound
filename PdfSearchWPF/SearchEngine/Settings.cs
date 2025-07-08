using System.Collections;

namespace PdfSearchWPF.SearchEngine
{
  public class Settings : IEnumerable
  {
    private readonly Dictionary<string, object> _values = new();

    public void Set<T>(string name, T value)
    {
      _values[name] = value!;
    }

    public T Get<T>(string name)
    {
      if (_values.TryGetValue(name, out var value))
        return (T)value;
      throw new KeyNotFoundException($"Option '{name}' not found.");
    }

    public bool TryGet<T>(string name, out T value)
    {
      if (_values.TryGetValue(name, out var obj) && obj is T castValue)
      {
        value = castValue;
        return true;
      }

      value = default!;
      return false;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _values.GetEnumerator();
    }
  }

}
