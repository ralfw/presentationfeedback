using EventStore.Contract;
using System;
using afapp.body.data;
using afapp.body.contract.data;

namespace afapp.body.providers
{

	public static class TimeProvider {
		public static void Configure() {
			Now = () => DateTime.Now;
		}

		public static void Configure(DateTime fixedNow) {
			if (fixedNow == DateTime.MinValue)
				Configure ();
			else
				Now = () => fixedNow;
		}

		public static Func<DateTime> Now { get; private set;}
	}
}
