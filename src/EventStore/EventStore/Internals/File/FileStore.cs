using EventStore.Contract;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EventStore.Internals.File
{
	using System.Runtime.Serialization.Formatters.Binary;

	internal class FileStore
	{
		private readonly string dirPath;

		internal FileStore(string path)
		{
			dirPath = path;
			var dir = new DirectoryInfo(path);
			if (!dir.Exists)
			{
				dir.Create();
			}
		}

		internal void Write(string filename, IRecordedEvent recordedEvent)
		{
			using (var stream = new FileStream(GetFullFilePath(filename), FileMode.Create))
			{
				BinaryFormatter bFormatter = new BinaryFormatter();
				bFormatter.Serialize(stream, recordedEvent);
			}
		}

		private string GetFullFilePath(string fileName)
		{
			return Path.Combine(dirPath, fileName);
		}

		internal IEnumerable<IRecordedEvent> ReadAll()
		{
			return Directory.GetFiles(dirPath).Select(Read);
		}

		private static IRecordedEvent Read(string fileName)
		{
			using (var reader = new FileStream(fileName, FileMode.Open))
			{
				BinaryFormatter bFormatter = new BinaryFormatter();
				return (RecordedEvent)bFormatter.Deserialize(reader);
			}
		}
	
		internal long GetNextSequenceNumber()
		{
			return Directory.GetFiles(dirPath).Length;
		}
	}
}
