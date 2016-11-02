using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using WorkTimeLogger.Extensions;
using WorkTimeLogger.Interfaces;
using WorkTimeLogger.Models;

namespace WorkTimeLogger
{
    /// <summary>
    /// Serialises events to XML and stores on disk. All time stored in local time.
    /// </summary>
    public class XMLFileDataProvider : IDataProvider
    {
        private readonly string _datapath;

        private ILogger _logger = LogManager.GetCurrentClassLogger();

        public XMLFileDataProvider()
        {
            _datapath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WorkTimeLogger");

            EnsureDirectoryExists(_datapath);
        }

        private string SerialiseEventData<T>(T data)
        {
            string serialised = string.Empty;
            var serializer = new XmlSerializer(data.GetType());

            using (StringWriter tw = new StringWriter())
            {
                serializer.Serialize(tw, data);

                serialised = tw.ToString();
            }

            return serialised;
        }

        private WorkDay DeserialiseEventData(string xml)
        {
            WorkDay data;
            using (StringReader sr = new StringReader(xml))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(WorkDay));
                data = (WorkDay)serializer.Deserialize(sr);
            }

            return data;
        }

        private void EnsureDirectoryExists(string path)
        {
            if (Directory.Exists(path))
            {
                return;
            }

            _logger.Debug($"Creating directory {path}");
            Directory.CreateDirectory(path);
        }

        private string BuildWeekDirectoryPath(DateTime time)
        {
            //create these paths when the event happens, because we might have rolled to the next day
            string yearDirectoryPath = Path.Combine(_datapath, DateTime.Now.Year.ToString());
            EnsureDirectoryExists(yearDirectoryPath);

            string monthDirectoryPath = Path.Combine(yearDirectoryPath, time.Month.ToString());
            EnsureDirectoryExists(monthDirectoryPath);

            string weekDirectoryPath = Path.Combine(monthDirectoryPath, $"week-{time.WeekNumber().ToString()}");
            EnsureDirectoryExists(weekDirectoryPath);

            return weekDirectoryPath;
        }

        public void UpdateEvents(DateTime time, WorkEvent newevent)
        {
            string weekDirectoryPath = BuildWeekDirectoryPath(time);
            string todaysDataPath = Path.Combine(
                                                    weekDirectoryPath,
                                                    $"{string.Format("{0:dd-MM}", DateTime.Now)}.xml"
                                                );

            var workday = File.Exists(todaysDataPath) ? DeserialiseEventData(File.ReadAllText(todaysDataPath))
                                                                                     : new WorkDay { Date = DateTime.Now };

            workday.Events.Add(newevent);

            File.WriteAllText(todaysDataPath, SerialiseEventData(workday));
        }

        /// <summary>
        /// Returns workday models for whole week of specified datetime
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public IEnumerable<WorkDay> GetWeekEvents(DateTime time)
        {
            List<WorkDay> workdays = new List<WorkDay>();
            string weekDirectoryPath = BuildWeekDirectoryPath(time);

            foreach (string workdayPath in Directory.GetFiles(weekDirectoryPath, "*.xml"))
            {
                string workdayXML = File.ReadAllText(workdayPath);
                WorkDay workday = DeserialiseEventData(workdayXML);

                //don't care about weekends
                if (workday.Date.DayOfWeek == DayOfWeek.Saturday || workday.Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    continue;
                }

                workdays.Add(workday);
            }

            workdays = workdays.OrderBy(x => x.Date).ToList();

            int workingDaysCount = 5;

            if (workdays.Count == workingDaysCount)
            {
                _logger.Debug($"{workingDaysCount} days of data found");
                return workdays;
            }
            else
            {
                List<WorkDay> workdaysTemp = new List<WorkDay>();
                DateTime dateToStamp = time.FirstDateOfWeek();

                for (int x = 0; x < workingDaysCount; ++x)
                {
                    var workDay = workdays.SingleOrDefault(d => d.Date.Day == dateToStamp.Day);

                    //data missing for day so add 
                    if (workDay == null)
                    {
                        _logger.Debug($"Data missing for {dateToStamp.ToString()}");
                        workdaysTemp.Add(new WorkDay
                        {
                            Date = dateToStamp
                        });
                    }
                    else
                    {
                        workdaysTemp.Add(workDay);
                    }

                    dateToStamp = dateToStamp.AddDays(1);
                }

                return workdaysTemp;
            }
        }

    }


}

