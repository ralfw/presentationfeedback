using System;
using EventStore.Contract;

namespace afapp.body
{
	public class Body
	{
		Repository repo;
		Conference conf;
		Mapper map;

		public Body (Repository repo, Conference conf, Mapper map) {
			this.repo = repo;
			this.conf = conf;
			this.map = map;
		}


		public SessionOverviewVM GenerateSessionOverview(string confId) {
			return new SessionOverviewVM{ 
				ConfId = confId,
				ConfTitle = "Conference #1",
				ActiveSessions = new[]{
					new SessionVM{
						Id = "s1",
						Title = "Session #1",
						Start = new DateTime(2015,1,26, 10,0,0),
						End = new DateTime(2015,1,26, 10,30,0),
						SpeakerName = "Speaker #1"
					},
					new SessionVM{
						Id = "s2",
						Title = "Session #2",
						Start = new DateTime(2015,1,26, 10,30,0),
						End = new DateTime(2015,1,26, 11,0,0),
						SpeakerName = "Speaker #2"
					}
				},
				InactiveSessions = new[]{
					new SessionVM{
						Id = "s3",
						Title = "Session #3",
						Start = new DateTime(2015,1,26, 15,0,0),
						End = new DateTime(2015,1,26, 16,30,0),
						SpeakerName = "Speaker #3"
					}
				}
			};
		}
	}
		

	public class Conference {

	}


	public class Repository {
		public Repository(IEventStore es) {

		}
	}

	public class ConferenceData {

	}


	public class Mapper {

	}
}

