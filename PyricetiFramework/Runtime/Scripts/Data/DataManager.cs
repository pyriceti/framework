using System;
using System.Collections.Generic;
using UnityEngine;

namespace PyricetiFramework
{
  public class DataManager : Singleton<DataManager>
  {
    [SerializeField] private List<DataDictionary> dataDictionaries = new List<DataDictionary>();

    /// <summary>
    /// Enable to cache query results
    /// </summary>
    private readonly Dictionary<Type, int> typesToIdxes = new Dictionary<Type, int>();

    public T getDataDictionary<T>() where T : DataDictionary
    {
      if (typesToIdxes.ContainsKey(typeof(T)))
        return (T) dataDictionaries[typesToIdxes[typeof(T)]];

      int idx = dataDictionaries.FindIndex(d => d is T);
      if (idx < 0)
        throw new Exception($"{typeof(T)} not found in data dictionaries");

      typesToIdxes.Add(typeof(T), idx);
      return (T) dataDictionaries[idx];
    }
  }
}