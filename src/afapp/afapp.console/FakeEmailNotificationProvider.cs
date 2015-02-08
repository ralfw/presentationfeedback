using afapp.body.contract;
using afapp.body.data;
using System;

namespace afapp.console
{
	internal class FakeEmailNotificationProvider : INotificationProvider
	{
		public void Send_feedback(SpeakerNotificationData notification)
		{
			Console.WriteLine("Dear {0} @ {1}:", notification.SpeakerName, notification.SpeakerEmail);
			Console.WriteLine("Thank you for speaking at '{0}'", notification.ConfTitle);
			Console.WriteLine("Your session '{0}' on the {1} from {2} to {3} received the follows scores:",
								notification.Title, notification.Start.ToShortDateString(), 
								notification.Start.ToShortTimeString(), 
								notification.End.ToShortTimeString());
			Console.WriteLine("  Red: {0}", notification.Reds);
			Console.WriteLine("  Yellow: {0}", notification.Yellows);
			Console.WriteLine("  Green: {0}\n", notification.Greens);
			Console.WriteLine("Best regards,");
			Console.WriteLine("The Semaphore feedback team");
		}
	}
}
