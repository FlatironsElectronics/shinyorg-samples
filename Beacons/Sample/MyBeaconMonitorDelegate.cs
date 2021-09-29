﻿using System;
using System.Threading.Tasks;
using Shiny.Beacons;


namespace Sample
{
    public class MyBeaconMonitorDelegate : IBeaconMonitorDelegate
    {
        readonly SampleSqliteConnection conn;
        public MyBeaconMonitorDelegate(SampleSqliteConnection conn) => this.conn = conn;
        public Task OnStatusChanged(BeaconRegionState newStatus, BeaconRegion region) => this.conn.InsertAsync(new ShinyEvent
        {
            Text = $"{region.Identifier} was {newStatus.ToString().ToLower()}",
            Detail = $"UUID: {region.Uuid} - M: {region.Major} - m: {region.Minor}",
            Timestamp = DateTime.UtcNow
        });
    }
}
