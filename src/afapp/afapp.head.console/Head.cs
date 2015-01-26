using System;
using afapp.body;
using EventStore;
using System.Collections.Generic;

namespace afapp.head.console
{

	class Head {
		Body body;

		public Head(Body body) {
			this.body = body;
		}

		public void Run(string[] args) {
			var vm = this.body.GenerateSessionOverview (args [0]);

			Console.WriteLine ("# Sessions of {0} ({1})", vm.ConfTitle, vm.ConfId);
			Console.WriteLine ("## Active");
			Display_sessions (vm.ActiveSessions);
			Console.WriteLine ("## Inactive");
			Display_sessions (vm.InactiveSessions);
		}

		void Display_sessions(IEnumerable<SessionVM> sessions) {
			foreach (var s in sessions)
				Console.WriteLine("{0}: {1}, {2}-{3}, {4}", s.Id, s.Title, s.Start, s.End, s.SpeakerName);
		}
	}
}
