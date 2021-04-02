using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

using _04_shared_domain.Data;

namespace _04_gui_client_wpf.Model
{
    public class DutiesModel : ObservableCollection<DutyModel>, IList<DutyModel>
    {
        public DutiesModel()
        {
        }
    }
}
