
namespace coapp.body
{
	using Contract.data;
	using data; 
	using System.Collections.Generic;
	using System.Linq;

	internal class Mapper
	{
		internal static ConferenceFeedbackData Map(IEnumerable<ScoredSessionData> sessions)
		{
			var sessionAsList = sessions as IList<ScoredSessionData> ?? sessions.ToList();
			return new ConferenceFeedbackData
			{
				Title = sessionAsList.First().ConfTitle,
				Sessions = sessionAsList.Select(Build_session)
			};
		}

		private static ConferenceFeedbackData.Session Build_session(ScoredSessionData data)
		{
			return new ConferenceFeedbackData.Session
			{
				Id = data.Id,
				Title = data.Title,
				Start = data.Start,
				End = data.End,
				SpeakerName = data.SpeakerName,
				SpeakerEmail = data.SpeakerEmail,
				Reds = data.UniqueFeedback.Count(x => x.Score == TrafficLightScores.Red),
				Yellows = data.UniqueFeedback.Count(x => x.Score == TrafficLightScores.Yellow),
				Greens = data.UniqueFeedback.Count(x => x.Score == TrafficLightScores.Green),
				Comments = data.UniqueFeedback.Select(feedback => new ConferenceFeedbackData.Comment
				{
					Email = feedback.Email,
					Content = feedback.Comment
				})
			};
		}
	}
}
