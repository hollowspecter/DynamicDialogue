using System;
using System.Collections.Generic;
using System.Text;
using DynamicDialogue.Core;
using DynamicDialogue.Compiler;

namespace DynamicDialogue
{
	/// <summary>
	/// Represents a method that receives diagnostic messages and error
	/// information from a <see cref="Dialogue"/>
	/// </summary>
	/// <param name="message">The text to log</param>
	public delegate void Logger(string message);

	/// <summary>
	/// Co-ordinates the querying and execution for dialogue.
	/// </summary>
	public class Dialogue
	{
		/// <summary>
		/// Invoked when the Dialogue needs to report debugging information
		/// </summary>
		public Logger LogDebugMessage
		{
			get; set;
		}

		/// <summary>
		/// Invoked when the Dialogue needs to report error information
		/// </summary>
		public Logger LogErrorMessage
		{
			get; set;
		}

		private Machine machine;

		/// <summary>
		/// TODO
		/// </summary>
		public StorageChangeHandler StorageChangeHandler => machine.StorageChangeHandler;
		/// <summary>
		/// 
		/// </summary>
		public TextResponseHandler TextResponseHandler => machine.TextResponseHandler;
		/// <summary>
		/// 
		/// </summary>
		public TriggerResponseHandler TriggerResponseHandler => machine.TriggerResponseHandler;

		public Dialogue()
		{
			machine = new Machine();
		}

		internal string AddPack(Pack pack)
		{
			var status = machine.LoadPack(pack);

			if (status == Machine.LoadStatus.Failure)
			{
				string reason = "";
				if (pack == null)
					reason = "because the provided pack was null";
				else if (pack.ReponseCount == 0 && pack.RuleCount == 0)
					reason = $"{pack.Name} because it was empty";
				else if (string.IsNullOrEmpty(pack.Name))
					reason = $"because its name was null or empty";
				LogErrorMessage?.Invoke($"Failed to load pack {reason}");
				throw new DialogueException($"Failed to load pack.");
			}
			else if (status == Machine.LoadStatus.Additive)
			{
				LogDebugMessage?.Invoke($"Pack {pack.Name} was successfully loaded additively.");
			}
			else if (status == Machine.LoadStatus.New)
			{
				LogDebugMessage?.Invoke($"Pack {pack.Name} was successfully loaded!");
			}

			return pack.Name;
		}

		public void UnloadPack(string key)
		{
			bool result = machine.TryUnloadPack(key);

			if (result)
			{
				LogDebugMessage?.Invoke($"Successfully removed the pack {key}");
			}
			else
			{
				LogErrorMessage?.Invoke($"Was not able to remove the pack {key}, are you sure it was loaded?");
				throw new DialogueException($"Failed to unload pack with key {key}");
			}
		}

		public string LoadProgram(string fileName)
		{
			try
			{
				Compiler.Compiler.CompileFile(fileName, out Pack pack);
				return AddPack(pack);
			}
			catch (ParseException e)
			{
				LogErrorMessage?.Invoke(e.Message);
			}
			catch (ArgumentException e)
			{
				LogErrorMessage?.Invoke(e.Message);
			}
			throw new DialogueException($"File {fileName} was not able to be compiled.");
		}

		public void UnloadAll()
		{
			machine.UnloadAllPacks();
		}

		public void Query(IVariableStorage query)
		{
			if (query == null)
			{
				LogErrorMessage?.Invoke("Query was null, failed");
				return;
			}

			if (machine.TryQueryRule(query, out var rule))
			{
				rule.Execute(machine);
			}
			else
			{
				LogDebugMessage?.Invoke("No rule found");
			}
		}

		public string GetText(string responseId)
		{
			if (responseId != null &&
				machine.TryQueryResponse(responseId, out Response response))
			{
				return response.GetRandomLine();
			}
			else
			{
				LogErrorMessage?.Invoke($"Response was not found {responseId}");
				throw new DialogueException($"Response not found {responseId}");
			}
		}
	}
}
