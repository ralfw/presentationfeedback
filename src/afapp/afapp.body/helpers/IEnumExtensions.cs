using System;
using System.Collections.Generic;

namespace afapp.body.helpers
{
	public static class IEnumExtensions {
		public static void ForEach<T>(this IEnumerable<T> list, Action<T> process) {
			foreach (var e in list)
				process (e);
		}
	}
}