#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Rhovlyn.Engine;

#endregion
namespace Rhovlyn.Launcher
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			var gm = new Rhovlyn.Engine.GameWindow();
			gm.Run();
		}
	}
}
