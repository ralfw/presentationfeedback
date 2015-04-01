using System;

namespace Contract.data
{
	public class DateTimeWithZone
	{
		private readonly DateTime utcDateTime;
		private readonly TimeZoneInfo timeZone;

		public DateTimeWithZone(DateTime dateTime, TimeZoneInfo timeZone)
		{
			utcDateTime = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified), timeZone);
			this.timeZone = timeZone;
		}

		public DateTime UtcTime { get { return utcDateTime; } }
		public DateTime LocalTime { get { return TimeZoneInfo.ConvertTime(utcDateTime, timeZone); } }
	}
}
