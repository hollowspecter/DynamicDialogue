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
		private List<Response> responseList = new List<Response>();
		private Dictionary<string, Response> responses = new Dictionary<string, Response>();

		public int RuleCount => rules.Count;
		public int ReponseCount => responses.Count;
		public string Name
		{
			get; private set;
		}

		public Pack(string name)
		{
			Name = name;
		}

		/// <summary>
		/// Adds a new rule to the pack.
		/// Uses InsertionSort to keep the pack sorted by the number of
		/// conditions it has, descending.
		/// </summary>
		/// <param name="rule"></param>//TODO
		public Pack AddRule(Rule rule)
		{
			int currentConditionCount = rule.ConditionCount;
			for (int i = 0; i < rules.Count; ++i)
			{
				if (currentConditionCount > rules[i].ConditionCount)
				{
					rules.Insert(i, rule);
					return this;
				}
			}

			rules.Add(rule);
			return this;
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
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="rule"></param>
		/// <returns></returns>
		public bool TryGetRule(int index, out Rule rule)
		{
			if (index < 0 ||
				index >= rules.Count)
			{
				rule = null;
				return false;
			}
			rule = rules[index];
			return true;
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Response GetResponse(int index)
		{
			return responseList[index];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool TryGetResponse(string id, out Response response)
		{
			return responses.TryGetValue(id, out response);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="response"></param>
		public Pack AddResponse(Response response)
		{
			responses.Add(response.Name, response);
			responseList.Add(response);
			return this;
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
