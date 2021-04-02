using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

using _04_gui_client_wpf.Model;

using _04_shared_domain.Data;

namespace _04_gui_client_wpf.Conversion
{
    public class DaysToDutiesTextOnDayConverter : IValueConverter
    {
        public DaysToDutiesTextOnDayConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime onDay = (DateTime)parameter;

            if (value is DaysModel daysModel)
            {
                IList<DutyModel> dutiesOnDay = daysModel.GetDutiesOnDay(onDay);

                string text = string.Join(
                    separator: " ", 
                        dutiesOnDay
                        .Select(x => $"[{x.DutyType} {x.Start:t}-{x.End:t}{(x.Deviations.Count > 0 ? ", " : "")}{string.Join("/", x.Deviations.Select(d => d.DeviationType))}]"));

                return text;
            }

            if (targetType == typeof(string))
            {
                return value.ToString();
            }

            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
