using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicDialogueTest")]
[assembly: InternalsVisibleTo("DynamicDialogueTest2")]
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
		/// <summary>
		/// Sorted list of conditions by key
		/// </summary>
		private List<Clause> conditions = new List<Clause>();
		private List<Consequence> consequences = new List<Consequence>();

		public int ConditionCount => conditions.Count;

		struct QueryPointer
		{
			public int currentIndex;
			public int queryIndex;

			public QueryPointer(int currentIndex, int queryIndex)
			{
				this.currentIndex = currentIndex;
				this.queryIndex = queryIndex;
			}
		}

		/// <summary>
		/// Checks all the conditions against the query.
		/// Assuming every query is sorted, we just have to iterate
		/// through all queries simultaneously.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public bool Check(IVariableStorage[] query)
		{
			//TODO more tests to see if it works as intended
			List<QueryPointer> queryPointers = new List<QueryPointer>();
			int conditionIndex = 0;
			bool inserted;

			// Setup arrays
			for (int queryIndex = 0; queryIndex < query.Length; ++queryIndex)
			{
				if (query[queryIndex].Count == 0)
					continue;

				inserted = false;
				for (int i = 0; i < queryPointers.Count; ++i)
				{
					if (query[queryIndex][0].CompareTo(query[queryPointers[i].queryIndex][0]) < 0)
					{
						queryPointers.Insert(i, new QueryPointer(0, queryIndex));
						inserted = true;
						break;
					}
				}
				if (!inserted)
				{
					queryPointers.Add(new QueryPointer(0, queryIndex));
				}
			}

			// iterate through it
			while (queryPointers.Count > 0)
			{
				// take the next element
				var nextElement = queryPointers[0];
				queryPointers.RemoveAt(0);

				// check against the current condition or skip if not close enough yet
				string currentKey = query[nextElement.queryIndex][nextElement.currentIndex];
				int compareTo = currentKey.CompareTo(conditions[conditionIndex].key);
				// if the key is equal, do the check
				if (compareTo == 0)
				{
					if (query[nextElement.queryIndex].TryGetValue<object>(currentKey, out var value) &&
						conditions[conditionIndex].Check(value))
					{
						conditionIndex++;
						// check for exit criteria
						if (conditionIndex == conditions.Count)
							return true;
					}
					else
					{
						// check for exit criteria
						return false;
					}
				}
				// check for exit criteria
				else if (compareTo > 0)
				{
					return false;
				}
				// if compareTo is smaller than 0, just keep looking

				// fill up the next element
				nextElement.currentIndex++;
				if (query[nextElement.queryIndex].Count > nextElement.currentIndex)
				{
					// sort insert it back
					inserted = false;
					for (int i = 0; i < queryPointers.Count; ++i)
					{
						if (query[nextElement.queryIndex][nextElement.currentIndex].CompareTo(query[nextElement.queryIndex][nextElement.currentIndex]) < 0)
						{
							queryPointers.Insert(i, nextElement);
							inserted = true;
						}
					}
					if (!inserted)
						queryPointers.Add(nextElement);
				}
			}

			return false;
		}

		public void Execute(Machine machine)
		{
			for (int i = 0; i < consequences.Count; ++i)
			{
				consequences[i].Execute(machine);
			}
		}

		public Rule AddCondition(Clause clause)
		{
			// Insert sort
			for (int i = 0; i < conditions.Count; ++i)
			{
				if (clause.key.CompareTo(conditions[i].key) < 0)
				{
					conditions.Insert(i, clause);
					return this;
				}
			}

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
