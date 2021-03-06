using System;
using System.IO;
using System.Collections.Generic;
using Rhovlyn.Engine.Util;

namespace Rhovlyn.Engine.IO
{
	/// <summary>
	/// <remarks>All Internal string must be in lower case</remarks>
	/// </summary>
	public class Settings
	{
		// < Header , Contents >
		private Dictionary<string , Dictionary< string , string >  > settings;

		public bool isLoaded { get; private set; }

		public Settings(string path)
		{
			isLoaded = this.Load(path);
		}

		public Settings()
		{
			isLoaded = false;
		}

		/// <summary>
		/// Load the specified path.
		/// </summary>
		/// <param name="path">Local path</param>
		public bool Load(string path)
		{
			using (var f = new FileStream(path, FileMode.Open))
			{
				return Load(f);
			}
		}

		/// <summary>
		/// Load a stream of Data assuming it is in the INI Format
		/// </summary>
		/// <param name="stream">Stream.</param>
		public bool Load(Stream stream)
		{
			isLoaded = false;
			settings = new Dictionary<string, Dictionary< string , string > >();
			settings.Add("", new Dictionary< string , string >());
			using (var reader = new StreamReader(stream))
			{
				var current = settings[""];
				var header = "";
				while (!reader.EndOfStream)
				{
					try
					{
						//In INI Format ;'s are comments
						var line = reader.ReadLine();
						if (line.IndexOf(';') != -1)
							line = line.Substring(0, line.IndexOf(';')); //removes all comments

						//We do not take kindly to empty lines
						line = line.Trim();
						if (string.IsNullOrEmpty(line))
							continue;

						//Header
						if (line.StartsWith("[") && line.EndsWith("]"))
						{
							header = line.Substring(1, line.Length - 2).ToLower();
							if (!Exists(header))
								settings.Add(header, new Dictionary< string , string >()); //Supports Partial definitaion
							current = settings[header];
						}
						//Key and Value
						else if (line.IndexOf('=') != -1)
						{
							//Split the line at the ='s
							var key = line.Substring(0, line.IndexOf('=')).Trim().ToLower();
							if (!Exists(header, key))
								current.Add(key, line.Substring(line.IndexOf('=') + 1).Trim());
							else
								Console.WriteLine("WARNING Double definition of " + header + "::" + key + "\nIgnoring new definition");
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error while reading settings");
						Console.WriteLine(ex);
					}
				}
			}
			isLoaded = true;
			return true;
		}

		public bool Exists(string header)
		{
			if (!isLoaded)
				return false;

			return this.settings.ContainsKey(header.ToLower());
		}

		public bool Exists(string header, string key)
		{
			if (!isLoaded)
				return false;

			if (Exists(header))
			{
				return this.settings[header.ToLower()].ContainsKey(key.ToLower());
			}
			return false;
		}

		/// <summary>
		/// Gets the <see cref="Rhovlyn.Engine.IO.Settings"/> with the specified header.
		/// </summary>
		/// <remark>Can throw expecptions</remark>
		/// <param name="header">Header.</param>
		public Dictionary< string , string > this [string header]
		{
			get { return this.settings[header.ToLower()]; }
		}

		public List<string> Headers { get { return new List<string>(this.settings.Keys); } }

		/// <summary>
		/// Get the specified header, key and return the value.
		/// </summary>
		/// <returns>
		/// True on successful retrival of value
		/// When false, result is not changed
		/// </returns>
		/// <param name="header">Header of the section</param>
		/// <param name="key">Key name</param>
		/// <param name="result">Result</param>
		public bool Get<T>(string header, string key, ref T result)
		{
			if (Exists(header, key))
			{
				var val = this.settings[header.ToLower()][key.ToLower()];
				return Parser.TryParse<T>(val, ref result);

			}
			return false;
		}
	}
}

