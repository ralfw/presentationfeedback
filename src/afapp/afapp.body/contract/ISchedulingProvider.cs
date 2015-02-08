using System;

namespace afapp.body
{
	public interface ISchedulingProvider
	{
		void Start(int schedulerRepeatInterval, Action action);
		void Stop();
	}
}