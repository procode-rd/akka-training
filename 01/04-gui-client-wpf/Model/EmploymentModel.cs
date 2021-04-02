using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using _04_gui_client_wpf.Core;

using _04_shared_domain.Data;

using ValueType = _04_shared_domain.Data.ValueType;

namespace _04_gui_client_wpf.Model
{
    public class EmploymentModel : NotifyPropertyChanged, IEnumerable<DayModel>
    {
        public EmploymentModel(EmploymentShard data)
        {
            this.Id = data.Id;
            this.Name = data.Name;
            this.Surname = data.Surname;

            this.Days = new DaysModel(this);
        }

        // bindables
        public long Id
        {
            get => this.Get<long>();
            set => this.Set(value);
        }

        public string Name
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string Surname
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public decimal Normtime 
        {
            get => this.Get<decimal>();
            set => this.Set(value);
        }

        public decimal Absence 
        {
            get => this.Get<decimal>();
            set => this.Set(value);
        }

        public DaysModel Days
        {
            get => this.Get<DaysModel>();
            private set => this.Set(value);
        }

        // api
        internal void AddDuty(DutyModel duty)
        {
            this.Days.AddDuty(duty);
            this.EmitPropertiesChanged(nameof(this.Days));
        }

        internal void AddValidationFrame(ValidationFrame validationFrame)
        {
            this.Days.AddValidationFrame(validationFrame);
            this.EmitPropertiesChanged(nameof(this.Days));
        }

        internal void AddValue(ValueShard valueShard)
        {
            switch (valueShard.ValueType)
            {
                case ValueType.Normtime:
                    this.Normtime = valueShard.Value;
                    break;

                case ValueType.Absence:
                    this.Absence = valueShard.Value;
                    break;

                default:
                    throw new NotSupportedException($"Unsupported value type: {valueShard.ValueType}");
            }
        }

        public IEnumerator<DayModel> GetEnumerator()
            => this.Days.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();

    }
}
