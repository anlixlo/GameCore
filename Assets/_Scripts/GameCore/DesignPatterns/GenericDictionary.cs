using System.Collections.Generic;

public class GenericDictionary
{
    private Dictionary<string, object> _dict = new Dictionary<string, object>();

    public void Add<T>(string key, T value) where T : class
    {
        _dict.Add(key, value);
    }

    public void Remove<T>(string key, T value) where T : class
    {
        if (_dict.ContainsKey(key))
        {
            _dict.Remove(key);
        }
    }

    public T GetValue<T>(string key) where T : class
    {
        return _dict[key] as T;
    }

    public void Clear()
    {
        _dict.Clear();
    }
}
