using System;
using System.Collections.Generic;

namespace DynamicDialogue.Core
{
	/// <summary>
	/// Provides a mechanism for storing and retrieving instances of the <see cref="Value"/> class.
	/// </summary>
	public interface IVariableStorage
	{
		/// <summary>
		/// Must return sorted key.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		string this[int index] { get; }
		int Count
		{
			get;
		}

		void SetValue(string variableName, string stringValue);
		void SetValue(string variableName, float floatValue);
		void SetValue(string variableName, bool boolValue);
		bool TryGetValue<T>(string variableName, out T result);
		void GetAllValues(out IReadOnlyDictionary<string, object> values);
		void Clear();
	}

	/// <summary>
	/// A simple concrete implementation of <see cref="IVariableStorage"/> that keeps all variables in memory
	/// </summary>
	public class MemoryVariableStorage : IVariableStorage
	{
		// TODO move those somewhere else?
		public static string Is = "Is";
		public static string From = "From";

		private SortedList<string, object> variables = new SortedList<string, object>();

		public string this[int index] => variables.Keys[index];//TODO test if this works
		public int Count => variables.Count;//TODO test this property

		/// <inheritdoc/>
		public void SetValue(string variableName, string stringValue)
		{
			variables[variableName] = stringValue;
		}

		/// <inheritdoc/>
		public void SetValue(string variableName, float floatValue)
		{
			variables[variableName] = floatValue;
		}

		/// <inheritdoc/>
		public void SetValue(string variableName, bool boolValue)
		{
			variables[variableName] = boolValue;
		}

		/// <inheritdoc/>
		public bool TryGetValue<T>(string variableName, out T result)
		{
			if (variables.TryGetValue(variableName, out var foundValue))
			{
				if (typeof(T).IsAssignableFrom(foundValue.GetType()))
				{
					result = (T)foundValue;
					return true;
				}
				else
				{
					throw new ArgumentException($"Variable {variableName} is present, but is of type {foundValue.GetType()}, not {typeof(T)}");
				}
			}

			result = default;
			return false;
		}

		/// <inheritdoc/>
		public void Clear()
		{
			variables.Clear();
		}

		/// <inheritdoc/>
		public void GetAllValues(out IReadOnlyDictionary<string, object> values)
		{
			values = variables;
		}
	}
}
