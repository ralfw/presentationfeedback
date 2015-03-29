using Contract.data;
using Repository.data;
using System.Linq;

namespace nsapp.body
{
	public class Mapper {

		public SpeakerNotificationData Map(ScoredSessionData session)
		{
			var comments = session.UniqueFeedback.Where(x => !string.IsNullOrWhiteSpace(x.Comment));
			var commentsString = string.Join("\n", comments.Select(x => string.Format("{0}: {1}", x.Score, x.Comment)));
			var feedback = session.UniqueFeedback.ToList();
			return new SpeakerNotificationData
			{
				ConfTitle = session.ConfTitle,
				Title = session.Title,
				Start = session.Start,
				End = session.End,
				SpeakerName = session.SpeakerName,
				SpeakerEmail = session.SpeakerEmail,
				Reds = feedback.Count(x => x.Score == TrafficLightScores.Red),
				Yellows = feedback.Count(x => x.Score == TrafficLightScores.Yellow),
				Greens = feedback.Count(x => x.Score == TrafficLightScores.Green),
				Comments = commentsString
			};
		}
	}
}
