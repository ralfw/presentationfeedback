using afapp.body;
using CLAP;
using Contract.data;
using Contract.provider;
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
			TimeProvider.Configure (fixedNow);

			var vm = this.body.Generate_session_overview (confid);

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

		private static void Display_sessions(IEnumerable<Session> sessions) {
			foreach (var s in sessions)
				Console.WriteLine("{0}: {1}, {2}-{3}, {4}", s.Id, s.Title, s.Start, s.End, s.SpeakerName);
		}
	}
}
