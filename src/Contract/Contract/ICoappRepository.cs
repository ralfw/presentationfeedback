using System.Collections.Generic;

namespace Contract
{
	using data;

	public interface ICoappRepository
	{
		void Store_conference(string id, string title);
		int Store_sessions(string conferenceId, IEnumerable<SessionParsed> sessions);
		IEnumerable<ScoredSessionData> Load_scored_sessions();
	}
}
