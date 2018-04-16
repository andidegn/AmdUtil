using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace AMD.Util.Data
{

  public abstract class SerializationHelper
  {
    public static object Clone(object toBeCloned, Type[] includedTypes)
    {
      return Deserialize(Serialize(toBeCloned, includedTypes), toBeCloned.GetType(), includedTypes);
      XmlSerializer xml = new XmlSerializer(toBeCloned.GetType(), includedTypes);
      MemoryStream ms = Serialize(toBeCloned, includedTypes);
      object obj = xml.Deserialize(ms);
      ms.Close();
      return obj;
    }

    public static MemoryStream Serialize(object toBeSerialized, Type[] includedTypes)
    {
      XmlSerializer xml = new XmlSerializer(toBeSerialized.GetType(), includedTypes);

      MemoryStream ms = new MemoryStream();

      xml.Serialize(ms, toBeSerialized);
      ms.Position = 0;
      return ms;
    }

    public static object Deserialize(MemoryStream ms, Type t, Type[] includedTypes)
    {
      XmlSerializer xml = new XmlSerializer(t, includedTypes);
      object obj = xml.Deserialize(ms);
      ms.Close();
      return obj;
    }

    public static String SerializeToString(object toBeSerialized, Type[] includedTypes)
    {
      XmlSerializer xml = new XmlSerializer(toBeSerialized.GetType(), includedTypes);

      StringWriter sw = new StringWriter();

      xml.Serialize(sw, toBeSerialized);
      return sw.ToString();
    }

    public static object DeserializeFromString(String serializedString, Type t, Type[] includedTypes)
    {
      XmlSerializer xml = new XmlSerializer(t, includedTypes);
      using (TextReader tr = new StringReader(serializedString))
      {
        try
        {
          return xml.Deserialize(tr);
        }
        catch (InvalidOperationException)
        {
          return null;
        }
      }
    }
  }
}
