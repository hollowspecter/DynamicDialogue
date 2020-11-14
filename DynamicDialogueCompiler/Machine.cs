using DynamicDialogue.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicDialogue
{
	/// <summary>
	/// Load and unload packs.
	/// Can merge new packs with old packs together.
	/// Accepts Queries and returns the best rule and the best response
	/// depending on the packs currently loaded
	/// </summary>
	public class Machine
	{
		internal enum LoadStatus
		{
			New,
			Additive,
			Failure
		}

		private Dictionary<string, Pack> packDictionairy = new Dictionary<string, Pack>();
		private List<Pack> packs = new List<Pack>();

		/// <summary>
		/// Loads a new pack into memory.
		/// If the key already exists, it will merge them.
		/// </summary>
		/// <param name="key">The key for the pack. Remember this to unload it.</param>
		/// <param name="pack">The pack to load</param>
		/// <returns>The appropriate status.
		/// New for successfully added packs with the key.
		/// Additive means the key already existed and the pack was additively added
		/// under the same key. Failure, when something went wrong (pack was null?)</returns>
		internal LoadStatus LoadPack(Pack pack)
		{
			// Check for traps
			if (pack == null ||
				(pack.ReponseCount == 0 && pack.RuleCount == 0) ||
				string.IsNullOrEmpty(pack.Name))
			{
				return LoadStatus.Failure;
			}
			// Add additively
			else if (packDictionairy.ContainsKey(pack.Name))
			{
				packDictionairy.TryGetValue(pack.Name, out var originalPack);

				// add all the rules
				int ruleCount = pack.RuleCount; //cache here because else it results in infinite loops when the originalPack = pack
				int responseCount = pack.ReponseCount;
				for (int i = 0; i < ruleCount; ++i)
					originalPack.AddRule(pack.GetRule(i));

				// add all the responses
				for (int i = 0; i < responseCount; ++i)
					originalPack.AddResponse(pack.GetResponse(i));

				return LoadStatus.Additive;
			}
			//Add newly
			else
			{
				packDictionairy.Add(pack.Name, pack);
				packs.Add(pack);
				return LoadStatus.New;
			}
		}

		/// <summary>
		/// Tries to unload a pack. Fails if the key
		/// is not registered with a pack.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		internal bool TryUnloadPack(string key)
		{
			if (packDictionairy.TryGetValue(key, out Pack pack))
			{
				packs.Remove(pack);
				packDictionairy.Remove(key);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Flushes all the loaded packs.
		/// </summary>
		internal void UnloadAllPacks()
		{
			packs.Clear();
			packDictionairy.Clear();
		}

		/// <summary>
		/// Returns the best suited Rule.
		/// A rule has to macth all conditions.
		/// If several rules match all, the one with the highest
		/// number of conditions is returned.
		/// If there are several with the same number of conditions,
		/// the first one to find is being returned.
		/// </summary>
		/// <param name="query">The storage containing all data for the query</param>
		/// <param name="rule">On return, contains the best rule.</param>
		/// <returns>True, when a rule was successfully matched, false if not.</returns>
		internal bool TryQueryRule(IVariableStorage query, out Rule rule)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns the reponse by matching it against the reponseId.
		/// </summary>
		/// <param name="reponseId">The reponseId. Needs to match with <see cref="Response.Name"/></param>
		/// <param name="response">On return, contains the corresponding response</param>
		/// <returns>True, if the reponse is successfully found, false if not</returns>
		internal bool TryQueryResponse(string reponseId, out Response response)
		{
			for (int i = 0; i < packs.Count; ++i)
			{
				if (packs[i].TryGetResponse(reponseId, out response))
				{
					return true;
				}
			}
			response = null;
			return false;
		}
	}
}
