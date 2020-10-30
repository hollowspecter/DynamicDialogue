using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicDialogue
{
	/// <summary>
	/// TODO
	/// </summary>
	public abstract class Clause
	{
		public abstract bool Check(IVariableStorage _storage);
	}

	/// <summary>
	/// TODO
	/// </summary>
	public class StringClause : Clause
	{
		private readonly string key;
		private readonly string compareToValue;

		public StringClause(string _key, string _compareToValue)
		{
			key = _key;
			compareToValue = _compareToValue;
		}

		public override bool Check(IVariableStorage _storage)
		{
			if (_storage.TryGetValue(key, out string result) &&
				result.Equals(compareToValue))
				return true;
			else
				return false;
		}
	}

	/// <summary>
	/// TODO
	/// </summary>
	public class ValueClause : Clause
	{
		public enum CompareMode
		{
			LESS_THAN,
			GREATER_THAN,
			EQUAL_TO
		}

		private readonly float minValue = float.MinValue;
		private readonly float maxValue = float.MaxValue;
		private readonly string key;

		public ValueClause(string _key, CompareMode _mode, float _compareToValue)
		{
			key = _key;

			switch (_mode)
			{
				case CompareMode.LESS_THAN:
					maxValue = _compareToValue;
					break;
				case CompareMode.GREATER_THAN:
					minValue = _compareToValue;
					break;
				case CompareMode.EQUAL_TO:
					minValue = _compareToValue;
					maxValue = _compareToValue;
					break;
			}
		}

		public override bool Check(IVariableStorage _storage)
		{
			if (_storage.TryGetValue(key, out float value) &&
				value >= minValue - float.Epsilon &&
				value <= maxValue + float.Epsilon)
				return true;
			else
				return false;
		}
	}
}
