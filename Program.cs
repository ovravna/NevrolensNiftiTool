using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using Nifti.NET;

[Serializable]
class SerializableNifti
{
	private ushort[] _data;

	public ushort[] Data => _data;

	public int[] Dimensions => _dimensions;

	private int[] _dimensions;
	public SerializableNifti(ushort[] data, int[] dimensions)
	{
		 _data = data;
		 _dimensions = dimensions;
	}
}

namespace NiftiNET
{


	
	
	class Program
	{

		public static void JsonSerialize<T>(T c, string path)
		{
			// var sr = new StreamReader(s);
			// var data = sr.ReadToEnd();
			string jsonString = JsonSerializer.Serialize(c);	
			
			if (File.Exists(path))
				File.Delete(path);
			File.WriteAllText(path, jsonString);
		}
		public static void SerializeNow<T>(T c, string path) {  
			// File f = new File("temp.dat");  
			// Stream s = f.Open(FileMode.Create);  
			if (File.Exists(path))
				File.Delete(path);
			Stream s = File.Create(path);
			BinaryFormatter b = new BinaryFormatter();  
			b.Serialize(s, c);  
			s.Close();  
		}  
		public static T DeSerializeNow<T>(string path) {  
			// File f = new File("temp.dat");
			Stream s = File.OpenRead(path);
			// Stream s = f.Open(FileMode.Open);  
			BinaryFormatter b = new BinaryFormatter();  
			var c = (T) b.Deserialize(s);  
			// Console.WriteLine(c.name);  
			s.Close();
			return c;
		}  
		
		
		static void Main(string[] args)
		{

			string inPath;

			if (args.Length > 0)
				inPath = args[0];
			else
			{
				Console.WriteLine("Please give path to nii file");
				return;
			}

			Console.WriteLine($"Reading... From {inPath}");
			Nifti.NET.Nifti nifti = null; 
			if (!inPath.EndsWith("ole"))
				 nifti = NiftiFile.Read(inPath);
			Console.WriteLine($"Completed Nifti generation from {inPath}");

			var outPath = string.Empty;
			
			if (args.Length >= 2)
				outPath = args[1];

			var filetype = inPath.Split(".")
				.Skip(1) // First part is not filetype
				.Where(s => !int.TryParse(s, out _)) // ".01." is subversion and should be part of filename
				.Aggregate((acc, s) => $"{acc}.{s}");
			var filename = inPath.Split("\\/".ToCharArray()).Last().Replace("." + filetype, ""); 
				
			var jsonPath = $"{outPath}{filename}.json";
			
			if (outPath.EndsWith("json", StringComparison.InvariantCultureIgnoreCase))
				jsonPath = outPath;


			Console.WriteLine($"Serializing... To JSON at {jsonPath}");
			if (!inPath.EndsWith("ole"))
				 JsonSerialize(nifti, jsonPath);
			Console.WriteLine($"Done! JSON file at {jsonPath}");

		}
	}
}
