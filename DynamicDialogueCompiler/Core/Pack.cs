using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicDialogueTest")]
namespace DynamicDialogue.Core
{
	/// <summary>
	/// A file compiles into a pack (of dogs).
	/// A pack consists of rules and responses.
	/// </summary>
	internal class Pack
	{
		private List<Rule> rules = new List<Rule>();
		private Dictionary<string, Response> responses = new Dictionary<string, Response>();

		public int RuleCount => rules.Count;
		public int ReponseCount => responses.Count;

		/// <summary>
		/// Adds a new rule to the pack.
		/// Uses InsertionSort to keep the pack sorted by the number of
		/// conditions it has, descending.
		/// </summary>
		/// <param name="rule"></param>//TODO
		public void AddRule(Rule rule)
		{
			int currentConditionCount = rule.ConditionCount;
			for (int i = 0; i < rules.Count; ++i)
			{
				if (currentConditionCount > rules[i].ConditionCount)
				{
					rules.Insert(i, rule);
					return;
				}
			}

			rules.Add(rule);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Rule GetRule(int index)
		{
			return rules[index];
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="response"></param>
		public void AddResponse(Response response)
		{
			responses.Add(response.Name, response);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="query"></param>
		public void Match(IVariableStorage query)
		{
			throw new NotImplementedException();
		}
	}
}
