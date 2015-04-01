using System.Collections.Generic;
using System.Linq;

namespace Repository.data
{
	using System;
	using Contract.data;

	public class ScoredSessionData
	{
		public string ConfId;
		public string ConfTitle;

		public string Id;
		public string Title;
		public string TimeZone;

		public DateTimeWithZone Start, End;

		public string SpeakerName;
		public string SpeakerEmail;

		public bool SpeakerNotified;

		public List<FeedbackData> Feedback = new List<FeedbackData>();

		public IEnumerable<FeedbackData> UniqueFeedback { get { 
				var feedbackByEmail = this.Feedback.GroupBy (f => f.Email);
				return feedbackByEmail.SelectMany<IGrouping<string,FeedbackData>, FeedbackData> (fg => {
					if (fg.Key == "")
						return fg;
					else
						// take only the last feedback given by an identified voter
						return new[] {fg.Last()};
				});
			}
		}
	}
}
 