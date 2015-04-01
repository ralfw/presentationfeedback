
namespace coapp.body.data
{
	using System;
	using System.Collections.Generic;
	using Contract.data;

	public class ConferenceFeedbackData
	{
		public string Title;
		public IEnumerable<Session> Sessions;

		public class Session
		{
			public string Id;
			public string Title;
			public DateTimeWithZone Start, End;
			public string SpeakerName;
			public string SpeakerEmail;
			public int Greens;
			public int Yellows;
			public int Reds;
			public IEnumerable<Comment> Comments;
		}

		public class Comment
		{
			public string Email;	
			public string Content;
		}
	}
}
