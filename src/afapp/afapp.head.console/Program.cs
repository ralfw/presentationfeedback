﻿using System;
using afapp.body;
using EventStore;
using System.Collections.Generic;

namespace afapp.head.console
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var es = new FileEventStore ("coapp.events");
			var repo = new Repository (es);
			var conf = new Conference ();
			var map = new Mapper ();
			var body = new Body (repo, conf, map);
			var head = new Head (body);

			head.Run (args);
		}
	}

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
