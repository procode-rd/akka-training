using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using _04_gui_client_wpf.Core;

using _04_shared_domain.Data;

namespace _04_gui_client_wpf.Model
{
    /// <summary>
    /// slave view on employments/duties
    /// </summary>
    public class DayModel : NotifyPropertyChanged, IEnumerable<DutyModel>
    {
        public DaysModel Days { get; }

        public DutiesModel Duties { get; }

        public DayModel(DaysModel days, DateTime date)
        {
            this.Day = date;
            this.Days = days;

            this.Duties = new DutiesModel();
        }

        // bindables
        public DateTime Day
        {
            get => this.Get<DateTime>();
            set => this.Set(value);
        }

        public ValidationType Status
        {
            get => this.Get<ValidationType>();
            set => this.Set(value);
        }

        public string Message
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        // api
        internal void AddDuty(DutyModel duty)
        {
            this.Duties.Add(duty);
        }

        internal void SetValidation(ValidationType validationType, string message)
        {
            this.Status = validationType;
            this.Message = message ?? string.Empty;
        }

        public IEnumerator<DutyModel> GetEnumerator()
            => this.Duties.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}
