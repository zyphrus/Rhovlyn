#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using System.Security.Policy;
using Rhovlyn.Engine.Graphics;
using Rhovlyn.Engine.Util;
using Rhovlyn.Engine.Maps;
using Rhovlyn.Engine.States;
using Rhovlyn.Engine.Managers;

#endregion
namespace Rhovlyn.Engine
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class GameWindow : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		ContentManager content;

		int frames_past = 0;
		double frames_timer = 0;

		public double FPS { get; private set; }

		public GameWindow()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			content = new ContentManager("Content/settings.ini");

		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			ApplySettings();
			IsMouseVisible = true;
			//HACK : Borderless Window cannot be set on Linux
			//Window.IsBorderless = false;


			content.Camera = new Camera(Vector2.Zero , this.Window.ClientBounds);
			content.Init(graphics.GraphicsDevice);


			//Keep the camera up to date with the Client Size
			this.Window.ClientSizeChanged += (object sender, EventArgs e) => { content.Camera.UpdateBounds(this.Window.ClientBounds); };
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			content.GameStates.Add("world" , new WorldState() );
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
				Exit();
			}

			content.CurrentState.Update(gameTime);
			content.Audio.Update();

			this.Window.Title = "FPS:" + (int)FPS +  " " + this.content.Camera.Bounds.ToString();
			base.Update(gameTime);
			frames_timer += gameTime.ElapsedGameTime.TotalSeconds;
			if (frames_timer > 1)
			{
				FPS = frames_past / frames_timer;
				frames_past = 0;
				frames_timer = 0;
			}
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			if (this.content.CurrnetMap != null)
				graphics.GraphicsDevice.Clear(this.content.CurrnetMap.Background);
			else
				graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin();
				this.content.CurrentState.Draw(gameTime, spriteBatch , content.Camera);
			spriteBatch.End();

			frames_past++;
			base.Draw(gameTime);
		}

		public void ApplySettings()
		{
			//Texture & Web Resourses
			bool webresource = false;
			content.Settings.GetBool("Textures" , "AllowWebResources" , ref webresource);
			IO.Path.AllowWebResouces = webresource;

			bool webcache = webresource;
			content.Settings.GetBool("Textures" , "AllowWebResourcesCache" , ref webcache);
			IO.Path.AllowWebResoucesCaching = webcache;

			string cachepath = IO.Path.WebResoucesCachePath;
			content.Settings.Get("Textures" , "CachePath" , ref cachepath);
			IO.Path.WebResoucesCachePath = cachepath;

			int cachetimeout = IO.Path.WebResoucesCacheTimeOut;
			content.Settings.GetInt("Textures" , "CacheTimeout" , ref cachetimeout);
			IO.Path.WebResoucesCacheTimeOut = cachetimeout;

			LoadGraphicsSettings();
		}

		public void LoadGraphicsSettings()
		{
			bool resizable = false;
			content.Settings.GetBool("window", "resizable", ref resizable);
			Window.AllowUserResizing = resizable;

			bool fullscreen = false;
			content.Settings.GetBool("window", "fullscreen", ref fullscreen);
			graphics.IsFullScreen = fullscreen;

			//Does not work on Linux
			bool vsync = true;
			content.Settings.GetBool("window", "vsync", ref fullscreen);
			graphics.SynchronizeWithVerticalRetrace = vsync;
		}
	}
}

