using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

using _04_gui_client_wpf.Model;

using _04_shared_domain.Data;

namespace _04_gui_client_wpf.Conversion
{
    public class DaysToValidationBrushOnDayConverter : IValueConverter
    {
        public DaysToValidationBrushOnDayConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime onDay = (DateTime)parameter;

            if (value is DaysModel daysModel && targetType == typeof(Brush))
            {
                DayModel dayModel = daysModel.GetDay(onDay);

                if (dayModel == null)
                {
                    return Brushes.White;
                }

                switch (dayModel.Status)
                {
                    case ValidationType.Ok: return Brushes.White;
                    case ValidationType.Notice: return Brushes.LightBlue;
                    case ValidationType.Warning: return Brushes.Yellow;
                    case ValidationType.Error: return Brushes.Red;
                    default: return Brushes.DeepPink;
                }
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
