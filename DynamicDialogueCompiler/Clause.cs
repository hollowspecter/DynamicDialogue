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
	public class ExistsClause : Clause
	{
		private readonly string key;

		public ExistsClause(string _key)
		{
			key = _key;
		}

		public override bool Check(IVariableStorage _storage)
		{
			return _storage.TryGetValue<object>(key, out _);
		}
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
	public class BoolClause : Clause
	{
		private readonly string key;
		private readonly bool compareValue;

		public BoolClause(string _key, bool _compareValue)
		{
			key = _key;
			compareValue = _compareValue;
		}

		public override bool Check(IVariableStorage _storage)
		{
			return _storage.TryGetValue(key, out bool result) && result == compareValue;
		}
	}

	/// <summary>
	/// TODO
	/// </summary>
	public class FloatClause : Clause
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

		public FloatClause(string _key, CompareMode _mode, float _compareToValue)
		{
			key = _key;

			switch (_mode)
			{
				case CompareMode.EQUAL_OR_LESS_THAN:
					maxValue = _compareToValue;
					break;
				case CompareMode.EQUAL_OR_GREATER_THAN:
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
