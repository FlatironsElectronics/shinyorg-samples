﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using Shiny;
using Shiny.Net.Http;
using Shiny.Notifications;
using Xamarin.Forms;


namespace Sample
{
    public class PendingViewModel : SampleViewModel
    {
        readonly IHttpTransferManager httpTransfers;
        IDisposable? sub;


        public PendingViewModel()
        {
            this.httpTransfers = ShinyHost.Resolve<IHttpTransferManager>();
            this.Create = this.NavigateCommand<CreatePage>();
            this.Load = this.LoadingCommand(async () =>
            {
                var transfers = await httpTransfers.GetTransfers();
                this.Transfers = transfers
                    .Select(transfer =>
                    {
                        var vm = new HttpTransferViewModel
                        {
                            Identifier = transfer.Identifier,
                            Uri = transfer.Uri,
                            IsUpload = transfer.IsUpload,
                            Cancel = this.ConfirmCommand(
                                "Are you sure you want to cancel all transfers?",
                                async () =>
                                {
                                    await this.httpTransfers.Cancel(transfer.Identifier);
                                    this.Load.Execute(null);
                                }
                            )
                        };

                        ToViewModel(vm, transfer);
                        return vm;
                    })
                    .ToList();
            });
            this.CancelAll = this.LoadingCommand(async () =>
            {
                await httpTransfers.Cancel();
                this.Load.Execute(null);
            });
        }


        public ICommand Create { get; }
        public ICommand Load { get; }
        public ICommand CancelAll { get; }


        IList<HttpTransferViewModel> transfers;
        public IList<HttpTransferViewModel> Transfers
        {
            get => this.transfers;
            private set
            {
                this.transfers = value;
                this.RaisePropertyChanged();
            }
        }


        public override void OnAppearing()
        {
            base.OnAppearing();
            this.Load.Execute(null);
            ShinyHost
                .Resolve<INotificationManager>()
                .RequestAccess();

            this.sub = this.httpTransfers
                .WhenUpdated()
                .WithMetrics()
                .SubOnMainThread(
                    transfer =>
                    {
                        var vm = this.Transfers.FirstOrDefault(x => x.Identifier == transfer.Transfer.Identifier);
                        if (vm != null)
                        {
                            ToViewModel(vm, transfer.Transfer);
                            vm.TransferSpeed = Math.Round((decimal)transfer.BytesPerSecond / 1024, 2) + "Kb/s";
                            vm.EstimateTimeRemaining = Math.Round(transfer.EstimatedTimeRemaining.TotalMinutes, 1) + " min(s)";
                        }
                    },
                    ex => this.Alert(ex.ToString())
                );
        }


        public override void OnDisappearing()
        {
            base.OnDisappearing();
            this.sub?.Dispose();
        }


        static void ToViewModel(HttpTransferViewModel viewModel, HttpTransfer transfer)
        {
            viewModel.PercentComplete = transfer.PercentComplete;
            viewModel.PercentCompleteText = $"{transfer.PercentComplete * 100}%";
            viewModel.Status = transfer.Status.ToString();
        }
    }
}
