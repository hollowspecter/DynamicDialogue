using NUnit.Framework;
using Antlr4.Runtime;
using System.IO;
using DynamicDialogue.Core;
using DynamicDialogue.Compiler;
using static DynamicDialogue.Test.TestFiles;

namespace DynamicDialogueTest
{
	public class CompilerTests
	{
		private const string line0 = "that's a cute doggo";
		private const string line1 = "awww so cute!!!";
		private const string line2 = "so awesome";
		private const string line3 = "such a cutie!";

		private BarkParser CreateParser(string text)
		{
			AntlrInputStream inputStream = new AntlrInputStream(text);
			BarkLexer lexer = new BarkLexer(inputStream);
			CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
			BarkParser parser = new BarkParser(commonTokenStream);

			//turning off the normal error listener and using ours
			lexer.RemoveErrorListeners();
			lexer.AddErrorListener(LexerErrorListener.Instance);
			parser.RemoveErrorListeners();
			parser.AddErrorListener(ParseErrorListener.Instance);

			return parser;
		}

		[TestCase(0, ExpectedResult = line0)]
		[TestCase(1, ExpectedResult = line1)]
		[TestCase(2, ExpectedResult = line2)]
		[TestCase(3, ExpectedResult = line3)]
		public string TestLineVisitor(int lineIndex)
		{
			// Arrange			
			var parser = CreateParser(File.ReadAllText(DogTalk));
			LineVisitor visitor = new LineVisitor();

			// Act & Assert
			return parser.talk().response(0).response_body().line(lineIndex).Accept(visitor);
		}

		[Test]
		public void TestResponseVisitor()
		{
			var parser = CreateParser(File.ReadAllText(DogTalk));
			ResponseVisitor visitor = new ResponseVisitor();

			Response response = parser.talk().response(0).Accept(visitor);

			Assert.That(response.Name, Is.EqualTo("SeeDog"));
			Assert.That(response.LineCount, Is.EqualTo(4));
		}

		[Test]
		public void TestEqualsStatement_Float()
		{
			var parser = CreateParser(File.ReadAllText(DogTalk));

			// Dogseen=0
			EqualsStatement statement = new EqualsStatement(parser.talk().rule(0).rule_body().conditions().condition_statement(1).equals_statement());

			Assert.That(statement.Key, Is.EqualTo("DogSeen"));
			Assert.That(statement.ThisMode, Is.EqualTo(EqualsStatement.Mode.Float));
			Assert.That(statement.FloatValue, Is.EqualTo(2f));
			Assert.That(statement.BoolValue, Is.EqualTo(false));
			Assert.That(statement.StringValue, Is.EqualTo(""));
		}

		[Test]
		public void TestEqualsStatement_String()
		{
			var parser = CreateParser(File.ReadAllText(DogTalk));

			// Is=@A
			EqualsStatement statement = new EqualsStatement(parser.talk().rule(0).rule_body().conditions().condition_statement(2).equals_statement());

			Assert.That(statement.Key, Is.EqualTo("Is"));
			Assert.That(statement.ThisMode, Is.EqualTo(EqualsStatement.Mode.String));
			Assert.That(statement.FloatValue, Is.EqualTo(0f));
			Assert.That(statement.BoolValue, Is.EqualTo(false));
			Assert.That(statement.StringValue, Is.EqualTo("@A"));
		}

		[Test]
		public void TestEqualsStatement_Bool()
		{
			var parser = CreateParser(File.ReadAllText(DogTalk));

			// Dead=true
			EqualsStatement statement = new EqualsStatement(parser.talk().rule(1).rule_body().conditions().condition_statement(4).equals_statement());

			Assert.That(statement.Key, Is.EqualTo("Dead"));
			Assert.That(statement.ThisMode, Is.EqualTo(EqualsStatement.Mode.Bool));
			Assert.That(statement.FloatValue, Is.EqualTo(0f));
			Assert.That(statement.BoolValue, Is.EqualTo(true));
			Assert.That(statement.StringValue, Is.EqualTo(""));
		}

		[Test]
		public void TestTriggerVisitor()
		{
			var parser = CreateParser(File.ReadAllText(DogTalk));
			TriggerVisitor visitor = new TriggerVisitor();

			Consequence trigger = parser.talk().rule(0).rule_body().Accept(visitor);

			Assert.That(trigger, Is.InstanceOf(typeof(TriggerResponse)));
			//TODO not sure how to test the Trigger Responses yet. Behaviour is yet undefined
		}

		[Test]
		public void TestRememberStatementVisitor()
		{
			var parser = CreateParser(File.ReadAllText(DogTalk));
			StorageChange change = new StorageChange();
			RememberStatementVisitor visitor = new RememberStatementVisitor(change);
			IVariableStorage storage = new MemoryVariableStorage();

			parser.talk().rule(0).rule_body().remember().Accept(visitor);
			change.Execute(storage);

			Assert.True(storage.TryGetValue("Dog_Seen", out float result));
			Assert.AreEqual(result, 1f);
		}

		[Test]
		public void RuleResponseVisitor()
		{
			var parser = CreateParser(File.ReadAllText(DogTalk));
			RuleResponseVisitor visitor = new RuleResponseVisitor();

			Consequence ruleResponse = parser.talk().rule(0).rule_body().rule_response().Accept(visitor);

			Assert.That(ruleResponse, Is.InstanceOf(typeof(TextResponse)));
			//TODO also not sure what to do here
		}

		[Test]
		public void TestConditionVisitor_Exists()
		{
			var parser = CreateParser(File.ReadAllText(DogTalk));
			ConditionVisitor visitor = new ConditionVisitor();
			IVariableStorage storage = new MemoryVariableStorage();
			storage.SetValue("ConceptSeeDog", false);

			Clause clause = parser.talk().rule(0).rule_body().conditions().condition_statement(0).Accept(visitor);

			Assert.That(clause, Is.InstanceOf(typeof(ExistsClause)));
			Assert.True(clause.Check(storage));
		}

		[Test]
		public void TestConditionVisitor_String()
		{
			var parser = CreateParser(File.ReadAllText(DogTalk));
			ConditionVisitor visitor = new ConditionVisitor();
			IVariableStorage storage = new MemoryVariableStorage();
			storage.SetValue("Is", "@A");

			Clause clause = parser.talk().rule(0).rule_body().conditions().condition_statement(2).Accept(visitor);

			Assert.That(clause, Is.InstanceOf(typeof(StringClause)));
			Assert.True(clause.Check(storage));
		}

		[Test]
		public void TestConditionVisitor_Bool()
		{
			var parser = CreateParser(File.ReadAllText(DogTalk));
			ConditionVisitor visitor = new ConditionVisitor();
			IVariableStorage storage = new MemoryVariableStorage();
			storage.SetValue("Dead", true);

			Clause clause = parser.talk().rule(1).rule_body().conditions().condition_statement(4).Accept(visitor);

			Assert.That(clause, Is.InstanceOf(typeof(BoolClause)));
			Assert.True(clause.Check(storage));
		}

		[Test]
		public void TestConditionVisitor_Float()
		{
			var parser = CreateParser(File.ReadAllText(DogTalk));
			ConditionVisitor visitor = new ConditionVisitor();
			IVariableStorage storage = new MemoryVariableStorage();
			storage.SetValue("DogSeen", 2f);

			Clause clause = parser.talk().rule(0).rule_body().conditions().condition_statement(1).Accept(visitor);

			Assert.That(clause, Is.InstanceOf(typeof(FloatClause)));
			Assert.True(clause.Check(storage));
		}

		[Test]
		public void TestRuleVisitor()
		{
			var parser = CreateParser(File.ReadAllText(DogTalk));
			RuleVisitor visitor = new RuleVisitor();
			IVariableStorage query = new MemoryVariableStorage();
			IVariableStorage empty = new MemoryVariableStorage();
			query.SetValue("ConceptSeeDog", true);
			query.SetValue("DogSeen", 2f);
			query.SetValue("Is", "@A");

			Rule rule = parser.talk().rule(0).rule_body().Accept(visitor);
			rule.Execute(empty);

			Assert.True(rule.Check(query));
			Assert.True(empty.TryGetValue("Dog_Seen", out float dogSeenValue));
			Assert.That(dogSeenValue, Is.EqualTo(1f));

			//TODO once i know how to check response and trigger
		}

		[Test]
		public void TestCompiler()
		{
			var status = Compiler.CompileFile(DogTalk, out var pack);

			Assert.That(status, Is.EqualTo(Compiler.Status.Success));
			Assert.That(pack.ReponseCount, Is.EqualTo(2));
			Assert.That(pack.RuleCount, Is.EqualTo(4));
		}
	}
}
