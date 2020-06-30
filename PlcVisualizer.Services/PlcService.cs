using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Logix;
using PlcVisualizer.Interfaces;
using PlcVisualizer.Models;

namespace PlcVisualizer.Services
{
    public sealed class PlcService : IPlcService, IDisposable
    {
        private readonly Controller _plc = new Controller();
        private readonly TagGroup _tagGroup = new TagGroup();
        private ReadOnlyCollection<Program> _programList;
        private string _filterName = "*";
        private string _filterType = "*";
        private string _ipAddress = "192.168.10.1";
        private string _cipPath = "0";
        private int _timeout = 3000;
        private int _updateRate = 1000;

        public event Action<TagInfo> TagUpdated;
        
        public string IpAddress
        {
            get => _ipAddress;
            set
            {
                _ipAddress = value;
                _plc.IPAddress = value;
            }
        }

        public string CipPath
        {
            get => _cipPath;
            set
            {
                _cipPath = value;
                _plc.Path = value;
            }
        }

        public int Timeout
        {
            get => _timeout;
            set
            {
                _timeout = value;
                _plc.Timeout = value;
            }
        }

        public int UpdateRate
        {
            get => _updateRate;
            set
            {
                _updateRate = value;
                _tagGroup.Interval = value;
            }
        }

        /// <summary>
        /// Connect to the PLC and upload the tags.
        /// </summary>
        public void Connect()
        {
            _plc.IPAddress = IpAddress;
            _plc.Path = CipPath;
            _plc.Timeout = Timeout;

            var result = _plc.Connect();
            if (result != ResultCode.E_SUCCESS)
            { 
                var msg = $"Error: {_plc.ErrorCode} - {_plc.ErrorString}";
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// Disconnect the service and dispose objects.
        /// </summary>
        public void Disconnect()
        {
            _tagGroup.ScanStop();

            _plc.Disconnect();

            _plc.Dispose();
        }

        public void StartScan()
        {
            // Assign parent Controller class and start scanning
            _tagGroup.Controller = _plc;
            _tagGroup.Interval = UpdateRate;
            _tagGroup.ScanStart();
        }

        /// <summary>
        /// Get the list of tags we want to display in the grid.
        /// </summary>
        /// <returns>Collection of tags.</returns>
        public IEnumerable<TagInfo> GetTags()
        {
            if (_plc.UploadTags() != ResultCode.E_SUCCESS)
            {
                throw new Exception(_plc.ErrorString);
            }

            _programList = _plc.ProgramList;

            var tags = new List<TagInfo>();

            // Get only SimulationTests and Master_Control tags
            foreach (var program in _programList.Where(p => p.Name == "SimulationTests" || p.Name == "Master_Control"))
            {
                var tagList = program.TagItems(_filterName, _filterType);

                foreach (var tagTemplate in tagList)
                {
                    var ti = new TagInfo() { Group = program.Name, Name = tagTemplate.Name, TagTemplate = tagTemplate };
                    tags.Add(ti);

                    ti.PropertyChanged += Ti_PropertyChanged;
                }
            }

            return tags;
        }

        public void Dispose()
        {
            _plc.Dispose();
            _tagGroup.Dispose();
        }

        /// <summary>
        /// We need to catch the item checked/unchecked events here in order to subscribe/unsubscribe tag changed notifications.
        /// </summary>
        /// <param name="sender">TagInfo object.</param>
        /// <param name="e">PropertyChangedEventArgs.</param>
        private void Ti_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                var ti = (TagInfo)sender;
                if (ti.IsChecked == true)
                {
                    SubscribeTag(ti);
                }
                else
                {
                    UnsubscribeTag(ti);
                }
            }
        }

        /// <summary>
        /// Read the tag and subscribe to tag changed event.
        /// </summary>
        /// <param name="ti">TagInfo object.</param>
        private void SubscribeTag(TagInfo ti)
        {
            if (ti.TagTemplate != null)
            {
                if (ti.TagTemplate.IsArray || (!ti.TagTemplate.IsAtomic && !ti.TagTemplate.IsString))
                {
                    return;
                }

                Tag newTag = ti.TagTemplate.ToTag();

                // If the quality is good subscribe to the tag changed event
                if (_plc.ReadTag(newTag) == ResultCode.E_SUCCESS)
                {
                    newTag.Changed += NewTag_Changed;

                    newTag.MyObject = ti;

                    ti.Tag = newTag;
                    ti.Value = newTag.Value;
                    ti.Quality = newTag.QualityString;
                    ti.TimeStamp = newTag.TimeStamp;

                    _tagGroup.AddTag(newTag);
                }
                else
                {
                    throw new Exception(newTag.ErrorString);
                }
            }

            // In this example we don't want array or structures
        }

        /// <summary>
        /// Unsubscribe from tag changed event and clear the fields.
        /// </summary>
        /// <param name="ti">TagInfo object.</param>
        private void UnsubscribeTag(TagInfo ti)
        {
            ti.Tag.Changed -= NewTag_Changed;
            ti.Tag.MyObject = null;
            ti.Value = null;
            ti.TimeStamp = null;
            ti.Quality = null;
            ti.Tag = null;
        }

        /// <summary>
        /// Update the tag value and timestamp.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void NewTag_Changed(object sender, EventArgs e)
        {
            var args = (DataChangeEventArgs)e;

            var ti = (TagInfo)args.MyObject;

            if (args.QualityCode == ResultCode.QUAL_GOOD)
            {
                ti.Value = args.Value;
                ti.TimeStamp = args.TimeStamp;
            }
            else
            {
                ti.Value = "???";
            }

            ti.Quality = args.QualityString;

            TagUpdated?.Invoke(ti);
        }
    }
}
