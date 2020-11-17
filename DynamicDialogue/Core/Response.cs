using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicDialogueTest")]
[assembly: InternalsVisibleTo("DynamicDialogueTest2")]
namespace DynamicDialogue.Core
{
	/// <summary>
	/// A response holds a number of lines
	/// </summary>
	internal class Response
	{
		private List<string> lines = new List<string>();
		private Random random = new Random();

		public string Name
		{
			get;
		}

		public int LineCount => lines.Count;

		public Response(string name)
		{
			Name = name;
		}

		public Response AddLine(string text)
		{
			lines.Add(text);
			return this;
		}

		public string GetRandomLine()
		{
			return lines[random.Next(0, lines.Count)];
		}
	}
}
