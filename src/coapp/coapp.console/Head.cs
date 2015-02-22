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

		[Verb]
		public void Upload(
			[Required, Aliases("id")] string confId,
			[Required, Aliases("title, t")] string confTitle,
			[Required, Aliases("filename, f")] string csvfilename
		) {
			var csvSessions = File.ReadAllText (csvfilename);

			var n = this.body.Register_conference (confId, confTitle, csvSessions);

			Console.WriteLine ("Conference {0} registered with {1} sessions.", confId, n);
		}

		[Verb]
		public void Feedback(
			[Required, Aliases("id")] string confId,
			[Aliases("d")] string dir)
		{
			var feedback = body.Generate_conference_feedback(confId);
			var fileName = string.Format("Feedback for {0}-{1}.txt", confId, feedback.ConfTitle);

			using (var writer = new StreamWriter(Path.Combine(dir ?? string.Empty, fileName)))
			{
				writer.Write(feedback.Content);
			}

			Console.WriteLine("Feedback written to file: '{0}'.", fileName);
		}
	}
}
