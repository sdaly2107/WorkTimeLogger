﻿namespace WorkTimeLogger
{
    public struct HourRange
    {
        public int from;
        public int to;

        public HourRange(int from, int to)
        {
            this.from = from;
            this.to = to;
        }
    }

    public class Settings
    {
        public double Lunch { get; set; }

        public double HoursBeforeLunchDeducted { get; set; }

        public double NoShowHours { get; set; }

        public double MinDailyHours { get; set; }

        public double MinWeekHours { get; set; }

        public HourRange BandwidthHours { get; set; }

        public HourRange CoreHours { get; set; }


        public Settings()
        {
            //defaults
            HoursBeforeLunchDeducted = 5;
            Lunch = 0.5;
            NoShowHours = 7.4; //default hours for no start/end time detected, ie holiday/sick
            MinDailyHours = 5; //must work 5 hours in core hours
            MinWeekHours = 37; //this could be less over the month
            BandwidthHours = new HourRange(7, 19); //hours counter from 7am to 7pm
            CoreHours = new HourRange(8, 13); //core hours from 8am to 1pm
        }
    }
}
