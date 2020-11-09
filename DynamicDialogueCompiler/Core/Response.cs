using System;
using System.Collections.Generic;

namespace DynamicDialogue.Core
{
	/// <summary>
	/// A response holds a number of lines
	/// </summary>
	public class Response
	{
		private List<string> lines = new List<string>();
		private Random random = new Random();

		public string Name
		{
			get;
		}

		public int LineCount => lines.Count;

		public Response(string _name)
		{
			Name = _name;
		}

		public void AddLine(string _text)
		{
			lines.Add(_text);
		}

		public string GetRandomLine()
		{
			return lines[random.Next(0, lines.Count)];
		}
	}
}
