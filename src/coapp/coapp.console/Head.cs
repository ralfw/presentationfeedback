using System;
using coapp.body;
using System.IO;

namespace coapp.console
{
	class Head {
		Body body;

		public Head(Body body) {
			this.body = body;
		}
			
		public void Run(string[] args) {
			var confId = args [0];
			var confTitle = args [1];
			var csvSessions = File.ReadAllText (args [2]);

			var n = this.body.RegisterConference (confId, confTitle, csvSessions);

			Console.WriteLine ("Conference {0} registered with {1} sessions.", confId, n);
		}
	}
}
