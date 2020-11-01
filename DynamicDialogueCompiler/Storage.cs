using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicDialogue
{
	/// <summary>
	/// Provides a mechanism for storing and retrieving instances of the <see cref="Value"/> class.
	/// </summary>
	public interface IVariableStorage
	{
		public static string Is = "Is";
		public static string From = "From";

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
		private Dictionary<string, object> variables = new Dictionary<string, object>();

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
