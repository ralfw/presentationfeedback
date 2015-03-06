using Contract.data;

namespace afapp.webui.FormModels
{
	public class FeedbackFormModel
	{
		public string SessionId { get; set; }
		public TrafficLightScores Score { get; set; }
		public string Comment { get; set; }
		public string Email { get; set; }
	}
}