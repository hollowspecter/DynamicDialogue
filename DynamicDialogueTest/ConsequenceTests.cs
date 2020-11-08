using DynamicDialogue;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

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
			IVariableStorage storage = new MemoryVariableStorage();

			change.Execute(storage);

			Assert.True(storage.TryGetValue(key, out bool result));
			Assert.That(result, Is.EqualTo(value));
		}

		[Test]
		public void TestStorageChange_Float()
		{
			StorageChange change = new StorageChange();
			string key = "test";
			float value = 13.5f;
			change.AddChange(key, value);
			IVariableStorage storage = new MemoryVariableStorage();

			change.Execute(storage);

			Assert.True(storage.TryGetValue(key, out float result));
			Assert.That(result, Is.EqualTo(value));
		}

		[Test]
		public void TestStorageChange_String()
		{
			StorageChange change = new StorageChange();
			string key = "test";
			string value = " .128)&ASD";
			change.AddChange(key, value);
			IVariableStorage storage = new MemoryVariableStorage();

			change.Execute(storage);

			Assert.True(storage.TryGetValue(key, out string result));
			Assert.That(result, Is.EqualTo(value));
		}

		[Test]
		public void TestTextReponse()
		{
		}

		[Test]
		public void TestTriggerResponse()
		{
		}
	}
}
