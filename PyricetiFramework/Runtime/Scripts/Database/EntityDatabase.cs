using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator

namespace PyricetiFramework
{
  // public interface IEntityProvider
  // {
  //   T GetEntity<T>(string id) where T : EntityData;
  // }
  
  public interface IEntityProvider<out T>
    // : IEntityProvider
    where T : EntityData
  {
    T GetEntity(string id);
  }
  
  // public interface IEntityProvider<out T> where T : EntityData
  // {
  //   T GetEntity(string id);
  // }


  public abstract class EntityDatabase : EngineScriptableObject { }
  
  public abstract class EntityDatabase<T> : EntityDatabase
    , IEntityProvider<T>
    // , IEnumerator<T>
    where T : EntityData
  {
    [SerializeField] private List<T> allEntities = new();

    /*public T Current { get; private set; }

    object IEnumerator.Current => Current;

    private int curIndex;*/

    private readonly List<string> guidsListCheckerCache = new List<string>(128);

    public ReadOnlyCollection<T> AllEntitiesRO => this.allEntities.AsReadOnly();

    public T GetEntity(string id)
    {
      foreach (T entity in this.allEntities)
      {
        if (entity.Id == id)
          return entity;
      }

      throw new EntityNotFound($"Entity with id <b>{id}</b> not found");
    }

    // public T1 GetEntity<T1>(string id) where T1 : EntityData
    // {
    //   return GetEntity(id);
    // }


    /*public T GetEntityUnsafe(string id)
    {
      try
      {
        return GetEntity(id);
      }
      catch (EntityNotFound)
      {
        return null;
      }
    }


    public bool MoveNext()
    {
      if (++curIndex >= allEntities.Count)
        return false;

      Current = allEntities[curIndex];
      return true;
    }


    public void Reset() => curIndex = -1;
    
    public void Dispose() { }*/

    private void OnEnable()
    {
      if (!Application.isPlaying) this.EditorCheckDataIntegrity();
    }

    private void EditorCheckDataIntegrity()
    {
      this.guidsListCheckerCache.Clear();

      foreach (T entity in this.allEntities)
      {
        if (this.guidsListCheckerCache.Contains(entity.Guid))
          Debug.LogError($"{this.name} database found duplicate guid {entity.Guid} for entity {entity.Id}");

        this.guidsListCheckerCache.Add(entity.Guid);
      }
    }
  }
}