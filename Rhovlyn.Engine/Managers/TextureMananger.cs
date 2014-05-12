using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.IO;
using Rhovlyn.Engine.Graphics;
using Microsoft.Xna.Framework;

namespace Rhovlyn.Engine.Util
{
    public class TextureMananger
    {
		private GraphicsDevice graphics;
		private Dictionary< string , SpriteMap > textures;

		public TextureMananger(GraphicsDevice graphics )
        {
			this.graphics = graphics;
			textures = new Dictionary<string, SpriteMap>();
        }

		public SpriteMap this[string index]
		{
			get { return textures[index]; }
			set { textures[index] = value; }
		}

		public bool Add (string name, SpriteMap spritemap)
		{
			if (!Exists(name))
			{
				textures.Add( name , spritemap );
				return true;
			}
			return false;
		}

		public bool Add (string name , Stream stream , List<Rectangle> frames)
		{

			if (!Exists(name))
			{
				textures.Add( name , new SpriteMap( Texture2D.FromStream( this.graphics , stream)  , frames ) );
				return true;
			}
			return false;
		} 

		public bool Add (string name , Stream stream)
		{
			if (!Exists(name))
			{
				textures.Add( name , new SpriteMap( Texture2D.FromStream( this.graphics , stream) ) );
				return true;
			}
			return false;
		}

		public bool Add (string name , string path)
		{
			return this.Add(name, new FileStream(path, FileMode.Open));
		}

		public bool Exists (string name)
		{
			return textures.ContainsKey(name);
		}

		/// <summary>
		/// Load a local file.
		/// </summary>
		/// <param name="path">Path</param>
		public bool Load(string path)
		{
			using (var fs = new FileStream(path , FileMode.Open))
			{
				return Load(fs);
			}
		}

		/// <summary>
		/// Load the specified stream.
		/// </summary>
		/// <param name="stream">Input Stream</param>
		public bool Load (Stream stream)
		{
			using (var reader = new StreamReader(stream))
			{
				try {
					while (!reader.EndOfStream)
					{
						var line = reader.ReadLine();
						if (line.IndexOf("#") != -1)
							line = line.Substring(0, line.IndexOf("#"));
						line.Trim();
						if (string.IsNullOrEmpty(line))
							continue;

						//Process commands
						if (line.StartsWith("@"))
						{
							line = line.Substring(1);

							if (line.StartsWith("include:"))
							{
								var obj = line.Substring(line.IndexOf("include:"));
								this.Load( Engine.IO.Path.ResolvePath(obj) );
							}
						} else {
							//Loads Local file
							var args = line.Split(',');

							var tname = args[0];
							var tpath = args[1];

							if (args.Length > 2)
							{
								List<Rectangle> rects = new List<Rectangle>();
								var tex = Texture2D.FromStream( graphics , Engine.IO.Path.ResolvePath(tpath));
								if (args[2].Contains("@"))
								{								
									// Explict Rectangle Declearations
									// x:y:w:h@....
									// 0:0:64:64@0:0:128:32
									foreach ( var rect in args[2].Split( '@') )
									{
										var parms = rect.Split(':');
										if (parms.Length != 4)
											continue;

										int x = 0,y = 0, w = 0,h = 0; 
										x = int.Parse(parms[0]);
										y = int.Parse(parms[1]);
										w = int.Parse(parms[2]);
										h = int.Parse(parms[3]);

										rects.Add(new Rectangle( x, y, w, h ));
									}
								} 
								// Semi-Implicit Bounds 
								// rows*colms
								// Divides up the texture into a set number of columns and rows
								// 2*2
								else if (args[2].Contains("*"))
								{
									var parms = args[2].Split('*');
									if (parms.Length != 2)
										continue;
									int row = int.Parse(parms[0]);
									int col = int.Parse(parms[1]);

									int w = (tex.Width / row);
									int h = (tex.Height / col);

									for (int y = 0; y < tex.Width; y += h)
									{
										for (int x = 0; x < tex.Width; x += w)
										{
											rects.Add(new Rectangle(x, y, w, h));
										}
									}
								}
								this.Add( tname , new SpriteMap( tex , rects ) );
							} else {
								this.Add( tname , Engine.IO.Path.ResolvePath(tpath) );
							}
						}
					}
				}	catch (Exception ex)
				{
					Console.WriteLine(ex);
					return false;
				}
			}
			return true;
		}

    }
}
