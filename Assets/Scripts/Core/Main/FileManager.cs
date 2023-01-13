using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileManager : MonoBehaviour 
{
	public static string savPath 
	{
		get 
		{
			#if UNITY_EDITOR
			return "Assets/";
			#else
			return Application.persistentDataPath + "/";
			#endif
		}
	}	
	
	public static string dataPath => savPath + "data/";

	public static string fileExtension = ".txt";
	public static string GetDirectoryFromPath(string filePath)
	{
		string directoryPath = "";
		string[] parts = filePath.Split('/');
		foreach(string part in parts)
		{
			if (!part.Contains("."))
				directoryPath += part + "/";
		}
		return directoryPath;
	}
	
	public static bool TryCreateDirectoryFromPath(string path)
	{
		string directoryPath = path;

		if (Directory.Exists(path) || File.Exists(path)) return true;
		if (path.Contains("."))
		{
			directoryPath = GetDirectoryFromPath (path);
			if (Directory.Exists(directoryPath)) return true;
		}

		if (directoryPath != "" && !directoryPath.Contains("."))
		{
			print (directoryPath);
			try
			{
				Directory.CreateDirectory(directoryPath);
				return true;
			}
			catch (System.Exception e)
			{
				Debug.LogError ("Could not create Directory!\nERROR DETAILS: " + e.ToString ());
				return false;
			}
		}
		else
		{
			Debug.LogError("Directory was invalid - " + directoryPath + "\npath="+path + "\ndirectoryPath="+directoryPath);
			return false;
		}
	}
	
	public static string AttemptCorrectFilePath(string filePath)
	{
		//make sure we add the default save path if desired.
		filePath = filePath.Replace("[]", dataPath);
		//add the default extension if no extension is present.
		if (!filePath.Contains(".")) filePath += fileExtension;

		return filePath;
	}

	public static void SaveFile(string filePath, string line)
	{
		SaveFile(filePath, new List<string>(){line});
	}
	
	public static void SaveFile(string filePath, List<string> lines)
	{
		filePath = AttemptCorrectFilePath(filePath);

		//If the directory does not exist, try to create it. Prevent continuation if path was not valid.
		if (!TryCreateDirectoryFromPath(filePath))
		{
			Debug.LogError ("FAILED TO SAVE FILE [" + filePath + "] Please see console/log for details.");
			return;
		}

		StreamWriter sw = new StreamWriter(filePath);
		int i = 0;
		for (i = 0; i < lines.Count; i++)
		{
			sw.WriteLine(lines[i]);
		}

		sw.Close();

		print("Saved " + i + " lines to file [" +filePath+"]");
	}
	
	static List<string> ArrayToList(string[] array, bool removeBlankLines = true)
	{
		List<string> list = new List<string>();
		for(int i = 0; i < array.Length; i++)
		{
			string s = array[i];
			if (s.Length > 0 || !removeBlankLines)
			{
				list.Add(s);
			}
		}
		return list;
	}
	
	public static List<string> LoadFile(string filePath, bool removeBlankLines = true)
	{
		filePath = AttemptCorrectFilePath(filePath);

		if (File.Exists(filePath))
		{
			List<string> lines = ArrayToList(File.ReadAllLines(filePath), removeBlankLines);
			return lines;
		}
		else
		{
			string errorMessage = "ERR! File "+filePath+" does not exist!";
			Debug.LogError(errorMessage);
			return new List<string>(){errorMessage};
		}
	}
	
	public static void SaveJSON(string filePath, object classToSave)
	{
		string jsonString = JsonUtility.ToJson(classToSave);
		SaveFile(filePath, jsonString);
	}
	
	public static T LoadJSON<T>(string filePath)
	{
		string jsonString = LoadFile(filePath)[0];
		return JsonUtility.FromJson<T>(jsonString);
	}
}