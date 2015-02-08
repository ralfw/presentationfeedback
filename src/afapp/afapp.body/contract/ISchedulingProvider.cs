using System;

namespace afapp.body.contract
{
	public interface ISchedulingProvider
	{
		void Start(int schedulerRepeatInterval, Action action);
		void Stop();
	}
}