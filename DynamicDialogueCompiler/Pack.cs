using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicDialogue
{
	/// <summary>
	/// A file compiles into a pack (of dogs).
	/// A pack consists of rules and responses.
	/// </summary>
	public class Pack
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
		/// <param name="_rule"></param>
		public void AddRule(Rule _rule)
		{
			int currentConditionCount = _rule.ConditionCount;
			for (int i = 0; i < rules.Count; ++i)
			{
				if (currentConditionCount > rules[i].ConditionCount)
				{
					rules.Insert(i, _rule);
					return;
				}
			}

			rules.Add(_rule);
		}

		public Rule GetRule(int _index)
		{
			return rules[_index];
		}

		public void AddResponse(Response _response)
		{
			responses.Add(_response.Name, _response);
		}

		public void Match(IVariableStorage query)
		{
			throw new NotImplementedException();
		}
	}
}
