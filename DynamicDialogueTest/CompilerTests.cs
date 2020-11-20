using NUnit.Framework;
using Antlr4.Runtime;
using System.IO;
using DynamicDialogue;
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
			var parser = CreateParser(File.ReadAllText(TestDataPath + DogTalk));
			LineVisitor visitor = new LineVisitor();

			// Act & Assert
			return parser.talk().response(0).response_body().line(lineIndex).Accept(visitor);
		}

		[Test]
		public void TestResponseVisitor()
		{
			var parser = CreateParser(File.ReadAllText(TestDataPath + DogTalk));
			ResponseVisitor visitor = new ResponseVisitor();

			Response response = parser.talk().response(0).Accept(visitor);

			Assert.That(response.Name, Is.EqualTo("SeeDog"));
			Assert.That(response.LineCount, Is.EqualTo(4));
		}

		[Test]
		public void TestEqualsStatement_Float()
		{
			var parser = CreateParser(File.ReadAllText(TestDataPath + DogTalk));

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
			var parser = CreateParser(File.ReadAllText(TestDataPath + DogTalk));

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
			var parser = CreateParser(File.ReadAllText(TestDataPath + DogTalk));

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
			var parser = CreateParser(File.ReadAllText(TestDataPath + DogTalk));
			TriggerVisitor visitor = new TriggerVisitor();

			Consequence trigger = parser.talk().rule(0).rule_body().Accept(visitor);

			Assert.That(trigger, Is.InstanceOf(typeof(TriggerResponse)));

			trigger.Execute(new Machine()
			{
				TriggerResponseHandler = currentTrigger =>
				{
					Assert.AreEqual(currentTrigger.To, "@B");
					Assert.AreEqual(currentTrigger.ConceptName, "CuteDog1");
				}
			});
		}

		[Test]
		public void TestRememberStatementVisitor()
		{
			var parser = CreateParser(File.ReadAllText(TestDataPath + DogTalk));
			StorageChange change = new StorageChange();
			RememberStatementVisitor visitor = new RememberStatementVisitor(change);
			parser.talk().rule(0).rule_body().remember().Accept(visitor);

			change.Execute(new Machine
			{
				StorageChangeHandler = changes =>
				{
					Assert.True(changes.TryGetValue("Dog_Seen", out var value));
					Assert.True(value is float);
					Assert.AreEqual((float)value, 1f);
				}
			});
		}

		[Test]
		public void RuleResponseVisitor()
		{
			var parser = CreateParser(File.ReadAllText(TestDataPath + DogTalk));
			RuleResponseVisitor visitor = new RuleResponseVisitor();

			Consequence ruleResponse = parser.talk().rule(0).rule_body().rule_response().Accept(visitor);

			Assert.That(ruleResponse, Is.InstanceOf(typeof(TextResponse)));

			ruleResponse.Execute(new Machine() { TextResponseHandler = responseId => Assert.AreEqual("SeeDog", responseId) });
		}

		[Test]
		public void TestConditionVisitor_Exists()
		{
			var parser = CreateParser(File.ReadAllText(TestDataPath + DogTalk));
			ConditionVisitor visitor = new ConditionVisitor();

			Clause clause = parser.talk().rule(0).rule_body().conditions().condition_statement(0).Accept(visitor);

			Assert.That(clause, Is.InstanceOf(typeof(ExistsClause)));
			Assert.AreEqual("ConceptSeeDog", clause.key);
			Assert.True(clause.Check(false));
		}

		[Test]
		public void TestConditionVisitor_String()
		{
			var parser = CreateParser(File.ReadAllText(TestDataPath + DogTalk));
			ConditionVisitor visitor = new ConditionVisitor();

			Clause clause = parser.talk().rule(0).rule_body().conditions().condition_statement(2).Accept(visitor);

			Assert.That(clause, Is.InstanceOf(typeof(StringClause)));
			Assert.AreEqual("Is", clause.key);
			Assert.True(clause.Check("@A"));
		}

		[Test]
		public void TestConditionVisitor_Bool()
		{
			var parser = CreateParser(File.ReadAllText(TestDataPath + DogTalk));
			ConditionVisitor visitor = new ConditionVisitor();

			Clause clause = parser.talk().rule(1).rule_body().conditions().condition_statement(4).Accept(visitor);

			Assert.That(clause, Is.InstanceOf(typeof(BoolClause)));
			Assert.AreEqual("Dead", clause.key);
			Assert.True(clause.Check(true));
		}

		[Test]
		public void TestConditionVisitor_Float()
		{
			var parser = CreateParser(File.ReadAllText(TestDataPath + DogTalk));
			ConditionVisitor visitor = new ConditionVisitor();

			Clause clause = parser.talk().rule(0).rule_body().conditions().condition_statement(1).Accept(visitor);

			Assert.That(clause, Is.InstanceOf(typeof(FloatClause)));
			Assert.AreEqual("DogSeen", clause.key);
			Assert.True(clause.Check(2f));
		}

		[Test]
		public void TestRuleVisitor_AllHandlers()
		{
			var parser = CreateParser(File.ReadAllText(TestDataPath + DogTalk));
			RuleVisitor visitor = new RuleVisitor();
			IVariableStorage query = new MemoryVariableStorage();
			query.SetValue("ConceptSeeDog", true);
			query.SetValue("DogSeen", 2f);
			query.SetValue("Is", "@A");

			Rule rule = parser.talk().rule(0).rule_body().Accept(visitor);
			rule.Execute(new Machine()
			{
				StorageChangeHandler = changes =>
				{
					Assert.True(changes.TryGetValue("Dog_Seen", out var dogSeenValue));
					Assert.AreEqual((float)dogSeenValue, 1f);
				},

				TextResponseHandler = responseId =>
				{
					Assert.AreEqual(responseId, "SeeDog");
				},

				TriggerResponseHandler = trigger =>
				{
					Assert.AreEqual(trigger.To, "@B");
					Assert.AreEqual(trigger.ConceptName, "CuteDog1");
				}
			});

			Assert.True(rule.Check(new IVariableStorage[] { query }));
		}

		[Test]
		public void TestCompiler()
		{
			var status = Compiler.CompileFile(TestDataPath + DogTalk, out var pack);

			Assert.That(status, Is.EqualTo(Compiler.Status.Success));
			Assert.That(pack.ReponseCount, Is.EqualTo(2));
			Assert.That(pack.RuleCount, Is.EqualTo(4));
		}
	}
}
