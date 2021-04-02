using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;

using _04_gui_client_wpf.Model;
using _04_gui_client_wpf.Transport;

using _04_shared_domain;
using _04_shared_domain.Data;
using _04_shared_domain.Request;

using Akka.Actor;

namespace _04_gui_client_wpf
{
    /// <summary>
    /// Provides local application logic.
    /// Calls appropriate api methods and fills in data kept within model.
    /// </summary>
    public class ApiService
    {
        private readonly MainWindowViewModel appModel;
        private readonly ActorSystemAdapter actorSystemAdapter;

        private IActorRef apiBridge;

        public ApiService(ActorSystemAdapter actorSystemAdapter, MainWindowViewModel model)
        {
            // in/out data model - for demo simplicity being set inside.
            this.appModel = model;

            // akka actor system handler
            this.actorSystemAdapter = actorSystemAdapter;
        }

        // public api
        public event EventHandler<string> LogLineGenerated;

        public void Dispose()
        {
            if (this.apiBridge != null)
            {
                this.Disconnect();
            }

            this.actorSystemAdapter.Dispose();
        }


        public bool Connect(string host, int port)
        {
            string remotePath = string.Empty;

            try
            {
                this.AppendLogLine($"Connecting to {host}:{port}...");

                // current instance of an actor bridge handling -in and -out commands
                this.apiBridge = this.actorSystemAdapter.BindToRemote(host, port, this.ApiResponseReceived, out remotePath);
            }
            catch (ActorNotFoundException)
            {
                this.AppendLogLine("Remote api system cannot be connected");
                return false;
            }

            this.appModel.IsConnected = this.apiBridge != null;

            if (this.appModel.IsConnected)
            {
                this.SendApiRequest(new ConnectRequest());

                this.appModel.ApiBridgePath = $"{this.apiBridge.Path} connected to {remotePath}";
                this.AppendLogLine(this.appModel.ApiBridgePath);
            }

            return this.appModel.IsConnected;
        }

        public void InitiateLoadDutyPlan(long customerId, long organizationId, DateTime from, DateTime to)
        {
            this.appModel?.Dispose();

            this.appModel.Dutyplan = new DutyplanModel(3, 4, from, to);

            this.SendApiRequest(new LoadDutyplanRequest(customerId, organizationId, from, to));
        }

        public void Disconnect()
        {
            if (this.apiBridge == null)
            {
                return;
            }

            // send to server
            this.SendApiRequest(new DisconnectRequest());

            this.AppendLogLine($"Closing self on request");

            this.apiBridge.Tell(PoisonPill.Instance);

            this.HandleDisconnected();
        }

        private void DisconnectFromDroppedLink()
        {
            if (this.apiBridge == null)
            {
                return;
            }

            this.apiBridge.Tell(PoisonPill.Instance);

            this.HandleDisconnected();
        }

        private void HandleDisconnected()
        {
            this.apiBridge = null;

            var text = "(disconnected)";

            this.appModel.Status =
                this.appModel.ApiBridgePath = text;

            this.appModel.IsConnected = false;

            this.AppendLogLine(text);
        }

        private void AppendLogLine(string logLine)
        {
            if (Application.Current.Dispatcher.Thread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                Application.Current.Dispatcher.Invoke(() => this.AppendLogLine(logLine));
                return;
            }

            logLine = $"{DateTime.Now}> {logLine}";

            this.appModel.LogLines.Add(logLine);
            this.LogLineGenerated?.Invoke(this, logLine);
        }

        private void SendApiRequest(ApiRequest request)
        {
            this.apiBridge.Tell(request);

            string text = request.ToString();
            this.appModel.Status = text;

            this.AppendLogLine(text);
        }

        private void ApiResponseReceived(ApiResponse response)
        {
            try
            {
                switch (response)
                {
                    case ConnectResponse connectResponse:
                        this.appModel.Status = $"Connected to Api ver: {connectResponse.Version} ({connectResponse.Message})";
                        return;

                    case ApiResponse<DisconnectRequest> _:
                        this.DisconnectFromDroppedLink();
                        break;

                    case ApiResponse<LoadDutyplanRequest, EmploymentShard[]> _:
                    case ApiResponse<LoadDutyplanRequest, DutyShard[]> _:
                    case ApiResponse<LoadDutyplanRequest, ValueShard[]> _:
                    case ApiResponse<LoadDutyplanRequest, ValidationFrame[]> _:
                        this.ProcessShards((IDataShard[])response.Result);
                        break;
                }

                this.appModel.Status = response.ToString();
            }
            finally
            {
                this.AppendLogLine(response.ToString());
            }
        }

        private void ProcessShards(IDataShard[] dataShards)
        {
            if (Application.Current.Dispatcher.Thread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                Application.Current.Dispatcher.Invoke(() => this.ProcessShards(dataShards));
                return;
            }

            foreach (IDataShard dataShard in dataShards)
            {
                switch (dataShard)
                {
                    case EmploymentShard employmentShard:
                        this.appModel.Dutyplan.AddEmployment(new EmploymentModel(employmentShard));
                        break;

                    case DutyShard dutyShard:
                        this.appModel.Dutyplan.AddDuty(new DutyModel(dutyShard));
                        break;

                    case ValueShard valueShard:
                        this.appModel.Dutyplan.AddValue(valueShard);
                        break;

                    case ValidationFrame validationFrame:
                        this.appModel.Dutyplan.AddValidationFrame(validationFrame);
                        break;

                    default:
                        AppendLogLine($"Shard type `{dataShard.GetType().FullName}` is not supported");
                        return;
                }
            }
        }
    }
}
