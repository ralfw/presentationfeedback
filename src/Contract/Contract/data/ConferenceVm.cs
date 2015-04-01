
using System;

namespace Contract.data
{
	public struct ConferenceVm
	{
		public string Id;
		public string Title;
		public TimeZoneInfo TimeZone;
		public DateTimeWithZone Start;
		public DateTimeWithZone End;
	}
}
