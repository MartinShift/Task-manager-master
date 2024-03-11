using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace Xmls;
public static class XmlHelper<T>
{
    public static void Serialize(T obj, string file)
    {
        var taskSerializer = new XmlSerializer(typeof(T));
        using (FileStream fs = new(file, FileMode.Create))
        {
            taskSerializer.Serialize(fs, obj);
        }
    }
    public static T Deserialize(string file)
    {
        using (var sr = new FileStream(file, FileMode.Open))
        {
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(sr);
        }
    }
}
