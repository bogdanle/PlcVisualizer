using System;
using System.Collections.Generic;
using PlcVisualizer.Models;

namespace PlcVisualizer.Interfaces;

public interface IPlcService
{
    /// <summary>
    /// Event fired when Tag gets updated.
    /// </summary>
    event Action<TagInfo> TagUpdated;

    string IpAddress { get; set; }

    string CipPath { get; set; }

    int Timeout { get; set; }

    int UpdateRate { get; set; }

    /// <summary>
    /// Connect to the PLC and upload the tags.
    /// </summary>
    void Connect();

    /// <summary>
    /// Disconnect the service and dispose objects.
    /// </summary>
    void Disconnect();

    /// <summary>
    /// Get the list of tags we want to display in the grid.
    /// </summary>
    /// <returns>Collections of tags.</returns>
    IEnumerable<TagInfo> GetTags();

    void StartScan();
}