using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace _04_gui_client_wpf
{
    public static class Commands
    {
        public static readonly RoutedCommand Exit = new RoutedCommand();

        public static readonly RoutedCommand Connect = new RoutedCommand();
        public static readonly RoutedCommand Disconnect = new RoutedCommand();
        public static readonly RoutedCommand LoadDutyPlan = new RoutedCommand();

    }
}
