using System;

namespace PyricetiFramework
{
  public class EntityNotFound : Exception
  {
    public EntityNotFound(string message) : base(message) { }
  }
}