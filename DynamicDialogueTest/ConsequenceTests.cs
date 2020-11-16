using DynamicDialogue.Core;
using DynamicDialogue;
using NUnit.Framework;

namespace DynamicDialogueTest
{
	public class ConsequenceTests
	{
		[Test]
		public void TestStorageChange_Bool()
		{
			StorageChange change = new StorageChange();
			string key = "test";
			bool value = true;
			change.AddChange(key, value);
			Machine machine = new Machine
			{
				StorageChangeHandler = changes =>
				{
					Assert.True(changes.TryGetValue(key, out var result));
					Assert.True(result is bool);
					Assert.True((bool)result);
				}
			};

			change.Execute(machine);
		}

		[Test]
		public void TestStorageChange_Float()
		{
			StorageChange change = new StorageChange();
			string key = "test";
			float value = 13.5f;
			change.AddChange(key, value);
			Machine machine = new Machine
			{
				StorageChangeHandler = changes =>
				{
					Assert.True(changes.TryGetValue(key, out var result));
					Assert.True(result is float);
					Assert.That((float)result, Is.EqualTo(value));
				}
			};

			change.Execute(machine);

		}

		[Test]
		public void TestStorageChange_String()
		{
			StorageChange change = new StorageChange();
			string key = "test";
			string value = " .128)&ASD";
			change.AddChange(key, value);

			change.Execute(new Machine
			{
				StorageChangeHandler = changes =>
				{
					Assert.True(changes.TryGetValue(key, out var result));
					Assert.True(result is string);
					Assert.AreEqual((string)result, value);
				}
			});
		}

		[Test]
		public void TestTextReponse()
		{
			string expectedId = "responseId";
			TextResponse textResponse = new TextResponse(expectedId);
			textResponse.Execute(new Machine
			{
				TextResponseHandler = responseId =>
				{
					Assert.AreEqual(responseId, expectedId);
				}
			});
		}

		[Test]
		public void TestTriggerResponse()
		{
			string expectedTo = "Vivi";
			string expectedConcept = "Talk";
			TriggerResponse triggerResponse = new TriggerResponse(expectedTo, expectedConcept);
			triggerResponse.Execute(new Machine
			{
				TriggerResponseHandler = trigger =>
				{
					Assert.AreEqual(trigger.To, expectedTo);
					Assert.AreEqual(trigger.ConceptName, expectedConcept);
				}
			});
		}
	}
}
