using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DynamicDialogue
{
	/// <summary>
	/// TODO
	/// </summary>
	public abstract class Consequence
	{
		public abstract void Execute(IVariableStorage storage);
	}

	/// <summary>
	/// TODO
	/// </summary>
	public class StorageChange : Consequence
	{
		private Dictionary<string, object> changes = new Dictionary<string, object>();

		public void AddChange(string key, bool value)
		{
			changes.Add(key, value);
		}

		public void AddChange(string key, float value)
		{
			changes.Add(key, value);
		}

		public void AddChange(string key, string value)
		{
			changes.Add(key, value);
		}

		public override void Execute(IVariableStorage storage)
		{
			foreach (var change in changes)
			{
				if (change.Value is bool)
					storage.SetValue(change.Key, (bool)change.Value);
				else if (change.Value is float)
					storage.SetValue(change.Key, (float)change.Value);
				else if (change.Value is string)
					storage.SetValue(change.Key, (string)change.Value);
			}
		}
	}

	/// <summary>
	/// TODO
	/// </summary>
	public class TextResponse : Consequence
	{
		private List<string> lines = new List<string>();
		private Random random = new Random();

		public void AddLine(string _text)
		{
			lines.Add(_text);
		}

		private string GetRandomLine()
		{
			return lines[random.Next(0, lines.Count)];
		}

		/// <summary>
		/// Should talk the line.
		/// Maybe with events? Database with "Talkers" who will then "Talk()"?
		/// </summary>
		public override void Execute(IVariableStorage storage)
		{
			Trace.WriteLine(GetRandomLine());
		}
	}
}
