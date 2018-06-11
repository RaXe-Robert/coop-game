using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Helper class for loading files.
/// </summary>
public static class FileLoader
{
    /// <summary>
    /// Reads all the save files from the given data path.
    /// </summary>
    /// <returns>List of Tuples that contains -FileName,FileContent-. Empty if no files are found</returns>
    public static List<Tuple<string, byte[]>> LoadFiles(string dataPath)
    {
        string[] files = Directory.GetFiles(dataPath);

        List<Tuple<string, byte[]>> filesRaw = new List<Tuple<string, byte[]>>();

        foreach (string file in files)
        {
            Tuple<string, byte[]> dataTuple = LoadFile(file);

            if (dataTuple != null)
                filesRaw.Add(dataTuple);
        }

        return filesRaw;
    }

    /// <summary>
    /// Read a the save files at the given path.
    /// </summary>
    /// <returns>List of Tuples that contains -FileName,FileContent-. Null if no file found.</returns>
    public static Tuple<string, byte[]> LoadFile(string filePath)
    {
        try
        {
            using (MemoryStream ms = new MemoryStream())
            using (FileStream fileStream = File.Open(filePath, FileMode.Open))
            {
                fileStream.CopyTo(ms);
                return Tuple.Create(Path.GetFileName(filePath), ms.ToArray());
            }

        }
        catch (IOException e)
        {
            Debug.LogError(e);
            return null;
        }
    }
}
