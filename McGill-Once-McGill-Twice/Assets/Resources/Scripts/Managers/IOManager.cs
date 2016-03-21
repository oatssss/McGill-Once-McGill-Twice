using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
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
    }

    public static T ReadFromFile<T>(string relativePath)
    {
        string filePath = Application.persistentDataPath + "/" + relativePath;
        string serialized = File.ReadAllText(filePath);
        T deserialized = Deserialize<T>(serialized);
        return deserialized;
    }

    public static void DeleteFile(string relativePath)
    {
        string filePath = Application.persistentDataPath + "/" + relativePath;
        File.Delete(filePath);
    }

    public static bool FileExists(string relativePath)
    {
        return File.Exists(Application.persistentDataPath + "/" + relativePath);
    }

    class DescendedDateComparer : IComparer<DateTime>
    {
        public int Compare(DateTime x, DateTime y)
        {
            // use the default comparer to do the original comparison for datetimes
            int ascendingResult = Comparer<DateTime>.Default.Compare(x, y);

            // turn the result around
            return 0 - ascendingResult;
        }
    }

    public static SortedList<DateTime,FileInfo> GetSavedSessionFiles()
    {
        DirectoryInfo dir = Directory.CreateDirectory(Application.persistentDataPath + "/" + GameConstants.PATH_SESSION_SAVES);
        FileInfo[] info = dir.GetFiles("*." + GameConstants.SUFFIX_SESSION_SAVES);
        SortedList<DateTime,FileInfo> list = new SortedList<DateTime,FileInfo>(new DescendedDateComparer());

        foreach (FileInfo f in info)
            { list.Add(f.LastWriteTime,f); }



        return list;
    }
}
