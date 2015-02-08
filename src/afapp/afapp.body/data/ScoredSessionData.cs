using System;

namespace afapp.body.data
{
	using contract.data;
	using System.Collections.Generic;

	public class ScoredSessionData
	{
		public string ConfId;
		public string ConfTitle;

		public string Id;
		public string Title;

		public DateTime Start, End;

		public string SpeakerName;
		public string SpeakerEmail;

		public bool SpeakerNotified;

		public List<FeedbackData> Feedback;

		//TODO: return unique feedback (only the last feedback per email)
	}
}
 