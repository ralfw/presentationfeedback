using System;

namespace afapp.body.data
{
	using contract.data;
	using System.Collections.Generic;

	public class SessionWithScoresData
	{
		public string Id;
		public string Title;
		public DateTime Start, End;
		public string SpeakerName;
		public string SpeakerEmail;
		public Func<string, string> ConferenceTitle;
		public Func<string, IEnumerable<TrafficLightScores>> Scores;
		public Func<string, bool> IsSpeakerNotifiedAboutSessionfeedback;
	}
}
 