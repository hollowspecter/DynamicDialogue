using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicDialogueTest")]
namespace DynamicDialogue.Core
{
	/// <summary>
	/// A condition clause takes a <see cref="IVariableStorage"/>
	/// and can be checked against ist.
	/// </summary>
	internal abstract class Clause
	{
		public abstract bool Check(IVariableStorage storage);
	}

	/// <summary>
	/// Checks, if a certain key is set in the <see cref="IVariableStorage"/>
	/// </summary>
	internal class ExistsClause : Clause
	{
		private readonly string key;

		public ExistsClause(string _key)
		{
			key = _key;
		}

		public override bool Check(IVariableStorage storage)
		{
			return storage.TryGetValue<object>(key, out _);
		}
	}

	/// <summary>
	/// Checks if a string value is set in the <see cref="IVariableStorage"/>
	/// </summary>
	internal class StringClause : Clause
	{
		private readonly string key;
		private readonly string compareToValue;

		public StringClause(string key, string compareToValue)
		{
			this.key = key;
			this.compareToValue = compareToValue;
		}

		public override bool Check(IVariableStorage storage)
		{
			if (storage.TryGetValue(key, out string result) &&
				result.Equals(compareToValue))
				return true;
			else
				return false;
		}
	}

	/// <summary>
	/// Checks if a certain bool value is set in the <see cref="IVariableStorage"/>
	/// </summary>
	internal class BoolClause : Clause
	{
		private readonly string key;
		private readonly bool compareValue;

		public BoolClause(string key, bool compareValue)
		{
			this.key = key;
			this.compareValue = compareValue;
		}

		public override bool Check(IVariableStorage storage)
		{
			return storage.TryGetValue(key, out bool result) && result == compareValue;
		}
	}

	/// <summary>
	/// Checks if a certain float value is set in the <see cref="IVariableStorage"/>
	/// </summary>
	internal class FloatClause : Clause
	{
		public enum CompareMode
		{
			EQUAL_OR_LESS_THAN,
			EQUAL_OR_GREATER_THAN,
			EQUAL_TO
		}

		private readonly float minValue = float.MinValue;
		private readonly float maxValue = float.MaxValue;
		private readonly string key;

		public FloatClause(string key, CompareMode mode, float compareToValue)
		{
			this.key = key;

			switch (mode)
			{
				case CompareMode.EQUAL_OR_LESS_THAN:
					maxValue = compareToValue;
					break;
				case CompareMode.EQUAL_OR_GREATER_THAN:
					minValue = compareToValue;
					break;
				case CompareMode.EQUAL_TO:
					minValue = compareToValue;
					maxValue = compareToValue;
					break;
			}
		}

		public override bool Check(IVariableStorage storage)
		{
			if (storage.TryGetValue(key, out float value) &&
				value >= minValue - float.Epsilon &&
				value <= maxValue + float.Epsilon)
				return true;
			else
				return false;
		}
	}
}
