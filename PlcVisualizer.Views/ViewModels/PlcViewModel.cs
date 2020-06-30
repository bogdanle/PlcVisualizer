using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using PlcVisualizer.Interfaces;
using PlcVisualizer.Models;
using PlcVisualizer.Views.Helpers;
using Prism.Commands;
using UI.Controls;
using UI.Infrastructure;
using Unity;

namespace PlcVisualizer.ViewModels
{
    public class PlcViewModel : ViewModelCore<PlcViewModel>
    {
        private readonly IPlcService _plcService;
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private readonly Dictionary<TagInfo, PlotData> _tagToPlotDataMap = new Dictionary<TagInfo, PlotData>();
        private readonly List<SolidColorBrush> _brushes = new List<SolidColorBrush>();
        private string _updateInterval;
        private ObservableCollection<TagInfo> _tags;
        private NotificationMessageData _messageData;
        private bool _isConnected;
        private ICommand _connectCommand;
        private ICommand _applyCommand;
        private ICommand _disconnectCommand;

        public PlcViewModel(IUnityContainer container, IPlcService plcService)
            : base(container)
        {
            _plcService = plcService;

            _timer.Tick += Timer_Tick;

            // Colors that will be used for charts from 1 to 10 (the max)
            _brushes.Add(Brushes.SteelBlue);
            _brushes.Add(Brushes.SeaGreen);
            _brushes.Add(Brushes.OrangeRed);
            _brushes.Add(Brushes.Purple);
            _brushes.Add(Brushes.DarkRed);
            _brushes.Add(Brushes.SlateGray);
            _brushes.Add(Brushes.Magenta);
            _brushes.Add(Brushes.Brown);
            _brushes.Add(Brushes.Gold);
            _brushes.Add(Brushes.Maroon);

            _updateInterval = ConfigurationManager.AppSettings["UpdateRate"];
        }

        public override void OnRefresh()
        {
        }

        public ObservableCollection<PlotData> PlotsData { get; } = new ObservableCollection<PlotData>();

        public NotificationMessageData MessageData
        {
            get => _messageData;
            set => SetProperty(ref _messageData, value);
        }

        public ObservableCollection<TagInfo> Tags
        {
            get => _tags;
            set => SetProperty(ref _tags, value);
        }

        public string UpdateInterval
        {
            get => _updateInterval;
            set => SetProperty(ref _updateInterval, value);
        }

        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        public ICommand ApplyCommand => _applyCommand ?? (_applyCommand = new DelegateCommand(OnApply));

        public ICommand ConnectCommand => _connectCommand ?? (_connectCommand = new DelegateCommand(OnConnect, ConnectCanExecute).ObservesProperty(() => IsConnected));

        public ICommand DisconnectCommand => _disconnectCommand ?? (_disconnectCommand = new DelegateCommand(OnDisconnect, DisconnectCanExecute).ObservesProperty(() => IsConnected));

        public async Task ConnectAsync()
        {
            _plcService.IpAddress = ConfigurationManager.AppSettings["IPAddress"];
            _plcService.CipPath = ConfigurationManager.AppSettings["CIPPath"];
            _plcService.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["Timeout"]);

            IsBusy = true;

            await Task.Run(() =>
            {
                try
                {
                    _plcService.Connect();
                    var tags = _plcService.GetTags().ToObservableCollection();

                    _plcService.StartScan();

                    IsConnected = true;

                    Dispatcher.Invoke(() =>
                    {
                        Tags = tags;

                        foreach (var ti in tags)
                        {
                            ti.PropertyChanged += Ti_PropertyChanged;
                        }

                        _timer.Start();

                        OnApply();

                        IsBusy = false;
                    });
                }
                catch (Exception ex)
                {
                    MessageData = new NotificationMessageData(ex.Message, NotificationMessageType.Error);
                }
                finally
                {
                    IsBusy = false;
                }
            });
        }

        public void Disconnect()
        {
            IsBusy = true;

            Task.Run(() =>
            {
                _plcService.Disconnect();

                IsConnected = false;

                Dispatcher.Invoke(() =>
                {
                    Tags?.Clear();
                    PlotsData.Clear();
                    _timer.Stop();
                });

                IsBusy = false;
            });
        }

        private void Ti_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                var ti = (TagInfo)sender;
                if (ti.IsChecked)
                {
                    SubscribeTag(ti);
                }
                else
                {
                    UnsubscribeTag(ti);
                }
            }
        }

        private void SubscribeTag(TagInfo ti)
        {
            if (PlotsData.Count >= 10)
            {
                MessageData = new NotificationMessageData("You can select up to 10 items", NotificationMessageType.Error);
                ti.IsChecked = false;
                return;
            }

            var obj = new PlotData
            {
                Label = ti.Name,
                Foreground = _brushes[PlotsData.Count],
                MaxSamples = (int)(Convert.ToInt32(ConfigurationManager.AppSettings["History"]) / _timer.Interval.TotalMilliseconds)
            };

            _tagToPlotDataMap.Add(ti, obj);

            PlotsData.Add(obj);
        }

        private void UnsubscribeTag(TagInfo ti)
        {
            if (_tagToPlotDataMap.ContainsKey(ti))
            {
                var obj = _tagToPlotDataMap[ti];
                _tagToPlotDataMap.Remove(ti);

                PlotsData.Remove(obj);
            }
        }

        private void OnApply()
        {
            try
            {
                var interval = Convert.ToInt32(UpdateInterval);
                _plcService.UpdateRate = interval;
                _timer.Interval = TimeSpan.FromMilliseconds(interval);
            }
            catch (Exception ex)
            {
                MessageData = new NotificationMessageData(ex.Message, NotificationMessageType.Error);
            }
        }

        private void OnConnect()
        {
            ConnectAsync();
        }

        private bool ConnectCanExecute()
        {
            return !IsConnected;
        }

        private bool DisconnectCanExecute()
        {
            return IsConnected;
        }

        private void OnDisconnect()
        {
            Disconnect();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Get the latest value of the tag
            foreach (var kvp in _tagToPlotDataMap)
            {
                kvp.Value.LastValue = new PointRef(0, Convert.ToDouble(kvp.Key.Value));
            }
        }
    }
}