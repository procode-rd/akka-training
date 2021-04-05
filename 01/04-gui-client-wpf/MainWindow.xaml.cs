using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using _04_gui_client_wpf.Conversion;
using _04_gui_client_wpf.Model;
using _04_gui_client_wpf.Transport;

namespace _04_gui_client_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        private readonly ApiService apiService;
        private ActorSystemAdapter actorSystemAdapter;

        private MainWindowViewModel model;

        public MainWindow()
        {
            InitializeComponent();

            this.CommandBindings.Add(new CommandBinding(Commands.Exit, this.ExitCommand_Executed));
            this.CommandBindings.Add(new CommandBinding(Commands.Connect, this.ConnectCommand_Executed, (s, e) => e.CanExecute = this.model.IsDisconnected));
            this.CommandBindings.Add(new CommandBinding(Commands.Disconnect, this.DisconnectCommand_Executed, (s, e) => e.CanExecute = this.model.IsConnected));
            this.CommandBindings.Add(new CommandBinding(Commands.LoadDutyPlan, this.LoadDutyPlanCommand_Executed, (s, e) => e.CanExecute = this.model.IsConnected));

            this.actorSystemAdapter = new ActorSystemAdapter();
            this.model = new MainWindowViewModel();
            this.DataContext = this.model;

            this.apiService = new ApiService(this.actorSystemAdapter, this.model);
            // this.apiService.LogLineGenerated += Model_LogLineGenerated;
        }

        private void Model_LogLineGenerated(object sender, string line)
        {
            this.Log.ScrollIntoView(line);
        }

        private void LoadDutyPlanCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var from = this.model.Dutyplan.From;
            var to = this.model.Dutyplan.To;

            this.SetupGridDayColumns(from, to);

            this.apiService.InitiateLoadDutyPlan(
                this.model.Dutyplan.OrganizationId,
                this.model.Dutyplan.CustomerId,
                from,
                to);
        }

        private void DisconnectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.apiService.Disconnect();
        }

        private void ConnectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.apiService.Connect("localhost", 9000);
        }

        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void Dispose()
        {
            this.actorSystemAdapter?.Dispose();
            this.actorSystemAdapter = null;
        }

        private void SetupGridDayColumns(DateTime from, DateTime to)
        {
            for (int i = 5; i < this.DutyPlan.Columns.Count; i++)
            {
                this.DutyPlan.Columns.RemoveAt(i);
            }

            DateTime date = from.Date;

            var daysToCellText = new DaysToDutiesTextOnDayConverter();
            var daysToBrushConverter = new DaysToValidationBrushOnDayConverter();

            while (date < to)
            {
                FrameworkElementFactory cellContentText = new FrameworkElementFactory(typeof(Label));
                cellContentText.SetValue(Label.ContentProperty, new Binding(nameof(EmploymentModel.Days))
                {
                    Converter = daysToCellText,
                    ConverterParameter = date,
                });
                cellContentText.SetValue(DockPanel.MarginProperty, new Thickness(2));
                cellContentText.SetValue(BackgroundProperty, Brushes.White);

                FrameworkElementFactory cellTemplate = new FrameworkElementFactory(typeof(DockPanel));
                cellTemplate.SetValue(DockPanel.BackgroundProperty, new Binding(nameof(EmploymentModel.Days))
                {
                    Converter = daysToBrushConverter,
                    ConverterParameter = date,
                });
                cellTemplate.AppendChild(cellContentText);

                DataTemplate dataTemplate = new DataTemplate();
                dataTemplate.VisualTree = cellTemplate;

                this.DutyPlan.Columns.Add(new DataGridTemplateColumn()
                {
                    CellTemplate = dataTemplate,
                    Header = date.ToShortDateString(),
                    IsReadOnly = true,
                });

                date = date.AddDays(1);
            }
        }

        private void DateFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime? now = ((DatePicker)sender).SelectedDate;

            if (now == null)
            {
                return;
            }

            if (DateTo.SelectedDate < now)
            {
                DateTo.SelectedDate = now.Value.AddDays(1);
            }
            DateTo.DisplayDateStart = now.Value.AddDays(1);
        }
    }
}
