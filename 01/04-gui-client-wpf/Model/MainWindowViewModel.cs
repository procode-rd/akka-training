using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using _04_gui_client_wpf.Core;
using _04_gui_client_wpf.Transport;

using _04_shared_domain;
using _04_shared_domain.Data;
using _04_shared_domain.Request;

using Akka.Actor;

namespace _04_gui_client_wpf.Model
{
    public class MainWindowViewModel : NotifyPropertyChanged, IDisposable
    {

        public MainWindowViewModel()
        {
            this.Status = "Ready to connect";
            this.ApiBridgePath = "(connect first)";

            this.Dutyplan = new DutyplanModel(-1, -1, DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7));
            this.LogLines = new ObservableCollection<string>();
        }
        
        // bound props

        public bool IsDisconnected
        {
            get => !this.IsConnected;
        }

        public bool IsConnected
        {
            get => this.Get<bool>();
            set
            {
                this.Set(value);
                this.EmitPropertiesChanged(nameof(IsDisconnected));
            }
        }

        public string Status
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string ApiBridgePath
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public DutyplanModel Dutyplan
        {
            get => this.Get<DutyplanModel>();
            set => this.Set(value);
        }

        public ObservableCollection<string> LogLines
        {
            get => this.Get<ObservableCollection<string>>();
            private set => this.Set(value);
        }

        public void Dispose()
        {
            this.Dutyplan?.Dispose();
            this.Dutyplan = null;

            this.LogLines.Clear();
        }
    }
}
