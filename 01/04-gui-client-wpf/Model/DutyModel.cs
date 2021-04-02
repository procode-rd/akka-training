using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using _04_gui_client_wpf.Core;

using _04_shared_domain.Data;

namespace _04_gui_client_wpf.Model
{
    public class DutyModel: NotifyPropertyChanged
    {
        public DutyModel(DutyShard shard)
        {
            this.EmploymentId = shard.EmploymentId;
            this.DutyId = shard.DutyId;
            this.DutyType = shard.DutyType;
            this.Start = shard.Start;
            this.End = shard.End;
            this.Deviations = new ObservableCollection<DeviationModel>(shard.Deviations.Select(x => new DeviationModel(x)));
        }

        public long EmploymentId { get => this.Get<long>(); set => this.Set(value); }
        public long DutyId { get => this.Get<long>(); set => this.Set(value); }
        public string DutyType { get => this.Get<string>(); set => this.Set(value); }
        public DateTime Start { get => this.Get<DateTime>(); set => this.Set(value); }
        public DateTime End { get => this.Get<DateTime>(); set => this.Set(value); }
        public ObservableCollection<DeviationModel> Deviations { get; }
    }
}
