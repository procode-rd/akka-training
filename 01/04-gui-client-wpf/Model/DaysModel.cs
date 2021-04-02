using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using _04_gui_client_wpf.Core;

using _04_shared_domain.Data;

namespace _04_gui_client_wpf.Model
{
    public class DaysModel : NotifyPropertyChanged, IEnumerable<DayModel>, INotifyCollectionChanged, IDisposable
    {
        private Dictionary<DateTime, DayModel> days;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public DaysModel(EmploymentModel employment)
        {
            this.days = new Dictionary<DateTime, DayModel>();

            this.Employment = employment;
        }

        public EmploymentModel Employment
        {
            get;
            private set;
        }

        public DutiesModel Duties
        {
            get => this.Get<DutiesModel>();
            private set => this.Set(value);
        }

        internal IList<DutyModel> GetDutiesOnDay(DateTime onDay)
        {
            return this.days.TryGetValue(onDay, out var dayModel)
                ? (IList<DutyModel>)dayModel.Duties
                : Array.Empty<DutyModel>();
        }

        internal DayModel GetDay(DateTime when)
        {
            return this.days.TryGetValue(when.Date, out var day)
                ? day
                : default;
        }

        internal void AddDuty(DutyModel duty)
        {
            if (!this.days.TryGetValue(duty.Start.Date, out var dayModel))
            {
                dayModel = new DayModel(this, duty.Start.Date);
                this.days.Add(dayModel.Day, dayModel);
            }

            dayModel.AddDuty(duty);
        }

        internal void AddValidationFrame(ValidationFrame validationFrame)
        {
            if (validationFrame.EmploymentId != this.Employment.Id)
            {
                throw new InvalidOperationException("Wrong destination place");
            }

            if (!this.days.TryGetValue(validationFrame.When.Date, out var dayModel))
            {
                dayModel = new DayModel(this, validationFrame.When.Date);
                this.days.Add(dayModel.Day, dayModel);
            }

            dayModel.SetValidation(validationFrame.ValidationType, validationFrame.Message);
        }

        // priv
        public void Dispose()
        {
        }

        public IEnumerator<DayModel> GetEnumerator()
            => this.days.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}
