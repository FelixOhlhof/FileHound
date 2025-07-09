using System.Collections;
using System.IO;
using System.Text.Json;

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

    public void Save(string fileName)
    {
      string path = Path.Join(".", fileName);

      Directory.CreateDirectory(Path.GetDirectoryName(path)!);

      var json = JsonSerializer.Serialize(_values, new JsonSerializerOptions
      {
        WriteIndented = true
      });

      File.WriteAllText(path, json);
    }

    public static Settings Load(string fileName)
    {
      string path = Path.Join(".", fileName);

      var settings = new Settings();

      if (!File.Exists(path))
        return settings;

      var json = File.ReadAllText(path);

      var temp = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)!;

      foreach (var kvp in temp)
      {
        if (kvp.Value.ValueKind == JsonValueKind.String)
          settings._values[kvp.Key] = kvp.Value.GetString()!;
        else if (kvp.Value.ValueKind == JsonValueKind.Number && kvp.Value.TryGetInt32(out int intVal))
          settings._values[kvp.Key] = intVal;
        else if (kvp.Value.ValueKind == JsonValueKind.True || kvp.Value.ValueKind == JsonValueKind.False)
          settings._values[kvp.Key] = kvp.Value.GetBoolean();
        else if (kvp.Value.ValueKind == JsonValueKind.Array)
        {
          var list = new List<string>();
          foreach (var item in kvp.Value.EnumerateArray())
          {
            if (item.ValueKind == JsonValueKind.String)
              list.Add(item.GetString()!);
          }
          settings._values[kvp.Key] = list;
        }
      }

      return settings;
    }
  }

}
