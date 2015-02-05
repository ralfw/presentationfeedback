using System;
using Quartz.Impl;
using Quartz;
using Quartz.Spi;

namespace QuartzWithDI
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			// singleton worker
//			var op = new ConsoleOutputProvider ();
//			var worker = new UpdateWorker (op);
//			var workerFactory = new UpdateWorkerFactory (worker);

			// alternatively: per job worker
			var createWorker = new Func<UpdateWorker> (() => {
				var op2 = new ConsoleOutputProvider ();
				return new UpdateWorker (op2);
			});
			var workerFactory = new UpdateWorkerFactory (createWorker);


			var scheduler = StdSchedulerFactory.GetDefaultScheduler();
			scheduler.JobFactory = workerFactory;
			scheduler.Start();

			var job = JobBuilder.Create<UpdateWorker>()
								.Build();
				
			var trigger = TriggerBuilder.Create()
				.StartNow()
				.WithSimpleSchedule(x => x.WithIntervalInSeconds(1).WithRepeatCount(3))
				.Build();

			scheduler.ScheduleJob(job, trigger);
		}
	}


	class UpdateWorkerFactory : IJobFactory {
		Func<UpdateWorker> createWorker;

		public UpdateWorkerFactory(Func<UpdateWorker> createWorker) {
			this.createWorker = createWorker;
		}

		public UpdateWorkerFactory(UpdateWorker worker) {
			this.createWorker = () => worker;
		}
			
		#region IJobFactory implementation

		public IJob NewJob (TriggerFiredBundle bundle, IScheduler scheduler)
		{
			Console.WriteLine ("NewJob for: {0}", bundle.JobDetail.JobType);
			return this.createWorker();
		}

		public void ReturnJob (IJob job)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}


	class UpdateWorker : IJob {
		IOutputProvider op;

		public UpdateWorker(IOutputProvider op) {
			this.op = op;
		}


		#region IJob implementation

		public void Execute (IJobExecutionContext context)
		{
			var msg = string.Format ("{0}: job is running", DateTime.Now);
			this.op.WriteLine (msg);
		}

		#endregion
	}


	interface IOutputProvider {
		void WriteLine (string message);
	}

	class ConsoleOutputProvider : IOutputProvider {
		#region IOutputProvider implementation

		public void WriteLine (string message)
		{
			Console.WriteLine ("[{0}]", message);
		}

		#endregion
	}
}
