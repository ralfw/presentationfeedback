using afapp.body.contract;
using afapp.body.contract.data;
using afapp.body.data;
using afapp.body.domain;
using afapp.body.providers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace afapp.body.helpers
{
	public static class IEnumExtensions {
		public static void ForEach<T>(this IEnumerable<T> list, Action<T> process) {
			foreach (var e in list)
				process (e);
		}
	}
}