using System;
using System.Collections.Generic;
using UnityEngine;

namespace PyricetiFramework
{
  public class DataManager : Singleton<DataManager>
  {
    [SerializeField] private List<EntityDatabase> databases = new();

    public List<EntityDatabase> Databases => this.databases;
    
    /// <summary>
    /// Enable to cache query results
    /// </summary>
    private readonly Dictionary<Type, int> typesToIdxes = new();

    public static EntityDatabase<TEntityData> GetDatabase<TDatabase, TEntityData>() where TEntityData : EntityData => Instance.InternalGetDatabase<TDatabase, TEntityData>();

    private EntityDatabase<TEntityData> InternalGetDatabase<TDatabase, TEntityData>() where TEntityData : EntityData
    {
      if (this.typesToIdxes.ContainsKey(typeof(TDatabase)))
        return this.databases[this.typesToIdxes[typeof(TDatabase)]] as EntityDatabase<TEntityData>;

      int idx = this.databases.FindIndex(d => d is TDatabase);
      if (idx < 0)
        throw new Exception($"{typeof(TDatabase)} not found in data dictionaries");

      this.typesToIdxes.Add(typeof(TDatabase), idx);
      return this.databases[idx] as EntityDatabase<TEntityData>;
    }
  }
}