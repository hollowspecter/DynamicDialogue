using DynamicDialogue;
using DynamicDialogue.Core;
using NUnit.Framework;

namespace DynamicDialogueTest
{
	public class MachineTests
	{
		private Pack pack1;
		private Pack pack2;
		private Pack pack3_sameNameAs1;
		private Rule rule_oneCondition;
		private Rule rule_twoConditions;
		private Rule rule_threeConditions;
		private Response response1;
		private Response response2;
		private readonly string existsClauseKey = "existsClauseKey";
		private readonly string existsClauseKey2 = "existsClauseKey2";
		private readonly string existsClauseKey3 = "existsClauseKey3";

		[SetUp]
		public void Setup()
		{
			// Setup rules
			rule_oneCondition = new Rule()
				.AddCondition(new ExistsClause(existsClauseKey));
			rule_twoConditions = new Rule()
				.AddCondition(new ExistsClause(existsClauseKey))
				.AddCondition(new ExistsClause(existsClauseKey2));
			rule_threeConditions = new Rule()
				.AddCondition(new ExistsClause(existsClauseKey))
				.AddCondition(new ExistsClause(existsClauseKey2))
				.AddCondition(new ExistsClause(existsClauseKey3));

			// Setup responses
			response1 = new Response("responseId");
			response2 = new Response("responseId2");

			// Add everything to the packs
			pack1 = new Pack("1");
			pack1.AddRule(rule_oneCondition);
			pack1.AddRule(rule_twoConditions);
			pack1.AddResponse(response1);

			pack2 = new Pack("2");
			pack2.AddRule(rule_threeConditions);
			pack2.AddResponse(response2);

			pack3_sameNameAs1 = new Pack("1").AddResponse(new Response("response1"));
		}

		[TearDown]
		public void TearDown()
		{
			pack1 = null;
			pack2 = null;
			rule_oneCondition = null;
			rule_twoConditions = null;
			rule_threeConditions = null;
			response1 = null;
			response2 = null;
		}

		[Test]
		public void TestLoadPack_New()
		{
			Machine machine = new Machine();

			var status = machine.LoadPack(pack1);

			Assert.That(status, Is.EqualTo(Machine.LoadStatus.New));
		}

		[Test]
		public void TestLoadPack_Additive_Fails_SameResponses()
		{
			Machine machine = new Machine();
			machine.LoadPack(pack1);
			Assert.Throws<System.ArgumentException>(() => machine.LoadPack(pack1));
		}

		[Test]
		public void TestLoadPack_Additive()
		{
			Machine machine = new Machine();
			machine.LoadPack(pack1);
			var status = machine.LoadPack(pack3_sameNameAs1);
			Assert.That(status, Is.EqualTo(Machine.LoadStatus.Additive));
		}

		[Test]
		public void TestLoadPack_Failure_NullPack()
		{
			Machine machine = new Machine();

			var status = machine.LoadPack(null);

			Assert.That(status, Is.EqualTo(Machine.LoadStatus.Failure));
		}

		[Test]
		public void TestLoadPack_Failure_EmptyString()
		{
			Machine machine = new Machine();
			Pack emptyPack = new Pack("");
			emptyPack.AddResponse(response1);

			var status = machine.LoadPack(emptyPack);

			Assert.That(status, Is.EqualTo(Machine.LoadStatus.Failure));
		}

		[Test]
		public void TestLoadPack_Failure_NullString()
		{
			Machine machine = new Machine();
			Pack nullPack = new Pack(null);
			nullPack.AddResponse(response1);
			var status = machine.LoadPack(nullPack);

			Assert.That(status, Is.EqualTo(Machine.LoadStatus.Failure));
		}

		[Test]
		public void TestUnloadAllPacks()
		{
			Machine machine = new Machine();

			machine.LoadPack(pack1);
			machine.UnloadAllPacks();

			Assert.False(machine.TryQueryResponse(response1.Name, out _));
		}

		[Test]
		public void TestUnloadPack_True()
		{
			Machine machine = new Machine();

			machine.LoadPack(pack1);

			Assert.True(machine.TryUnloadPack(pack1.Name));
		}

		[Test]
		public void TestUnloadPack_False()
		{
			Machine machine = new Machine();

			machine.LoadPack(pack1);

			Assert.False(machine.TryUnloadPack(pack1.Name + "a"));
		}

		[Test]
		public void TestTryQueryRule_True()
		{
			Machine machine = new Machine();
			machine.LoadPack(pack1);
			MemoryVariableStorage query = new MemoryVariableStorage();
			query.SetValue(existsClauseKey, true);

			Assert.True(machine.TryQueryRule(query, out var resultRule));
			Assert.That(resultRule, Is.EqualTo(rule_oneCondition));
		}

		[Test]
		public void TestTryQueryRule_PickHigherConditionCount()
		{
			Machine machine = new Machine();
			machine.LoadPack(pack1);
			MemoryVariableStorage query = new MemoryVariableStorage();
			query.SetValue(existsClauseKey, true);
			query.SetValue(existsClauseKey2, true);

			Assert.True(machine.TryQueryRule(query, out var resultRule));
			Assert.That(resultRule, Is.EqualTo(rule_twoConditions));
		}

		[Test]
		public void TestTryQueryRule_PickFromMultiplePacks()
		{
			Machine machine = new Machine();
			machine.LoadPack(pack1);
			machine.LoadPack(pack2);
			MemoryVariableStorage query = new MemoryVariableStorage();
			query.SetValue(existsClauseKey, true);
			query.SetValue(existsClauseKey2, true);
			query.SetValue(existsClauseKey3, true);
			Rule resultRule;

			Assert.True(machine.TryQueryRule(query, out resultRule));
			Assert.That(resultRule, Is.EqualTo(rule_threeConditions));
		}

		[Test]
		public void TestTryQueryRule_False()
		{
			Machine machine = new Machine();
			machine.LoadPack(pack1);
			MemoryVariableStorage query = new MemoryVariableStorage();

			Assert.False(machine.TryQueryRule(query, out var resultRule));
			Assert.IsNull(resultRule);
		}

		[Test]
		public void TestTryQueryResponse_True()
		{
			Machine machine = new Machine();
			machine.LoadPack(pack1);

			Assert.True(machine.TryQueryResponse(response1.Name, out var resultResponse));
			Assert.That(resultResponse.Name, Is.EqualTo(response1.Name));
		}

		[Test]
		public void TestTryQueryResponse_True_FromMultiplePacks()
		{
			Machine machine = new Machine();
			machine.LoadPack(pack1);
			machine.LoadPack(pack2);

			Assert.True(machine.TryQueryResponse(response1.Name, out var resultResponse));
			Assert.That(resultResponse.Name, Is.EqualTo(response1.Name));
			Assert.True(machine.TryQueryResponse(response2.Name, out resultResponse));
			Assert.That(resultResponse.Name, Is.EqualTo(response2.Name));
		}

		[Test]
		public void TestTryQueryReponse_False()
		{
			Machine machine = new Machine();
			machine.LoadPack(pack1);

			Assert.False(machine.TryQueryResponse(response1.Name + "a", out var resultResponse));
			Assert.IsNull(resultResponse);
		}
	}
}
