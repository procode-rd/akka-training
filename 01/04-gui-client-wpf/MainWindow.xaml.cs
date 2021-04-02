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
            this.CommandBindings.Add(new CommandBinding(Commands.Connect, this.ConnectCommand_Executed, this.CanExecute_ConnectCommand));
            this.CommandBindings.Add(new CommandBinding(Commands.Disconnect, this.DisconnectCommand_Executed, this.CanExecute_DisconnectCommand));
            this.CommandBindings.Add(new CommandBinding(Commands.LoadDutyPlan, this.LoadDutyPlanCommand_Executed, this.CanExecute_LoadDutyPlanCommand));

            this.actorSystemAdapter = new ActorSystemAdapter();
            this.model = new MainWindowViewModel();
            this.DataContext = this.model;

            this.apiService = new ApiService(this.actorSystemAdapter, this.model);
            this.apiService.LogLineGenerated += Model_LogLineGenerated;
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


        private void Model_LogLineGenerated(object sender, string line)
        {
            this.Log.ScrollIntoView(line);
        }

        private void CanExecute_LoadDutyPlanCommand(object sender, CanExecuteRoutedEventArgs e) 
            => e.CanExecute = this.model.IsConnected;

        private void LoadDutyPlanCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var from = DateTime.Now.AddDays(-8);
            var to = DateTime.Now.AddDays(6);

            this.SetupGridDayColumns(from, to);

            this.apiService.InitiateLoadDutyPlan(
                3, 4, from, to);
        }

        private void CanExecute_DisconnectCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.model.IsConnected;
        }

        private void DisconnectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.apiService.Disconnect();
        }

        private void CanExecute_ConnectCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !this.model.IsConnected;
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
    }
}
