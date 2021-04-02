using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using _04_shared_domain.Data;

namespace _04_gui_client_wpf.Model
{
    public class EmploymentsModel : ObservableCollection<EmploymentModel>, IDisposable
    {
        public EmploymentsModel()
        {
        }

        // api

        internal void AddDuty(DutyModel duty)
        {
            this[duty.EmploymentId]?.AddDuty(duty);
        }

        internal void AddValue(ValueShard valueShard)
        {
            this[valueShard.EmploymentId]?.AddValue(valueShard);
        }

        internal void AddValidationFrame(ValidationFrame validationFrame)
        {
            this[validationFrame.EmploymentId]?.AddValidationFrame(validationFrame);
        }


        public void Dispose()
        {
            this.Clear();
        }

        // priv

        private EmploymentModel this[long id]
        {
            get => this.FirstOrDefault(x => x.Id == id);
        }
    }
}
