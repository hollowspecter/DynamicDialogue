using NUnit.Framework;
using DynamicDialogue;
using Antlr4;
using Antlr4.Runtime;
using System.IO;
using DynamicDialogue.Compiler;
using System.Diagnostics;

namespace DynamicDialogueTest
{
	public class Tests
	{

		[SetUp]
		public void Setup()
		{
		}

		[TearDown]
		public void TearDown()
		{
		}

		private TalkingParser CreateParser(string _text)
		{
			AntlrInputStream inputStream = new AntlrInputStream(_text);
			TalkingLexer talkingLexer = new TalkingLexer(inputStream);
			CommonTokenStream commonTokenStream = new CommonTokenStream(talkingLexer);
			TalkingParser talkingParser = new TalkingParser(commonTokenStream);
			return talkingParser;
		}

		[Test]
		public void TestTalk()
		{
			var talkingParser = CreateParser(File.ReadAllText(@"Data\dogTalk.talk"));
			Assert.That(talkingParser.talk().ChildCount, Is.EqualTo(4));
		}

		[Test]
		public void TestResponse()
		{
			var talkingParser = CreateParser(File.ReadAllText(@"Data\dogTalk.talk"));
			var firstResponse = talkingParser.talk().response(0).response_body();
			Assert.That(firstResponse.ChildCount, Is.EqualTo(4));
		}

		[Test]
		public void TestLine()
		{
			var talkingParser = CreateParser(File.ReadAllText(@"Data\dogTalk.talk"));
			var firstResponse = talkingParser.talk().response(0).response_body();
			Assert.AreEqual("\"that's a cute doggo\"", firstResponse.line(0).TEXT().GetText());
			Assert.AreEqual("\"awww so cute!!!\"", firstResponse.line(1).TEXT().GetText());
		}

		[Test]
		public void TestRuleName()
		{
			var talkingParser = CreateParser(File.ReadAllText(@"Data\dogTalk.talk"));
			var rule = talkingParser.talk().rule(0);
			Assert.AreEqual("PersonA_SeesDog", rule.WORD().GetText());
		}

		[Test]
		public void TestRule()
		{
			var talkingParser = CreateParser(File.ReadAllText(@"Data\dogTalk.talk"));
			var ruleBody = talkingParser.talk().rule(0).rule_body();
			Assert.That(ruleBody.conditions().ChildCount, Is.EqualTo(5)); //CONDITIONS 3 statements NEWLINE
			Assert.That(ruleBody.rule_response().WORD().GetText(), Is.EqualTo("SeeDog"));
			Assert.That(ruleBody.remember().ChildCount, Is.EqualTo(1));
			Assert.That(ruleBody.trigger(), Is.Not.Null);
			Assert.That(ruleBody.trigger().mention().WORD().GetText(), Is.EqualTo("CuteDog1"));
			Assert.That(ruleBody.trigger().mention().GetText(), Is.EqualTo("@B"));
		}
	}
}