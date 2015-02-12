using EventStore.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EventStore.Internals.File
{
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
			using (var writer = new StreamWriter(GetFullFilePath(filename)))
			{
				writer.WriteLine(recordedEvent.Id);
				writer.WriteLine(recordedEvent.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffffff"));
				writer.WriteLine(recordedEvent.SequenceNumber);

				writer.WriteLine(recordedEvent.Event.Context);
				writer.WriteLine(recordedEvent.Event.Name);
				writer.Write(recordedEvent.Event.Payload);
			}
		}

		private string GetFullFilePath(string fileName)
		{
			return Path.Combine(dirPath, fileName);
		}

		internal IEnumerable<IRecordedEvent> ReadAll()
		{
			return Directory.GetFiles(dirPath).Select(x => Read(x, GetTypeOfEvent));
		}

		private static IRecordedEvent Read(string fileName, Func<string, Type> getEventType)
		{
			using (var reader = new StreamReader(fileName))
			{
				var id = Guid.Parse("" + reader.ReadLine());
				var timeStamp = DateTime.Parse(reader.ReadLine());
				var sequenceNumber = long.Parse("" + reader.ReadLine());
				var context = reader.ReadLine();
				var name = reader.ReadLine();
				var payload = reader.ReadToEnd();
				var @event = (IEvent) Activator.CreateInstance(getEventType(name), context, name, payload);
				return new RecordedEvent(id, timeStamp, sequenceNumber, @event);
			}
		}

		private static Type GetTypeOfEvent(string eventName)
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies)
			{
				var eventType = assembly.GetTypes().FirstOrDefault(t => t.Name == eventName && 
					t.GetInterfaces().Contains(typeof(IEvent)));
				if (eventType != null)
				{
					return eventType;
				}
			}
			throw new Exception("Unknown class:" + eventName);
		}

		internal long GetNextSequenceNumber()
		{
			return Directory.GetFiles(dirPath).Length;
		}
	}
}
