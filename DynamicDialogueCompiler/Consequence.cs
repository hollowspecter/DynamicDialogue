using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;

namespace DynamicDialogue
{
	/// <summary>
	/// Consequences get executed, when a rule gets executed.
	/// </summary>
	public abstract class Consequence
	{
		public abstract void Execute(IVariableStorage storage);
	}

	/// <summary>
	/// A StorageChange is a type of <see cref="Consequence"/> that
	/// changes the referenced <see cref="IVariableStorage"/>
	/// </summary>
	public class StorageChange : Consequence
	{
		private Dictionary<string, object> changes = new Dictionary<string, object>();

		public StorageChange AddChange(string key, bool value)
		{
			changes.Add(key, value);
			return this;
		}

		public StorageChange AddChange(string key, float value)
		{
			changes.Add(key, value);
			return this;
		}

		public StorageChange AddChange(string key, string value)
		{
			changes.Add(key, value);
			return this;
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
	/// Consequence that outputs text
	/// 
	/// TODO: how does the text response trigger something
	///		  that actually can be implemented very differently?
	/// </summary>
	public class TextResponse : Consequence
	{
		private string responseId;

		public TextResponse(string _responseId)
		{
			responseId = _responseId;
		}

		public override void Execute(IVariableStorage storage)
		{
			Trace.WriteLine(responseId);
		}
	}

	/// <summary>
	/// Consequence, that triggers a response to the current text
	/// 
	/// TODO: how does the text response trigger something
	///	      that actually can be implemented very differently?
	/// </summary>
	public class TriggerResponse : Consequence
	{
		private string from;
		private string to;
		private string conceptName;

		public TriggerResponse(string _to, string _conceptName)
		{
			to = _to;
			conceptName = _conceptName;
		}

		public override void Execute(IVariableStorage storage)
		{
			storage.TryGetValue(IVariableStorage.From, out from);
			Trace.WriteLine($"Trigger executed. From = {from}, To = {to}, conceptName = {conceptName}");
		}
	}
}
