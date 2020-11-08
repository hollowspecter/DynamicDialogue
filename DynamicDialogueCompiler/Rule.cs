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
			for (int i = 0; i < conditions.Count; ++i)
			{
				if (conditions[i].Check(_query) == false)
					return false;
			}
			return true;
		}

		public void Execute(IVariableStorage _query)
		{
			for (int i = 0; i < consequences.Count; ++i)
			{
				consequences[i].Execute(_query);
			}
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
