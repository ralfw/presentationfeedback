﻿
using EventStore.Contract;
using Repository.data;

namespace coapp.body
{
	using Contract.data;
	using System.Collections.Generic;
	using System.Linq;

	public class Body
	{
		readonly CSVParser parser;
		readonly Repository.Repository repo;

		public Body (IEventStore es)	{
			this.repo = new Repository.Repository(es);
			this.parser = new CSVParser ();
		}

		public int Register_conference(string id, string title, string timeZone, string csvSessions) {
			this.repo.Store_conference (id, title, timeZone);
			var sessions = this.parser.ParseSessions (csvSessions);
			return this.repo.Store_sessions (id, timeZone, sessions);
		}

		public ConferenceCvsFeedback Generate_conference_feedback(string confId)
		{
			var scoredSessionData = Get_scored_sessions_for_conference(confId);
			var confOverviewData = Mapper.Map(scoredSessionData);
			return new ConferenceCvsFeedback{ 
				ConfTitle = confOverviewData.Title,
				Content = CsvMapper.Map(confOverviewData.Sessions)
			};
		}

		private IEnumerable<ScoredSessionData> Get_scored_sessions_for_conference(string confId)
		{
			return repo.Load_scored_sessions().Where(x => x.ConfId == confId);
		}
	}
}