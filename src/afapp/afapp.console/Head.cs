using afapp.body;
using afapp.body.contract.data;
using CLAP;
using System;
using System.Collections.Generic;
using afapp.body.providers;

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
			TimeProvider.Configure (fixedNow);

			var vm = this.body.GenerateSessionOverview (confid);

			Console.WriteLine ("# Sessions of {0} ({1})", vm.ConfTitle, vm.ConfId);
			Console.WriteLine ("## Active");
			Display_sessions (vm.ActiveSessions);
			Console.WriteLine ("## Inactive");
			Display_sessions (vm.InactiveSessions);
			Environment.Exit(0);
		}


		[Verb(Aliases = "vote")] 
		public void Store_feedback(
			[Required, Aliases("id")]	string sessionId,
			[Required, Aliases("e")] 	string email,
			[Required, Aliases("s")] 	TrafficLightScores score, 
			[DefaultValue(""), Aliases("c")] string comment)
		{
			body.Store_feedback (sessionId, score, comment, email);
			Console.WriteLine("Thank you for your feedback!");
			Environment.Exit(0);
		}

		[Verb(Aliases = "startspeakernotification")]
		public void Start_speaker_notification(
			[Aliases("now,n")]					DateTime fixedNow,
			[DefaultValue(20), Aliases("f")] 	int feedbackPeriod,
			[DefaultValue(5), Aliases("s")] 	int schedulerRepeatInterval)
		{
			TimeProvider.Configure(fixedNow);

			body.Start_background_speaker_notification(feedbackPeriod, schedulerRepeatInterval);

			Console.WriteLine("Scheduling... - Press any key to stop");
			Console.ReadKey();

			body.Stop_speaker_notification();
			Console.WriteLine("Scheduler shutdown!");
			Environment.Exit(0);
		}


		private static void Display_sessions(IEnumerable<SessionOverview.Session> sessions) {
			foreach (var s in sessions)
				Console.WriteLine("{0}: {1}, {2}-{3}, {4}", s.Id, s.Title, s.Start, s.End, s.SpeakerName);
		}
	}
}
