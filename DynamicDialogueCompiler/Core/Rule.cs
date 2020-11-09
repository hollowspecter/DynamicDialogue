using System.Collections.Generic;

namespace DynamicDialogue.Core
{
	/// <summary>
	/// Consists of conditions (<see cref="Clause"/>) and <see cref="Consequence"/>s.
	/// A rule can be matched against a query (<see cref="IVariableStorage"/>),
	/// and can be executed.
	/// Execution of a rule results in executing all the consequences consequently.
	/// </summary>
	public class Rule
	{
		private List<Clause> conditions = new List<Clause>();
		private List<Consequence> consequences = new List<Consequence>();

		public int ConditionCount => conditions.Count;

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
