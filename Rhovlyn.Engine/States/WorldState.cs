using System;
using Rhovlyn.Engine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rhovlyn.Engine.States
{
	public class WorldState : IGameState
    {
		private ContentManager content;
		public WorldState()
        {
        }

		public void Initialize()
		{

		}

		public void LoadContent(ContentManager content)
		{
			this.content = content;
			content.Maps.Add( "test" , "Content/map.txt" , content.Textures );
		}

		public void UnLoadContent(ContentManager content)
		{

		}

		public void Draw (GameTime gameTime , SpriteBatch spriteBatch , Camera camera)
		{
			content.CurrnetMap.Draw(gameTime , spriteBatch , camera);
			content.CurrnetMap.Sprites.Draw(gameTime , spriteBatch , camera);
		}
		public void Update (GameTime gameTime)
		{

		}
    }
}
