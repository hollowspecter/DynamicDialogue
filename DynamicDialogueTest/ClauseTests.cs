using DynamicDialogue;
using DynamicDialogue.Core;
using NUnit.Framework;

namespace DynamicDialogueTest
{
	public class ClauseTests
	{
		private MemoryVariableStorage storage = new MemoryVariableStorage();

		[SetUp]
		public void Setup()
		{
			storage.SetValue("ConceptSeeDog", 0);
			storage.SetValue("ConceptSeeCat", 1);
			storage.SetValue("Is", "@Vivi");
			storage.SetValue("Hello", " ");
			storage.SetValue("IsAlive", true);
			storage.SetValue("IsDead", false);
		}

		[TearDown]
		public void TearDown()
		{
			storage.Clear();
		}

		[TestCase("ConceptSeeDog", ExpectedResult = true)]
		[TestCase("ConceptSeeDog1", ExpectedResult = false)]
		[TestCase("conceptSeeDog1", ExpectedResult = false)]
		[TestCase("asd", ExpectedResult = false)]
		public bool TestExistsClause(string key)
		{
			// Arrange
			Clause clause = new ExistsClause(key);

			// Act
			return clause.Check(storage);
		}

		[TestCase("Is", "@Vivi", ExpectedResult = true)]
		[TestCase("Hello", " ", ExpectedResult = true)]
		[TestCase("Is", "Vivi", ExpectedResult = false)]
		[TestCase("Hello", "	", ExpectedResult = false)]
		[TestCase("qweoih", "	", ExpectedResult = false)]
		public bool TestStringClause(string key, string compareTo)
		{
			Clause clause = new StringClause(key, compareTo);
			return clause.Check(storage);
		}

		[TestCase("IsAlive", true, ExpectedResult = true)]
		[TestCase("IsDead", false, ExpectedResult = true)]
		[TestCase("IsDead", true, ExpectedResult = false)]
		[TestCase("asd", true, ExpectedResult = false)]
		public bool TestBoolClause(string key, bool compareTo)
		{
			Clause clause = new BoolClause(key, compareTo);
			return clause.Check(storage);
		}

		[TestCase("ConceptSeeDog", 0f, ExpectedResult = true)]
		[TestCase("ConceptSeeCat", 1f, ExpectedResult = true)]
		[TestCase("ConceptSeeCat", 1.1f, ExpectedResult = false)]
		[TestCase("ConceptSeeCat", 1.001f, ExpectedResult = false)]
		public bool TestFloatClauseEquals(string key, float compareTo)
		{
			return new FloatClause(key, FloatClause.CompareMode.EQUAL_TO, compareTo).Check(storage);
		}

		[TestCase("ConceptSeeDog", 0.1f, ExpectedResult = true)]
		[TestCase("ConceptSeeDog", 0.0f, ExpectedResult = true)]
		[TestCase("ConceptSeeDog", -0.1f, ExpectedResult = false)]
		public bool TestFloatClauseEqualOrLess(string key, float compareTo)
		{
			return new FloatClause(key, FloatClause.CompareMode.EQUAL_OR_LESS_THAN, compareTo).Check(storage);
		}

		[TestCase("ConceptSeeDog", -0.1f, ExpectedResult = true)]
		[TestCase("ConceptSeeDog", 0f, ExpectedResult = true)]
		[TestCase("ConceptSeeDog", 0.1f, ExpectedResult = false)]
		public bool TestFloatClauseEqualOrGreater(string key, float compareTo)
		{
			return new FloatClause(key, FloatClause.CompareMode.EQUAL_OR_GREATER_THAN, compareTo).Check(storage);
		}
	}
}
