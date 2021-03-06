using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Rhovlyn.Engine.Graphics
{
	public delegate void AnimationFrameChangedHandler(AnimatedSprite sprite,int index);
	public delegate void AnimationEndedHandler(AnimatedSprite sprite);
	public delegate void AnimationStartedHandler(AnimatedSprite sprite);

	public class Animation
	{
		public Animation(List<int> frames = null, List<double> times = null)
		{
			this.frames = frames ?? new List<int>();

			this.times = times ?? new List<double>();

			AnimationStarted = null;
			AnimationEnded = null;
			FrameChanged = null;
		}

		List<int> frames;

		public List<int> Frames { get { return frames; } }

		// Frame to use for index of animation
		List<double> times;

		public List<double> Times { get { return  times; } }

		//Events for Starting, Ending and changes in animation
		public event AnimationStartedHandler AnimationStarted;
		public event AnimationEndedHandler AnimationEnded;
		public event AnimationFrameChangedHandler FrameChanged;

		public void OnAnimationStarted(AnimatedSprite sprite)
		{
			if (AnimationStarted != null)
				AnimationStarted(sprite);
		}

		public void OnAnimationEnded(AnimatedSprite sprite)
		{
			if (AnimationEnded != null)
				AnimationEnded(sprite);
		}

		public void OnFrameChanged(AnimatedSprite sprite, int index)
		{
			if (FrameChanged != null)
				FrameChanged(sprite, index);
		}
	};

	public class SpriteMap
	{
		public Texture2D Texture { get; private set; }

		public List<Rectangle> Frames { get; private set; }

		public string TextureName { get { return Texture.Name; } private set { Texture.Name = value; } }

		public Dictionary <string , Animation> Animations { get; private set; }

		public SpriteMap(Texture2D texture, string name)
		{
			Texture = texture;
			TextureName = name;
			Frames = new List<Rectangle>();
			Frames.Add(new Rectangle(0, 0, Texture.Width, Texture.Height));
			Animations = new Dictionary<string , Animation>();
		}

		public SpriteMap(Texture2D texture, string name, List<Rectangle> frames)
		{
			Texture = texture;
			TextureName = name;
			Frames = frames;
			Animations = new Dictionary<string , Animation>();
		}

		public SpriteMap(Texture2D texture, string name, int rows, int coloumns)
		{
			Texture = texture;
			TextureName = name;
			Animations = new Dictionary<string , Animation>();
			int w = Texture.Width / rows;
			int h = Texture.Height / coloumns;

			Frames = new List<Rectangle>();
			for (int y = 0; y < Texture.Height; y += h) {
				for (int x = 0; x < Texture.Width; x += w) {
					Frames.Add(Util.TextureUtil.EncapsulateTexture(texture, new Rectangle(x, y, w, h)));
				}
			}
		}

		#region Animation Management

		public bool AddAnimation(string name, Animation animation)
		{
			if (!ExistsAnimation(name)) {
				Animations.Add(name, animation);
				return true;
			}
			return false;
		}

		public bool ExistsAnimation(string name)
		{
			return Animations.ContainsKey(name);
		}

		public Animation GetAnimation(string name)
		{
			return Animations.ContainsKey(name) ? Animations[name] : null;
		}


		//		public static object ParseAnimation(string text)
		//		{
		//			var frames = new List<int>();
		//			var times = new List<double>();
		//			int loops = 0;
		//			//Example
		//			// 1,2:0.1,3:1.0
		//			// #loops,[FrameIndex:Time]...
		//			// Read as : 1 loop , 1st index frame=2 for 0.1sec, 2nd index frame=3 for 1.0sec
		//			foreach (var seg in text.Split(',')) {
		//				var opt = seg.Split(':');
		//				if (opt.Length == 1) {
		//					loops = int.Parse(opt[0]);
		//				} else if (opt.Length == 2) {
		//					frames.Add(int.Parse(opt[0]));
		//					times.Add(double.Parse(opt[1]));
		//				} else {
		//					throw new System.IO.InvalidDataException("Invalid Animation segment : " + seg);
		//				}
		//			}
		//			return new Animation(frames, times);
		//		}

		#endregion
	}
}

