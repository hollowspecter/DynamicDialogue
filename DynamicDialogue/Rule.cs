using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicDialogue
{
	/// <summary>
	/// TODO
	/// </summary>
	public class Rule
	{
		private List<Clause> conditions = new List<Clause>();
		private List<Consequence> consequences = new List<Consequence>();

		public bool Check(IVariableStorage _query)
		{
			throw new NotImplementedException();
		}

		public bool Execute(IVariableStorage _query)
		{
			throw new NotImplementedException();
		}
		public Rule AddCondition(Clause _clause)
		{
			conditions.Add(_clause);
			return this;
		}

		public Rule AddConsequence(Consequence _consequence)
		{
			consequences.Add(_consequence);
			return this;
		}
	}
}
