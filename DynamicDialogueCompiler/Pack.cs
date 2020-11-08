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

		//TODO add a function that sorts all rules by number of conditions
		//TODO or even better, make insertion sort and always insert where the same number of conditions is met
		public void AddRule(Rule _rule)
		{
			rules.Add(_rule);
		}

		public void AddResponse(Response _response)
		{
			responses.Add(_response.Name, _response);
		}

		/// <summary>
		/// Sorts the rules by the number of conditions
		/// </summary>
		public void SortRules()
		{
			throw new NotImplementedException();
		}

		public void Match(IVariableStorage query)
		{
			throw new NotImplementedException();
		}
	}
}
