
namespace pfapp.webui.Providers
{
	using Contract;
	using Contract.data;
	using System.Net.Mail;
	using System.Text;

	public class EmailNotificationProvider : INotificationProvider
	{
		public void Send_feedback(SpeakerNotificationData data)
		{
			var message = BuildMessage(data);
			var smtp = new SmtpClient();
			smtp.Send(message);
		}

		private static MailMessage BuildMessage(SpeakerNotificationData data)
		{
			var message = new MailMessage("admin@presentationfeedback.apphb.com", data.SpeakerEmail);
			var builder = new StringBuilder();
			builder.AppendFormat("Dear {0}\n\n", data.SpeakerName);
			builder.AppendFormat("Thank you for speaking at '{0}'.\n\n", data.ConfTitle);
			builder.AppendFormat("Your session '{0}' on the {1} from {2} to {3} received the follows scores:\n",
								data.Title, data.Start.ToShortDateString(),
								data.Start.ToShortTimeString(),
								data.End.ToShortTimeString());
			builder.AppendFormat("  Red: {0}\n", data.Reds);
			builder.AppendFormat("  Yellow: {0}\n", data.Yellows);
			builder.AppendFormat("  Green: {0}\n\n", data.Greens);
			builder.AppendFormat("Comments:\n");
			builder.Append(data.Comments);
			builder.AppendFormat("\n\nBest regards,\n");
			builder.AppendFormat("The Presentation Feedback team");
			message.Body = builder.ToString();
			message.Subject = string.Format("Feedback result for session {0}", data.Title);
			return message;
		}
	}
}