using System;
using coapp.body;
using System.IO;
using CLAP;

namespace coapp.console
{
	class Head {
		Body body;

		public Head(Body body) {
			this.body = body;
		}


		public void Run(string[] args) {
			Parser.Run (args, this);
		}


		[Verb]
		public void Upload(
			[Required, Aliases("id")] string confId,
			[Required, Aliases("title, t")] string confTitle,
			[Required, Aliases("filename, f")] string csvfilename
		) {
			var csvSessions = File.ReadAllText (csvfilename);

			var n = this.body.RegisterConference (confId, confTitle, csvSessions);

			Console.WriteLine ("Conference {0} registered with {1} sessions.", confId, n);
		}
	}
}
