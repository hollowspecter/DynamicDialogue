using NUnit.Framework;
using DynamicDialogue;
using Antlr4;
using Antlr4.Runtime;
using System.IO;
using DynamicDialogue.Compiler;
using DynamicDialogue.Core;
using System.Diagnostics;
using static DynamicDialogue.Test.TestFiles;

namespace DynamicDialogueTest
{
	public class DialogueTests
	{
		[Test]
		public void TestAddPack_Failure_Null()
		{
			Dialogue dialogue = new Dialogue()
			{
				LogErrorMessage = message =>
				{
					Assert.AreEqual("Failed to load pack because the provided pack was null", message);
				}
			};

			Assert.Throws<DialogueException>(() => dialogue.AddPack(null));
		}

		[Test]
		public void TestAddPack_Failure_Empty()
		{
			Dialogue dialogue = new Dialogue()
			{
				LogErrorMessage = message =>
				{
					Assert.AreEqual("Failed to load pack 1 because it was empty", message);
				}
			};

			Assert.Throws<DialogueException>(() => dialogue.AddPack(new Pack("1")));
		}

		[TestCase("", ExpectedResult = "Failed to load pack because its name was null or empty")]
		[TestCase(null, ExpectedResult = "Failed to load pack because its name was null or empty")]
		public string TestAddPack_Failure_EmptyName(string packName)
		{
			string result = "";
			Dialogue dialogue = new Dialogue()
			{
				LogErrorMessage = message =>
				{
					result = message;
				}
			};

			Assert.Throws<DialogueException>(() =>
				dialogue.AddPack(new Pack("")
				.AddResponse(new Response("response"))));

			return result;
		}

		[Test]
		public void TestAddPack_Additive()
		{
			Dialogue dialogue = new Dialogue();
			string packName = "asd";
			dialogue.AddPack(new Pack(packName).AddResponse(new Response("1")));

			dialogue.LogDebugMessage = message =>
			{
				Assert.AreEqual("Pack asd was successfully loaded additively.", message);
			};

			Assert.AreEqual(packName, dialogue.AddPack(new Pack(packName).AddResponse(new Response("2"))));
		}

		[Test]
		public void TestAddPack_New()
		{
			Dialogue dialogue = new Dialogue()
			{
				LogDebugMessage = message =>
				{
					Assert.AreEqual("Pack asd was successfully loaded!", message);
				}
			};
			string packName = "asd";
			Assert.AreEqual(packName, dialogue.AddPack(new Pack(packName).AddResponse(new Response("1"))));
		}

		[Test]
		public void TestUnloadPack_Success()
		{
			string key = "asd";
			Dialogue dialogue = new Dialogue();
			dialogue.AddPack(new Pack(key).AddResponse(new Response("1")));

			dialogue.LogDebugMessage = message =>
				Assert.AreEqual($"Successfully removed the pack {key}", message);
		}

		[Test]
		public void TestUnloadPack_Fail()
		{
			string key = "asd";
			Dialogue dialogue = new Dialogue();
			dialogue.LogErrorMessage = message => Assert.AreEqual($"Was not able to remove the pack {key}, are you sure it was loaded?", message);
			Assert.Throws<DialogueException>(() => dialogue.UnloadPack(key));
		}

		[Test]
		public void TestLoadProgram_Success()
		{
			Dialogue dialogue = new Dialogue();
			dialogue.LoadProgram(TestDataPath + DogTalk);
		}

		[TestCase("")]
		[TestCase(null)]
		[TestCase("wrongName")]
		public void TestLoadProgram_Fail(string fileName)
		{
			Dialogue dialogue = new Dialogue();
			Assert.Throws<DialogueException>(() => dialogue.LoadProgram(""));
		}

		[Test]
		public void TestQuery_Error()
		{
			Dialogue dialogue = new Dialogue()
			{
				LogErrorMessage = message =>
				{
					Assert.AreEqual("Query was null, failed", message);
				}
			};
			dialogue.Query(null);
		}

		[Test]
		public void TestQuery_NotFound()
		{
			IVariableStorage query = new MemoryVariableStorage();
			query.SetValue("key", true);
			Dialogue dialogue = new Dialogue()
			{
				LogDebugMessage = message => Assert.AreEqual("No rule found", message)
			};
		}

		[Test]
		public void TestQuery_Success()
		{
			IVariableStorage query = new MemoryVariableStorage();
			string key = "key";
			query.SetValue(key, true);
			Dialogue dialogue = new Dialogue();
			dialogue
				.AddPack(new Pack("packname")
				.AddRule(new Rule()
				.AddCondition(new ExistsClause(key))));
			dialogue.Query(query);
		}

		[Test]
		public void TestGetText_Success()
		{
			string responseId = "doggo";
			string line = "barko barko";
			Dialogue dialogue = new Dialogue();
			dialogue.AddPack(new Pack("packname")
				.AddResponse(new Response(responseId)
				.AddLine(line)));
			Assert.AreEqual(line, dialogue.GetText(responseId));
		}

		[TestCase("")]
		[TestCase(null)]
		[TestCase("not there")]
		public void TestGetText_Fail(string responseId)
		{
			string line = "barko barko";
			Dialogue dialogue = new Dialogue();
			dialogue.AddPack(new Pack("packname")
				.AddResponse(new Response(responseId ?? "asd")
				.AddLine(line)));
			dialogue.LogErrorMessage = message =>
			{
				Assert.AreEqual($"Response was not found {responseId}", message);
			};
		}
	}
}
