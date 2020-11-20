using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicDialogueTest")]
[assembly: InternalsVisibleTo("DynamicDialogueTest2")]
namespace DynamicDialogue.Core
{
	/// <summary>
	/// A condition clause takes a <see cref="IVariableStorage"/>
	/// and can be checked against ist.
	/// </summary>
	internal abstract class Clause
	{
		public readonly string key;

		public Clause(string key)
		{
			this.key = key;
		}

		public abstract bool Check(object value);
	}

	/// <summary>
	/// Checks, if a certain key is set in the <see cref="IVariableStorage"/>
	/// </summary>
	internal class ExistsClause : Clause
	{
		public ExistsClause(string key) : base(key) { }

		public override bool Check(object value)
		{
			return true;
		}
	}

	/// <summary>
	/// Checks if a string value is set in the <see cref="IVariableStorage"/>
	/// </summary>
	internal class StringClause : Clause
	{
		private readonly string compareToValue;

		public StringClause(string key, string compareToValue) : base(key)
		{
			this.compareToValue = compareToValue;
		}

		public override bool Check(object value)
		{
			return value is string &&
				(string)value == compareToValue;
		}
	}

	/// <summary>
	/// Checks if a certain bool value is set in the <see cref="IVariableStorage"/>
	/// </summary>
	internal class BoolClause : Clause
	{
		private readonly bool compareValue;

		public BoolClause(string key, bool compareValue) : base(key)
		{
			this.compareValue = compareValue;
		}

		public override bool Check(object value)
		{
			return value is bool && (bool)value == compareValue;
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

		public FloatClause(string key, CompareMode mode, float compareToValue) : base(key)
		{
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

		public override bool Check(object value)
		{
			return (value is float &&
				(float)value >= minValue - float.Epsilon &&
				(float)value <= maxValue + float.Epsilon);
		}
	}
}
