using Quartz;
using System;
using System.IO;
using System.Web.Hosting;

namespace QuartzSpike.QuartzJobs
{
	public class UpdateFileJob : IJob
	{
		public void Execute(IJobExecutionContext context)
		{
			using (var writer = new StreamWriter(HostingEnvironment.MapPath(@"~/Storage/dummyData.txt"), false))
			{
				UpdateFile(writer);
			}
		}

		private static void UpdateFile(StreamWriter writer)
		{
			writer.Write("Last updated: " + DateTime.Now);
		}
	}
}