using System;

namespace AMD.Util.AttributeHelper
{
  public class NameAttribute : Attribute
  {
    public string Name { get; private set; }
    public NameAttribute(string name)
    {
      this.Name = name;
    }
  }
}
