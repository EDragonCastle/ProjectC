using System.Collections.Generic;

/// <summary>
/// [Blackboard]
/// BT 시스템의 데이터 저장소다.
/// Dictionary로 구현해 Key-Value 형태로 공유하고 접근하는 데 사용된다.
/// </summary>
public class Blackboard
{
    private Dictionary<string, object> data = new Dictionary<string, object>();
    
    /// <summary>
    /// 지정된 값을 저장하거나 기존 데이터를 업데이트한다.
    /// </summary>
    /// <param name="key">데이터를 식별하는 고유 키</param>
    /// <param name="value">저장할 값</param>
    public void SetValue(string key, object value)
    {
        if (data.ContainsKey(key))
            data[key] = value;
        else
            data.Add(key, value);
    }

    /// <summary>
    /// 지정된 키에 해당하는 값을 가져온다. T 값을 사용하여 원하는 Type으로 캐스팅한다.
    /// </summary>
    /// <typeparam name="T">가져올 값의 예상 Type</typeparam>
    /// <param name="key">가져올 데이터의 키</param>
    /// <returns>키에 해당하는 값</returns>
    public T GetValue<T>(string key)
    {
        if(data.TryGetValue(key, out object value))
            return (T)value;

        // 없으면 Error Code를 던진다.
        throw new KeyNotFoundException($"BlackBoard에 키 {key}가 존재하지 않는다.");
    }

    /// <summary>
    /// Blackboard에 지정된 키가 있는지 확인한다.
    /// </summary>
    /// <param name="key">확인할 키</param>
    /// <returns>있으면 treu, 없으면 false</returns>
    public bool ContainKey(string key)
    {
        return data.ContainsKey(key);
    }

    public void RemoveValue(string key)
    {
        if(ContainKey(key))
            data.Remove(key);
    }
}
