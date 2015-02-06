using afapp.body;
using afapp.body.data;
using System;
using afapp.body.contract;

namespace afapp.console
{
	internal class FakeEmailService : IEmailService
	{
		public void Notify_speaker(SpeakerNotificationData data)
		{
			Console.WriteLine("Dear {0}", data.Session.SpeakerName);
			Console.WriteLine("Thank you for speaking at '{0}'", data.ConferenceTitle);
			Console.WriteLine("Your session '{0}' on the {1} from {2} to {3} received the follows scores:",
				data.Session.Title, data.Session.Start.ToShortDateString(), data.Session.Start.ToShortTimeString(), 
				data.Session.End.ToShortTimeString());
			Console.WriteLine("Red: {0}", data.Reds);
			Console.WriteLine("Yellow: {0}", data.Yellows);
			Console.WriteLine("Green: {0}\n", data.Greens);
			Console.WriteLine("Best regards,");
			Console.WriteLine("The Semaphore feedback team");
		}
	}
}
