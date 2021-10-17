﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Shiny;
using Shiny.Jobs;
using Shiny.Notifications;


namespace Sample
{
    public class SampleJob : IJob
    {
        readonly INotificationManager notificationManager;
        public SampleJob(INotificationManager notificationManager)
            => this.notificationManager = notificationManager;


        public async Task Run(JobInfo jobInfo, CancellationToken cancelToken)
        {
            await this.notificationManager.Send(
                "Job Started",
                $"{jobInfo.Identifier} Started"
            );
            var seconds = jobInfo.Parameters.Get("SecondsToRun", 10);
            await Task.Delay(TimeSpan.FromSeconds(seconds), cancelToken);

            await this.notificationManager.Send(
                "Job Finished",
                $"{jobInfo.Identifier} Finished"
            );
        }
    }
}
