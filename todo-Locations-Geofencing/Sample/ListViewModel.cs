﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Shiny;
using Shiny.Locations;


namespace Sample
{
    public class ListViewModel : ViewModel
    {
        readonly IGeofenceManager geofenceManager;
        readonly IDialogs dialogs;


        public ListViewModel()
        {
            this.geofenceManager = ShinyHost.Resolve<IGeofenceManager>();

            this.Create = navigator.NavigateCommand("CreateGeofence");
            this.DropAllFences = ReactiveCommand.CreateFromTask(
                async _ =>
                {
                    var confirm = await this.dialogs.Confirm("Are you sure you wish to drop all geofences?");
                    if (confirm)
                    {
                        await this.geofenceManager.StopAllMonitoring();
                        await this.LoadRegions();
                    }
                }
            );
        }


        public ICommand Create { get; }
        public ICommand DropAllFences { get; }

        public IList<GeofenceRegionViewModel> Geofences { get; private set; } = new List<GeofenceRegionViewModel>();
        public IList<GeofenceEvent> Events { get; private set; } = new List<GeofenceEvent>();


        public override async void OnAppearing()
        {
            base.OnAppearing();
            await this.LoadRegions();
        }


        async Task LoadRegions()
        {
            var geofences = await this.geofenceManager.GetMonitorRegions();

            this.Geofences = geofences
                .Select(region => new GeofenceRegionViewModel
                {
                    Region = region,
                    Remove = ReactiveCommand.CreateFromTask(async _ =>
                    {
                        var confirm = await this.dialogs.Confirm("Are you sure you wish to remove geofence - " + region.Identifier);
                        if (confirm)
                        {
                            await this.geofenceManager.StopMonitoring(region.Identifier);
                            await this.LoadRegions();
                        }
                    }),
                    RequestCurrentState = ReactiveCommand.CreateFromTask(async _ =>
                    {
                        GeofenceState? status = null;
                        using (var cancelSrc = new CancellationTokenSource())
                        {
                            //using (this.dialogs.Loading("Requesting State for " + region.Identifier, cancelSrc.Cancel))
                                status = await this.geofenceManager.RequestState(region, cancelSrc.Token);
                        }

                        if (status != null)
                        {
                            await Task.Delay(2000);
                            await this.dialogs.Alert($"{region.Identifier} status is {status}");
                        }
                    })
                })
                .ToList();

            this.RaisePropertyChanged(nameof(this.Geofences));
        }
    }
}
