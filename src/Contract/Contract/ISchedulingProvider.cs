using System;

namespace Contract
{
	public interface ISchedulingProvider
	{
		void Start(int schedulerRepeatInterval, Action action);
		void Stop();
	}
}