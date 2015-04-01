using CLAP;
using coapp.body;
using System;
using System.IO;

namespace coapp.console
{
	class Head {
		readonly Body body;

		public Head(Body body) {
			this.body = body;
		}
		
		public void Run(string[] args) {
			Parser.Run (args, this);
		}

		// Time zone strings: https://msdn.microsoft.com/en-us/library/cc749073.aspx
		[Verb]
		public void Upload(
			[Required, Aliases("id")] string confId,
			[Required, Aliases("t,title")] string confTitle,
			[Required, Aliases("z")] string timeZone,
			[Required, Aliases("f,filename")] string csvfilename
		) {
			var csvSessions = File.ReadAllText (csvfilename);

			var n = this.body.Register_conference (confId, confTitle, timeZone, csvSessions);

			Console.WriteLine ("Conference {0} registered with {1} sessions.", confId, n);
		}

		[Verb]
		public void Feedback(
			[Required, Aliases("id")] string confId,
			[Aliases("d")] string dir)
		{
			var feedback = body.Generate_conference_feedback(confId);

			var fileName = Store_feedback (confId, dir, feedback);

			Console.WriteLine("Feedback written to file: '{0}'.", fileName);
		}

		static string Store_feedback (string confId, string dir, Contract.data.ConferenceCvsFeedback feedback)
		{
			var fileName = string.Format ("Feedback for {0}-{1}.txt", confId, feedback.ConfTitle);
			var filePath = Path.Combine (dir ?? string.Empty, fileName);
			File.WriteAllText (filePath, feedback.Content);
			return fileName;
		}
	}
}
