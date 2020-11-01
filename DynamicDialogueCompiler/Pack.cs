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
		//TODO add a function that sorts all rules by number of conditions
		private List<Rule> rules = new List<Rule>();
		private Dictionary<string, Response> responses = new Dictionary<string, Response>();

		public void AddRule(Rule _rule)
		{
			rules.Add(_rule);
		}

		public void AddResponse(Response _response)
		{
			responses.Add(_response.Name, _response);
		}
	}
}
