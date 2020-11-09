using DynamicDialogue;
using DynamicDialogue.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DynamicDialogueTest
{
	public class MachineTests
	{
		private Pack pack;
		private Rule rule;
		private Response response;

		[SetUp]
		public void Setup()
		{
			rule = new Rule();
			response = new Response("testId");
			pack.AddRule(rule);
			pack.AddResponse(response);
		}

		[TearDown]
		public void TearDown()
		{
			response = null;
			rule = null;
			pack = null;
		}

		public void TestLoadPack_New()
		{
			Machine machine = new Machine();

			var status = machine.LoadPack("key1", pack);

			Assert.That(status, Is.EqualTo(Machine.LoadStatus.New));
		}

		public void TestLoadPack_Additive()
		{
			Machine machine = new Machine();
			string keyName = "key1";
			machine.LoadPack(keyName, pack);
			var status = machine.LoadPack(keyName, pack);
			Assert.That(status, Is.EqualTo(Machine.LoadStatus.Additive));
		}

		public void TestLoadPack_Failure()
		{
			Machine machine = new Machine();

			var status = machine.LoadPack("key1", null);

			Assert.That(status, Is.EqualTo(Machine.LoadStatus.Failure));
		}

		public void TestUnloadAllPacks()
		{
			Machine machine = new Machine();

			machine.LoadPack("key1", pack);
			machine.UnloadAllPacks();

			Assert.False(machine.TryQueryResponse(response.Name, out _));
		}

		public void TestUnloadPack_True()
		{
			Machine machine = new Machine();
			string key = "key1";

			machine.LoadPack("key1", pack);

			Assert.True(machine.TryUnloadPack(key));
		}

		public void TestUnloadPack_False()
		{
			Machine machine = new Machine();
			string key = "key1";

			machine.LoadPack("key1", pack);

			Assert.True(machine.TryUnloadPack(key + "a"));
		}

		public void TestTryQueryRule_True()
		{
		}

		public void TestTryQueryRule_False()
		{
		}

		public void TestTryQueryResponse_True()
		{
		}

		public void TestTryQueryReponse_False()
		{
		}
	}
}
