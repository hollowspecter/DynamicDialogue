
using System.Reflection;
using System.IO;
using System.Linq;
using System.Globalization;

namespace DynamicDialogue.Test
{
	public static class TestFiles
	{
		public static string ProjectRootPath
		{
			get
			{
				var path = Assembly.GetCallingAssembly().Location.Split(Path.DirectorySeparatorChar).ToList();

				var index = path.FindIndex(x => x == "DynamicDialogueTest");

				if (index == -1)
				{
					throw new System.IO.DirectoryNotFoundException("Cannot find test data directory");
				}

				var testDataDirectory = path.Take(index).ToList();

				var pathToTestData = string.Join(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture), testDataDirectory.ToArray());

				return pathToTestData;
			}
		}

		public static string TestDataPath
		{
			get
			{
				return Path.Combine(ProjectRootPath, "Tests\\");
			}
		}

		public static readonly string DogTalk = "dogTalk.bark";
	}
}
