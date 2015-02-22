namespace Contract.data
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public struct SessionParsed {
		public string Id;
		public string Title;
		public DateTime Start, End;
		public string SpeakerName;
		public string SpeakerEmail;
	}

	public class CSVParser {
		public IEnumerable<SessionParsed> ParseSessions(string text) {
			return text.Split (new[]{ '\n' }, StringSplitOptions.RemoveEmptyEntries)
					   .Select (line => {
							var fields = line.Split('\t');
							return new SessionParsed{
								Id = fields[0],
								Title = fields[1],
								Start = DateTime.Parse(fields[2]),
								End = DateTime.Parse(fields[3]),
								SpeakerName = fields[4],
								SpeakerEmail = fields[5].Replace("\r", "")
							};
					   });
		}
	}
	
}
