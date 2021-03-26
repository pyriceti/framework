using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PyricetiFramework.Tests.Runtime
{
  public class EngineObjectTests
  {
    [Test]
    public void ATestObjectIsSuccessfullyCreated()
    {
      var go = new GameObject();
      go.AddComponent<EngineObjectTest>();
      
      Debug.Log("Created EngineObjectTest");
      
      Assert.IsNotNull(go);
      Assert.IsNotNull(go.GetComponent<EngineObjectTest>());
    }

    [UnityTest]
    public IEnumerator ATestObjectIsSuccessfullyCreatedThenDestroyed()
    {
      var go = new GameObject();
      go.AddComponent<EngineObjectTest>();
      yield return null;
      
      Debug.Log("Created EngineObjectTest");
      
      Object.Destroy(go);
      yield return null;
      
      Debug.Log("Destroyed EngineObjectTest");

      Assert.IsTrue(go == null);
    }
  }
}