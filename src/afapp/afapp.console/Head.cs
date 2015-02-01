using afapp.body;
using CLAP;
using System;
using System.Collections.Generic;

namespace afapp.console
{
	class Head {
		readonly Body body;

		public Head(Body body) {
			this.body = body;
		}

		public void Run(string[] args) {
			Parser.Run (args, this);
		}


		[Verb]
		public void Overview(
			[Required, Aliases("id")] 	string confid, 
			[Aliases("now,n")]			DateTime fixedNow) 
		{
			this.body.Now = BuildCurrentTimeProvider (fixedNow);

			var vm = this.body.GenerateSessionOverview (confid);

			Console.WriteLine ("# Sessions of {0} ({1})", vm.ConfTitle, vm.ConfId);
			Console.WriteLine ("## Active");
			Display_sessions (vm.ActiveSessions);
			Console.WriteLine ("## Inactive");
			Display_sessions (vm.InactiveSessions);
		}


		[Verb(Aliases = "vote")]
		private void Store_feedback(
			[Required] string sessionId,
			[Required] string confId,
			[Required] string email,
			[Required] TrafficLightScore score, 
			string comment)
		{
			body.Store_feedback(
				new FeedbackData {
					SessionId = sessionId,
					ConfId = confId, 
					Email = email, 
					Score = score, 
					Comment = comment
			});
			Console.WriteLine("Thank you for your feedback!");
		}


		private static void Display_sessions(IEnumerable<SessionVM> sessions) {
			foreach (var s in sessions)
				Console.WriteLine("{0}: {1}, {2}-{3}, {4}", s.Id, s.Title, s.Start, s.End, s.SpeakerName);
		}


		private static Func<DateTime> BuildCurrentTimeProvider(DateTime fixedNow) {
			if (fixedNow == DateTime.MinValue)
				return () => DateTime.Now;
			else
				return () => fixedNow;
		}
	}
}
