using NUnit.Framework;
using DynamicDialogue;
using Antlr4;
using Antlr4.Runtime;
using System.IO;
using DynamicDialogue.Compiler;
using System.Diagnostics;
using static DynamicDialogueTest.TestFiles;

namespace DynamicDialogueTest
{
	public class RuleTests
	{
		private BarkParser CreateParser(string _text)
		{
			AntlrInputStream inputStream = new AntlrInputStream(_text);
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

		[Test]
		public void TestRulesSorted()
		{
			Compiler.CompileFile(DogTalk, out var pack);

			for (int i = 1; i < pack.RuleCount; ++i)
			{
				Assert.That(pack.GetRule(i - 1).ConditionCount, Is.GreaterThanOrEqualTo(pack.GetRule(i).ConditionCount));
			}
		}
	}
}
