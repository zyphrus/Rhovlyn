using System;
using System.IO;
using System.Collections.Generic;

namespace Rhovlyn.Engine.IO
{
	public class PartialFile
	{
		private class BlockInfo
		{
			public int Start { get; set; }

			public int Length { get; set; }
		}

		private Dictionary<string , BlockInfo> blocks = new Dictionary<string , BlockInfo>();

		private string FilePath { get; set; }

		public PartialFile(string filepath)
		{
			FilePath = filepath;
			LoadFile();
		}

		public bool LoadFile()
		{
			using (var reader = new  StreamReader(new FileStream(FilePath, FileMode.Open))) {
				int position = 0;
				var current = new BlockInfo();
				blocks[""] = current;
				current.Start = position;
				while (!reader.EndOfStream) {
					var line = reader.ReadLine();
					var len = line.Length;
					position += len + 1;

					if (line.IndexOf('#') != -1)
						line = line.Substring(0, line.IndexOf('#')).Trim(); //removes all comments
					if (string.IsNullOrEmpty(line))
						continue;

					if (line.StartsWith("<") && line.EndsWith(">")) {
						current.Length = position - len - current.Start - 1;

						blocks[line.Substring(1, line.Length - 2)] = new BlockInfo();
						current = blocks[line.Substring(1, line.Length - 2)];

						current.Start = position;  
					}
				}
				current.Length = (int)(reader.BaseStream.Position - current.Start);
			}
			return true;
		}

		public bool BlockExists(string name)
		{
			return blocks.ContainsKey(name);
		}

		public string[] BlockNames {
			get {
				var names = new string[blocks.Keys.Count];
				blocks.Keys.CopyTo(names, 0);
				return names;
			}
		}

		public MemoryStream LoadBlock(string name = "")
		{
			if (!BlockExists(name))
				return null;
			using (var reader = new StreamReader(new FileStream(FilePath, FileMode.Open))) {

				var info = this.blocks[name];
				var buffer = new byte[info.Length];
				Console.WriteLine("Getting Block " + name + " length " + info.Length + " @ " + info.Start);
				reader.BaseStream.Seek(info.Start, SeekOrigin.Begin);
				reader.BaseStream.Read(buffer, 0, info.Length);
				return new MemoryStream(buffer);
			}
		}
	}
}

