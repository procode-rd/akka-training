using System;
using System.Collections.Generic;
using System.Text;

using _04_gui_client_wpf.Core;

using _04_shared_domain.Data;

namespace _04_gui_client_wpf.Model
{
    public class DutyplanModel : NotifyPropertyChanged, IDisposable
    {
        public DutyplanModel(long customerId, long organizationId, DateTime from, DateTime to)
        {
            this.CustomerId = customerId;
            this.OrganizationId = organizationId;
            this.From = from;
            this.To = to;

            this.Employments = new EmploymentsModel();
        }

        // bindable props

        public EmploymentsModel Employments 
        {
            get => this.Get<EmploymentsModel>();
            private set => this.Set(value);
        }
        public long CustomerId
        {
            get => this.Get<long>();
            private set => this.Set(value);
        }

        public long OrganizationId
        {
            get => this.Get<long>();
            private set => this.Set(value);
        }

        public DateTime From
        {
            get => this.Get<DateTime>();
            set => this.Set(value);
        }

        public DateTime To
        {
            get => this.Get<DateTime>();
            set => this.Set(value);
        }

        // api
        internal void AddEmployment(EmploymentModel employment)
        {
            this.Employments.Add(employment);
        }

        internal void AddDuty(DutyModel duty)
        {
            this.Employments.AddDuty(duty);
        }

        internal void AddValue(ValueShard valueShard)
        {
            this.Employments.AddValue(valueShard);
        }

        internal void AddValidationFrame(ValidationFrame validationFrame)
        {
            this.Employments.AddValidationFrame(validationFrame);
        }

        public void Dispose()
        {
            this.Employments?.Dispose();
        }
    }
}
