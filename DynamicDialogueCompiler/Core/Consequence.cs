using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicDialogueTest")]
namespace DynamicDialogue.Core
{
	/// <summary>
	/// Consequences get executed, when a rule gets executed.
	/// </summary>
	internal abstract class Consequence
	{
		public abstract void Execute(Machine machine);
	}

	/// <summary>
	/// A StorageChange is a type of <see cref="Consequence"/> that
	/// changes the referenced <see cref="IVariableStorage"/>
	/// </summary>
	internal class StorageChange : Consequence
	{
		public delegate void StorageChangeHandler(IReadOnlyDictionary<string, object> changes);
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

		public override void Execute(Machine machine)
		{
			machine.StorageChangeHandler.Invoke(changes);
		}
	}

	/// <summary>
	/// Consequence that outputs text
	/// </summary>
	internal class TextResponse : Consequence
	{
		public delegate void TestResponseHandler(string responseId);
		private string responseId;

		public TextResponse(string responseId)
		{
			this.responseId = responseId;
		}

		public override void Execute(Machine machine)
		{
			machine.TextResponseHandler.Invoke(responseId);
		}
	}

	internal struct Trigger
	{
		public Trigger(string to, string conceptName)
		{
			To = to;
			ConceptName = conceptName;
		}

		public string To
		{
			get; private set;
		}

		public string ConceptName
		{
			get; private set;
		}
	}

	/// <summary>
	/// Consequence, that triggers a response to the current text
	/// </summary>
	internal class TriggerResponse : Consequence
	{
		public delegate void TriggerResponseHandler(Trigger trigger);

		private Trigger trigger;

		public TriggerResponse(string to, string conceptName)
		{
			trigger = new Trigger(to, conceptName);
		}

		public override void Execute(Machine machine)
		{
			machine.TriggerResponseHandler.Invoke(trigger);
		}
	}
}
