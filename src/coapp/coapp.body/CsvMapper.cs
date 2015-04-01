

namespace coapp.body
{
	using data;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Text;

	class CsvMapper {
		public static string Map(IEnumerable<ConferenceFeedbackData.Session> sessions)
		{
			var headers = Build_column_headers();
			var lines = Build_csv_lines(sessions);
			return Assemble_csv_lines(headers, lines);
		}

		private static string Build_column_headers()
		{
			return "Id	Title	Start	End	Speaker name	Speaker email	Greens	Yellows	Reds	Comments";
		}

		private static IEnumerable<string> Build_csv_lines(IEnumerable<ConferenceFeedbackData.Session> sessions)
		{
			var result = new List<string>();
			foreach (var session in sessions)
			{
				Build_csv_line(session, result);
			}
			return result;
		}

		private static void Build_csv_line(ConferenceFeedbackData.Session session, ICollection<string> lines)
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.Append(session.Id).Append("\t");
			stringBuilder.Append(session.Title).Append("\t");
			stringBuilder.Append(session.Start.LocalTime.ToString("s",DateTimeFormatInfo.InvariantInfo)).Append("\t");
			stringBuilder.Append(session.End.LocalTime.ToString("s", DateTimeFormatInfo.InvariantInfo)).Append("\t");
			stringBuilder.Append(session.SpeakerName).Append("\t");
			stringBuilder.Append(session.SpeakerEmail).Append("\t");
			stringBuilder.Append(session.Greens).Append("\t");
			stringBuilder.Append(session.Yellows).Append("\t");
			stringBuilder.Append(session.Reds).Append("\t");
			var comments = session.Comments.Where(x => !string.IsNullOrWhiteSpace(x.Content));
			var commentsString = string.Join("\n", comments.Select(x => string.Format("{0}: {1}", x.Email, x.Content)));
			stringBuilder.Append("\"").Append(commentsString).Append("\"");
			lines.Add(stringBuilder.ToString());
		}

		private static string Assemble_csv_lines(string columnHeaders, IEnumerable<string> lines)
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.Append(columnHeaders).Append("\n");
			stringBuilder.Append(string.Join("\n", lines));
			return stringBuilder.ToString();
		}
	}
}
