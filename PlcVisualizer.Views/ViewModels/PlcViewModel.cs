using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using PlcVisualizer.Interfaces;
using PlcVisualizer.Models;
using PlcVisualizer.Views.Helpers;
using UI.Controls;
using UI.Infrastructure;
using UI.Infrastructure.Interfaces;

namespace PlcVisualizer.ViewModels;

public partial class PlcViewModel : ViewModelCore<PlcViewModel>
{
    private readonly IPlcService _plcService;
    private readonly DispatcherTimer _timer = new();
    private readonly Dictionary<TagInfo, PlotData> _tagToPlotDataMap = new();
    private readonly List<SolidColorBrush> _brushes = new();

    [ObservableProperty]
    private NotificationMessageData _messageData;

    [ObservableProperty]
    private ObservableCollection<TagInfo> _tags;

    [ObservableProperty]
    private string _updateInterval;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    [NotifyCanExecuteChangedFor(nameof(DisconnectCommand))]
    private bool _isConnected;

    public PlcViewModel(
        IMessenger messenger,
        IMessageBoxService messageBox,
        IFileDialogService fileDialog,
        IErrorDialogService errorDialog,
        ILogger<PlcViewModel> logger,
        IPlcService plcService)
        : base(messenger, messageBox, fileDialog, errorDialog, logger)
    {
        _plcService = plcService;

        _timer.Tick += Timer_Tick;

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

        UpdateInterval = ConfigurationManager.AppSettings["UpdateRate"] ?? "50";
    }

    public ObservableCollection<PlotData> PlotsData { get; } = [];

    [RelayCommand]
    private void Apply()
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

    [RelayCommand(CanExecute = nameof(CanConnect))]
    private async Task ConnectAsync()
    {
        _plcService.IpAddress = ConfigurationManager.AppSettings["IPAddress"] ?? "192.168.10.22";
        _plcService.CipPath = ConfigurationManager.AppSettings["CIPPath"] ?? "0";
        _plcService.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["Timeout"] ?? "3000");

        IsBusy = true;

        try
        {
            var tags = await Task.Run(() =>
            {
                _plcService.Connect();
                var fetchedTags = _plcService.GetTags().ToObservableCollection();
                _plcService.StartScan();
                return fetchedTags;
            });

            Tags = tags;
            IsConnected = true;

            foreach (var ti in tags)
            {
                ti.PropertyChanged += Ti_PropertyChanged;
            }

            _timer.Start();
            Apply();
        }
        catch (Exception ex)
        {
            MessageData = new NotificationMessageData(ex.Message, NotificationMessageType.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanDisconnect))]
    private async Task DisconnectAsync()
    {
        IsBusy = true;

        await Task.Run(() => _plcService.Disconnect());

        IsConnected = false;
        Tags?.Clear();
        PlotsData.Clear();
        _timer.Stop();

        IsBusy = false;
    }

    private bool CanConnect()
    {
        return !IsConnected;
    }

    private bool CanDisconnect()
    {
        return IsConnected;
    }

    private void Ti_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TagInfo.IsChecked))
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
            MaxSamples = (int)(Convert.ToInt32(ConfigurationManager.AppSettings["History"] ?? "0") / _timer.Interval.TotalMilliseconds)
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

    private void Timer_Tick(object sender, EventArgs e)
    {
        foreach (var kvp in _tagToPlotDataMap)
        {
            kvp.Value.LastValue = new PointRef(0, Convert.ToDouble(kvp.Key.Value));
        }
    }
}