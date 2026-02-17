using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using PlcVisualizer.Interfaces;
using PlcVisualizer.Models;

namespace PlcVisualizer.Services;

public sealed class MockPlcService : IPlcService
{
    private readonly DispatcherTimer _timer = new();
    private readonly Dictionary<TagInfo, Func<double>> _map = new();
    private readonly Random _random = new();
    private double _x;

    public MockPlcService()
    {
        _timer.Tick += Timer_Tick;
        _timer.Interval = TimeSpan.FromMilliseconds(20);
    }

    public event Action<TagInfo> TagUpdated;

    public string IpAddress { get; set; } = "192.168.10.1";

    public string CipPath { get; set; } = "0";

    public int Timeout { get; set; } = 3000;

    public int UpdateRate { get; set; } = 1000;

    /// <inheritdoc/>
    public void Connect()
    {
        Thread.Sleep(1000);
    }

    /// <inheritdoc/>
    public void Disconnect()
    {
        _timer.Stop();

        Thread.Sleep(500);

        _map.Clear();
    }

    /// <inheritdoc/>
    public void StartScan()
    {
        _timer.Start();
    }

    /// <inheritdoc/>
    public IEnumerable<TagInfo> GetTags()
    {
        var tags = new List<TagInfo>
        {
            new TagInfo { Name = "TEST1.Tag1", DataType = "float", Group = "TEST1", Quality = "good" },
            new TagInfo { Name = "TEST1.Tag2", DataType = "bool", Group = "TEST1", Quality = "good" },
            new TagInfo { Name = "TEST1.Tag3", DataType = "float", Group = "TEST1", Quality = "good" },
            new TagInfo { Name = "TEST2.Tag1", DataType = "double", Group = "TEST2", Quality = "good" },
            new TagInfo { Name = "TEST2.Tag2", DataType = "float", Group = "TEST2", Quality = "good" },
            new TagInfo { Name = "TEST2.Tag3", DataType = "float", Group = "TEST2", Quality = "good" },
            new TagInfo { Name = "TEST3.Tag1", DataType = "double", Group = "TEST3", Quality = "good" },
            new TagInfo { Name = "TEST3.Tag2", DataType = "float", Group = "TEST3", Quality = "terrible" },
            new TagInfo { Name = "TEST3.Tag3", DataType = "float", Group = "TEST3", Quality = "poor" },
            new TagInfo { Name = "TEST3.Tag4", DataType = "float", Group = "TEST3", Quality = "awful" },
            new TagInfo { Name = "TEST3.Tag5", DataType = "float", Group = "TEST3", Quality = "bad" }
        };

        _map.Add(tags[0], Tag1Func);
        _map.Add(tags[1], Tag2Func);
        _map.Add(tags[2], Tag3Func);
        _map.Add(tags[3], Tag4Func);
        _map.Add(tags[4], Tag5Func);

        return tags;
    }

    public void Dispose()
    {
    }

    private double Tag1Func()
    {
        return 10 * Math.Sin(_x / 10);
    }

    private double Tag2Func()
    {
        return Math.Sin(_x / 10) > 0 ? 0 : 1;
    }

    private double Tag3Func()
    {
        var v = (_x / 10) % 10;

        if (v >= 1)
        {
            v = 5 - v;
        }

        return v;
    }

    private double Tag4Func()
    {
        var v = (_x / 10) % 10;

        return v;
    }

    private double Tag5Func()
    {
        var v = _random.Next(0, 100) / 10;

        if (v >= 5)
        {
            v = 5 - v;
        }

        return v;
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
    }

    private void UnsubscribeTag(TagInfo ti)
    {
        ti.Value = null;
        ti.TimeStamp = null;
        ti.Quality = null;
        ti.Tag = null;
    }

    private void NewTag_Changed(object sender, EventArgs e)
    {
        TagUpdated?.Invoke(null);
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        _x++;

        foreach (var kvp in _map)
        {
            if (kvp.Key.IsChecked)
            {
                kvp.Key.Value = kvp.Value.Invoke();
                kvp.Key.TimeStamp = DateTime.Now;
            }
        }
    }
}