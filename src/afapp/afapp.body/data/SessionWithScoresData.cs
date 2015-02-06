using System;

namespace afapp.body.data
{
	using contract.data;
	using System.Collections.Generic;

	public class SessionWithScoresData
	{
		public string ConferenceTitle;
		public string Id;
		public string Title;
		public DateTime Start, End;
		public string SpeakerName;
		public string SpeakerEmail;
		public IEnumerable<TrafficLightScores> Scores;
		public bool IsSpeakerNotifiedAboutSessionfeedback;
	}
}
