using NUnit.Framework;
using Antlr4.Runtime;
using DynamicDialogue.Compiler;
using DynamicDialogue;
using static DynamicDialogue.Test.TestFiles;

namespace DynamicDialogueTest
{
	public class RuleTests
	{
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
