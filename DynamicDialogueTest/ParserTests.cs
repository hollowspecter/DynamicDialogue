using NUnit.Framework;
using DynamicDialogue;
using Antlr4;
using Antlr4.Runtime;
using System.IO;
using DynamicDialogue.Compiler;
using System.Diagnostics;

namespace DynamicDialogueTest
{
	public class ParserTests
	{
		private string dogTalkPath = @"Data\dogTalk.talk";

		private TalkingParser CreateParser(string _text)
		{
			AntlrInputStream inputStream = new AntlrInputStream(_text);
			TalkingLexer lexer = new TalkingLexer(inputStream);
			CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
			TalkingParser parser = new TalkingParser(commonTokenStream);

			//turning off the normal error listener and using ours
			lexer.RemoveErrorListeners();
			lexer.AddErrorListener(LexerErrorListener.Instance);
			parser.RemoveErrorListeners();
			parser.AddErrorListener(ParseErrorListener.Instance);

			return parser;
		}

		[Test]
		public void TestTalk()
		{
			var talkingParser = CreateParser(File.ReadAllText(dogTalkPath));
			Assert.That(talkingParser.talk().ChildCount, Is.EqualTo(4));
		}

		[Test]
		public void TestRule()
		{
			var talkingParser = CreateParser(File.ReadAllText(dogTalkPath));
			var ruleBody = talkingParser.talk().rule(0).rule_body();
			Assert.That(ruleBody.conditions().condition_statement().Length, Is.EqualTo(3));
			Assert.That(ruleBody.rule_response().WORD().GetText(), Is.EqualTo("SeeDog"));
			Assert.That(ruleBody.remember().equals_statement().Length, Is.EqualTo(1)); // REMEMBER and the one condition
			Assert.That(ruleBody.trigger().WORD().GetText(), Is.EqualTo("CuteDog1"));
			Assert.That(ruleBody.trigger().MENTION().GetText(), Is.EqualTo("@B"));
		}

		[Test]
		public void TestRuleName()
		{
			var talkingParser = CreateParser(File.ReadAllText(dogTalkPath));
			var rule = talkingParser.talk().rule(0);
			Assert.AreEqual("PersonA_SeesDog", rule.WORD().GetText());
		}

		[Test]
		public void TestConditionStatements()
		{
			var talkingParser = CreateParser(File.ReadAllText(dogTalkPath));
			var conditions = talkingParser.talk().rule(0).rule_body().conditions();
			Assert.That(conditions.condition_statement(0).GetText(), Is.EqualTo("ConceptSeeDog"));
			Assert.That(conditions.condition_statement(1).GetText(), Is.EqualTo("DogSeen=0"));
			Assert.That(conditions.condition_statement(2).GetText(), Is.EqualTo("@A"));
		}

		[Test]
		public void TestEqualsStatement()
		{
			var talkingParser = CreateParser(File.ReadAllText(dogTalkPath));
			var equalsStatement = talkingParser.talk().rule(0).rule_body().conditions().condition_statement(1).equals_statement();
			Assert.That(equalsStatement.WORD(0).GetText(), Is.EqualTo("DogSeen"));
			Assert.That(int.Parse(equalsStatement.NUMBER().GetText()), Is.EqualTo(0));
		}

		[Test]
		public void TestResponse()
		{
			var talkingParser = CreateParser(File.ReadAllText(dogTalkPath));
			var firstResponse = talkingParser.talk().response(0).response_body();
			Assert.That(firstResponse.ChildCount, Is.EqualTo(4));
		}

		[Test]
		public void TestLine()
		{
			var talkingParser = CreateParser(File.ReadAllText(dogTalkPath));
			var firstResponse = talkingParser.talk().response(0).response_body();
			Assert.AreEqual("\"that's a cute doggo\"", firstResponse.line(0).TEXT().GetText());
			Assert.AreEqual("\"awww so cute!!!\"", firstResponse.line(1).TEXT().GetText());
		}
	}
}