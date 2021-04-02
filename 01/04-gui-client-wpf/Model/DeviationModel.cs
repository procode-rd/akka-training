using System;
using System.Collections.Generic;
using System.Text;

using _04_gui_client_wpf.Core;

using _04_shared_domain.Data;

namespace _04_gui_client_wpf.Model
{
    public class DeviationModel : NotifyPropertyChanged
    {
        public DeviationModel(DeviationShard shard)
        {
            this.Id = shard.Id;
            this.DutyId = shard.DutyId;
            this.DeviationId = shard.DeviationId;
            this.DeviationType = shard.DeviationType;
            this.Start = shard.Start;
            this.End = shard.End;
        }

        public long Id { get => this.Get<long>(); set => this.Set(value); }
        public long DutyId { get => this.Get<long>(); set => this.Set(value); }
        public long DeviationId { get => this.Get<long>(); set => this.Set(value); }
        public string DeviationType { get => this.Get<string>(); set => this.Set(value); }
        public DateTime Start { get => this.Get<DateTime>(); set => this.Set(value); }
        public DateTime End { get => this.Get<DateTime>(); set => this.Set(value); }
    }
}
