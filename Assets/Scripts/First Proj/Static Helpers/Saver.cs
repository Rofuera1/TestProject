using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class Saver
{
    public static void SaveFile<T>(T obj, string path) where T: class
    {
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
        }
    }
    public static T LoadFile<T>(string path) where T : class
    {
        if (!File.Exists(path))
            return null;

        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            T obj = formatter.Deserialize(stream) as T;
            return obj;
        }
    }
}
