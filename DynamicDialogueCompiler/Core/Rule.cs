using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicDialogueTest")]
namespace DynamicDialogue.Core
{
	/// <summary>
	/// Consists of conditions (<see cref="Clause"/>) and <see cref="Consequence"/>s.
	/// A rule can be matched against a query (<see cref="IVariableStorage"/>),
	/// and can be executed.
	/// Execution of a rule results in executing all the consequences consequently.
	/// </summary>
	internal class Rule
	{
		private List<Clause> conditions = new List<Clause>();
		private List<Consequence> consequences = new List<Consequence>();

		public int ConditionCount => conditions.Count;

		public bool Check(IVariableStorage query)
		{
			for (int i = 0; i < conditions.Count; ++i)
			{
				if (conditions[i].Check(query) == false)
					return false;
			}
			return true;
		}

		public void Execute(IVariableStorage query)
		{
			for (int i = 0; i < consequences.Count; ++i)
			{
				consequences[i].Execute(query);
			}
		}

		public Rule AddCondition(Clause clause)
		{
			conditions.Add(clause);
			return this;
		}

		public Rule AddConsequence(Consequence consequence)
		{
			consequences.Add(consequence);
			return this;
		}
	}
}
