using System.Collections.Generic;
using System.Xml.Serialization;
using System;

namespace WorkTimeLogger.Models
{
    [XmlRoot]
    public class WorkDay
    {
        public DateTime Date { get; set; }

        public List<WorkEvent> Events { get; set; }

        public WorkDay()
        {
            Events = new List<WorkEvent>();
        }

    }
}
