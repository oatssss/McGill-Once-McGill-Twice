using UnityEngine;
using System.IO;
using FullSerializer;

public class IOManager {
    
    public class ObjectParseException : IOException { }
    
    private static readonly fsSerializer Serializer = new fsSerializer();
    
    public static string Serialize<T>(T aValue)
    {
        // serialize the data
        fsData data;
        Serializer.TrySerialize(typeof(T), aValue, out data).AssertSuccess();

        // emit the data via JSON
        return fsJsonPrinter.CompressedJson(data);
    }
    
    public static T Deserialize<T>(string serializedState) {
        // step 1: parse the JSON data
        fsData data = fsJsonParser.Parse(serializedState);

        // step 2: deserialize the data
        object deserialized = null;
        Serializer.TryDeserialize(data, typeof(T), ref deserialized).AssertSuccess();
        T output = (T) deserialized;
        
        if (output == null)
            { throw new ObjectParseException(); }
        else
            { return (T) deserialized; }
    }
    
    public static void WriteToFile<T>(string relativePath, T aValue)
    {
        string filePath = Application.persistentDataPath + "/" + relativePath;
        string serialized = Serialize<T>(aValue);
        File.WriteAllText(filePath, serialized);
        // StreamWriter fileWriter = File.CreateText(fileName);
        // fileWriter.WriteLine(serialzed);
        // fileWriter.Close();
    }
    
    public static T ReadFromFile<T>(string relativePath)
    {
        string filePath = Application.persistentDataPath + "/" + relativePath;
        string serialized = File.ReadAllText(filePath);
        T deserialized = Deserialize<T>(serialized);
        return deserialized;
    }
}
