using CLAP;
using Contract.provider;
using nsapp.body;
using System;

namespace nsapp.console
{
	internal class Head
	{
		private readonly Body body;

		public Head(Body body)
		{
			this.body = body;
		}

		public void Run(string[] args)
		{
			Parser.Run(args, this);
		}


		[Verb(Aliases = "start")]
		public void Start_speaker_notification(
			[Aliases("now,n")] DateTime fixedNow,
			[DefaultValue(20), Aliases("f")] int feedbackPeriod,
			[DefaultValue(5), Aliases("s")] int schedulerRepeatInterval)
		{
			TimeProvider.Configure(fixedNow);

			body.Start_background_speaker_notification(feedbackPeriod, schedulerRepeatInterval);

			Console.WriteLine("Scheduling... - Press any key to stop");
			Console.ReadKey();

			body.Stop_speaker_notification();
			Console.WriteLine("Scheduler shutdown!");
			Environment.Exit(0);
		}
	}
}
