using Microsoft.Win32;
using System;
using System.Xml;
using System.Xml.Serialization;

namespace WorkTimeLogger.Models
{

    public class WorkEvent
    {
        [XmlAttribute("type")]
        public SessionSwitchReason Type { get; set; }

        public DateTime Time { get; set; }
    }
}
