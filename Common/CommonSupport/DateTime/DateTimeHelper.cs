//********************************************************************************************
// This code based on public domain code by Sergey Stoyan.
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections;
using System.Web;

namespace CommonSupport
{
    /// <summary>
    /// Miscellaneous and parsing thread-safe methods for DateTime,
    /// class helps with parsing multiple date time formats.
    /// </summary>
    public static class DateTimeHelper
    {
        static object lock_variable = new object();

        #region miscellaneous methods

        public static uint GetSecondsSinceUnixEpoch(DateTime date_time)
        {
            lock (lock_variable)
            {
                TimeSpan t = date_time - new DateTime(1970, 1, 1);
                int ss = (int)t.TotalSeconds;
                if (ss < 0)
                    return 0;
                return (uint)ss;
            }
        }

        #endregion

        #region parsing definitions

        /// <summary>
        /// Defines a substring where date-time was found and result of conversion
        /// </summary>
        public class ParsedDateTime
        {
            readonly public int IndexOfDate = -1;
            readonly public int LengthOfDate = -1;
            readonly public int IndexOfTime = -1;
            readonly public int LengthOfTime = -1;
            readonly public DateTime DateTime;
            /// <summary>
            /// True if a date was found within string
            /// </summary>
            readonly public bool IsDateFound;
            /// <summary>
            /// True if a time was found within string
            /// </summary>
            readonly public bool IsTimeFound;

            internal ParsedDateTime(int index_of_date, int length_of_date, int index_of_time, int length_of_time, DateTime date_time)
            {
                IndexOfDate = index_of_date;
                LengthOfDate = length_of_date;
                IndexOfTime = index_of_time;
                LengthOfTime = length_of_time;
                DateTime = date_time;
                IsDateFound = index_of_date > -1;
                IsTimeFound = index_of_time > -1;
            }
        }

        /// <summary>
        /// Date that is accepted in the following cases:
        /// - no date was parsed by TryParse();
        /// - no year was found by TryParseDate();
        /// It is ignored when DefaultDateIsCurrent = true
        /// </summary>
        public static DateTime DefaultDate
        {
            set
            {
                _DefaultDate = value;
            }
            get
            {
                if (DefaultDateIsNow)
                    return DateTime.Now;
                else
                    return _DefaultDate;
            }
        }
        static DateTime _DefaultDate = DateTime.Now;

        /// <summary>
        /// If true then DefaultDate property is ignored and DefaultDate is always DateTime.Now
        /// </summary>
        public static bool DefaultDateIsNow = true;

        /// <summary>
        /// Defines default date-time format.
        /// </summary>
        public enum DateTimeFormat
        {
            /// <summary>
            /// month number goes before day number
            /// </summary>
            USA_DATE,
            /// <summary>
            /// day number goes before month number
            /// </summary>
            UK_DATE,
            ///// <summary>
            ///// time is specifed through AM or PM
            ///// </summary>
            //USA_TIME,
        }

        #endregion

        #region parsing derived methods for DateTime output

        /// <summary>
        /// Tries to find date and time within the passed string and return it as DateTime structure. 
        /// </summary>
        /// <param name="str">string that contains date and(or) time</param>
        /// <param name="default_format">format that must be used preferably in ambivalent instances</param>
        /// <param name="date_time">parsed date-time output</param>
        /// <returns>true if both date and time were found, else false</returns>
        static public bool TryParseDateTime(string str, DateTimeFormat default_format, out DateTime date_time)
        {
            lock (lock_variable)
            {
                ParsedDateTime parsed_date_time;
                if (!TryParseDateTime(str, default_format, out parsed_date_time))
                {
                    date_time = new DateTime(1, 1, 1);
                    return false;
                }
                date_time = parsed_date_time.DateTime;
                return true;
            }
        }

        /// <summary>
        /// Tries to find date and(or) time within the passed string and return it as DateTime structure. 
        /// If only date was found, time in the returned DateTime is always 0:0:0.
        /// If only time was found, date in the returned DateTime is DefaultDate.
        /// </summary>
        /// <param name="str">string that contains date and(or) time</param>
        /// <param name="default_format">format that must be used preferably in ambivalent instances</param>
        /// <param name="date_time">parsed date-time output</param>
        /// <returns>true if date and(or) time was found, else false</returns>
        static public bool TryParse(string str, DateTimeFormat default_format, out DateTime date_time)
        {
            lock (lock_variable)
            {
                ParsedDateTime parsed_date_time;
                if (!TryParse(str, default_format, out parsed_date_time))
                {
                    date_time = new DateTime(1, 1, 1);
                    return false;
                }
                date_time = parsed_date_time.DateTime;
                return true;
            }
        }

        /// <summary>
        /// Tries to find time within the passed string and return it as DateTime structure. 
        /// It recognizes only time while ignoring date, so date in the returned DateTime is always 1/1/1.
        /// </summary>
        /// <param name="str">string that contains time</param>
        /// <param name="default_format">format that must be used preferably in ambivalent instances</param>
        /// <param name="time">parsed time output</param>
        /// <returns>true if time was found, else false</returns>
        public static bool TryParseTime(string str, DateTimeFormat default_format, out DateTime time)
        {
            lock (lock_variable)
            {
                ParsedDateTime parsed_time;
                if (!TryParseTime(str, default_format, out parsed_time, null))
                {
                    time = new DateTime(1, 1, 1);
                    return false;
                }
                time = parsed_time.DateTime;
                return true;
            }
        }

        /// <summary>
        /// Tries to find date within the passed string and return it as DateTime structure. 
        /// It recognizes only date while ignoring time, so time in the returned DateTime is always 0:0:0.
        /// If year of the date was not found then it accepts the current year. 
        /// </summary>
        /// <param name="str">string that contains date</param>
        /// <param name="default_format">format that must be used preferably in ambivalent instances</param>
        /// <param name="date">parsed date output</param>
        /// <returns>true if date was found, else false</returns>
        static public bool TryParseDate(string str, DateTimeFormat default_format, out DateTime date)
        {
            lock (lock_variable)
            {
                ParsedDateTime parsed_date;
                if (!TryParseDate(str, default_format, out parsed_date))
                {
                    date = new DateTime(1, 1, 1);
                    return false;
                }
                date = parsed_date.DateTime;
                return true;
            }
        }

        #endregion

        #region parsing derived methods for ParsedDateTime output

        /// <summary>
        /// Tries to find date and time within the passed string and return it as ParsedDateTime object. 
        /// </summary>
        /// <param name="str">string that contains date-time</param>
        /// <param name="default_format">format that must be used preferably in ambivalent instances</param>
        /// <param name="parsed_date_time">parsed date-time output</param>
        /// <returns>true if both date and time were found, else false</returns>
        static public bool TryParseDateTime(string str, DateTimeFormat default_format, out ParsedDateTime parsed_date_time)
        {
            lock (lock_variable)
            {
                if (DateTimeHelper.TryParse(str, DateTimeHelper.DateTimeFormat.USA_DATE, out parsed_date_time)
                    && parsed_date_time.IsDateFound
                    && parsed_date_time.IsTimeFound
                    )
                    return true;

                parsed_date_time = null;
                return false;
            }
        }

        /// <summary>
        /// Tries to find time within the passed string and return it as ParsedDateTime object. 
        /// It recognizes only time while ignoring date, so date in the returned ParsedDateTime is always 1/1/1
        /// </summary>
        /// <param name="str">string that contains date-time</param>
        /// <param name="default_format">format that must be used preferably in ambivalent instances</param>
        /// <param name="parsed_date_time">parsed date-time output</param>
        /// <returns>true if time was found, else false</returns>
        static public bool TryParseTime(string str, DateTimeFormat default_format, out ParsedDateTime parsed_time)
        {
            lock (lock_variable)
            {
                return TryParseTime(str, default_format, out parsed_time, null);
            }
        }

        /// <summary>
        /// Tries to find date and(or) time within the passed string and return it as ParsedDateTime object. 
        /// If only date was found, time in the returned ParsedDateTime is always 0:0:0.
        /// If only time was found, date in the returned ParsedDateTime is DefaultDate.
        /// </summary>
        /// <param name="str">string that contains date-time</param>
        /// <param name="default_format">format that must be used preferably in ambivalent instances</param>
        /// <param name="parsed_date_time">parsed date-time output</param>
        /// <returns>true if date or time was found, else false</returns>
        static public bool TryParse(string str, DateTimeFormat default_format, out ParsedDateTime parsed_date_time)
        {
            lock (lock_variable)
            {
                parsed_date_time = null;

                ParsedDateTime parsed_date;
                ParsedDateTime parsed_time;
                if (!TryParseDate(str, default_format, out parsed_date))
                {
                    if (!TryParseTime(str, default_format, out parsed_time, null))
                        return false;

                    DateTime date_time = new DateTime(DefaultDate.Year, DefaultDate.Month, DefaultDate.Day, parsed_time.DateTime.Hour, parsed_time.DateTime.Minute, parsed_time.DateTime.Second);
                    parsed_date_time = new ParsedDateTime(-1, -1, parsed_time.IndexOfTime, parsed_time.LengthOfTime, date_time);
                }
                else
                {
                    if (!TryParseTime(str, default_format, out parsed_time, parsed_date))
                    {
                        DateTime date_time = new DateTime(parsed_date.DateTime.Year, parsed_date.DateTime.Month, parsed_date.DateTime.Day, 0, 0, 0);
                        parsed_date_time = new ParsedDateTime(parsed_date.IndexOfDate, parsed_date.LengthOfDate, -1, -1, date_time);
                    }
                    else
                    {
                        DateTime date_time = new DateTime(parsed_date.DateTime.Year, parsed_date.DateTime.Month, parsed_date.DateTime.Day, parsed_time.DateTime.Hour, parsed_time.DateTime.Minute, parsed_time.DateTime.Second);
                        parsed_date_time = new ParsedDateTime(parsed_date.IndexOfDate, parsed_date.LengthOfDate, parsed_time.IndexOfTime, parsed_time.LengthOfTime, date_time);
                    }
                }

                return true;
            }
        }

        #endregion

        #region parsing base methods

        /// <summary>
        /// Tries to find time within the passed string (relatively to the passed parsed_date if any) and return it as ParsedDateTime object.
        /// It recognizes only time while ignoring date, so date in the returned ParsedDateTime is always 1/1/1
        /// </summary>
        /// <param name="str">string that contains date</param>
        /// <param name="default_format">format that must be used preferably in ambivalent instances</param>
        /// <param name="parsed_time">parsed date-time output</param>
        /// <param name="parsed_date">ParsedDateTime object if the date was found within this string, else NULL</param>
        /// <returns>true if time was found, else false</returns>
        public static bool TryParseTime(string str, DateTimeFormat default_format, out ParsedDateTime parsed_time, ParsedDateTime parsed_date)
        {
            lock (lock_variable)
            {
                parsed_time = null;

                Match m;
                if (parsed_date != null && parsed_date.IndexOfDate > -1)
                {//look around the found date
                    //look for <date> [h]h:mm[:ss] [PM/AM]
                    m = Regex.Match(str.Substring(parsed_date.IndexOfDate + parsed_date.LengthOfDate), @"(?<=^\s*,?\s+|^\s*at\s*|^\s*[T\-]\s*)(?'hour'\d{1,2})\s*:\s*(?'minute'\d{2})\s*(?::\s*(?'second'\d{2}))?(?:\s*([AP]M))?(?=$|[^\d\w])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    if (!m.Success)
                        //look for [h]h:mm:ss <date>
                        m = Regex.Match(str.Substring(0, parsed_date.IndexOfDate), @"(?<=^|[^\d])(?'hour'\d{1,2})\s*:\s*(?'minute'\d{2})\s*(?::\s*(?'second'\d{2}))?(?:\s*([AP]M))?(?=$|[\s,]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                }
                else//look anywere within string
                    //look for [h]h:mm[:ss] [PM/AM]
                    m = Regex.Match(str, @"(?<=^|\s+|\s*T\s*)(?'hour'\d{1,2})\s*:\s*(?'minute'\d{2})\s*(?::\s*(?'second'\d{2}))?(?:\s*([AP]M))?(?=$|[^\d\w])", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                if (m.Success)
                {
                    try
                    {
                        int hour = int.Parse(m.Groups["hour"].Value);
                        if (hour < 0 || hour > 23)
                            return false;

                        int minute = int.Parse(m.Groups["minute"].Value);
                        if (minute < 0 || minute > 59)
                            return false;

                        int second = 0;
                        if (!string.IsNullOrEmpty(m.Groups["second"].Value))
                        {
                            second = int.Parse(m.Groups["second"].Value);
                            if (second < 0 || second > 59)
                                return false;
                        }

                        if (string.Compare(m.Groups[4].Value, "PM", true) > -1)
                            hour += 12;

                        DateTime date_time = new DateTime(1, 1, 1, hour, minute, second);
                        parsed_time = new ParsedDateTime(-1, -1, m.Index, m.Length, date_time);
                    }
                    catch
                    {
                        return false;
                    }
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Tries to find date within the passed string and return it as ParsedDateTime object. 
        /// It recognizes only date while ignoring time, so time in the returned ParsedDateTime is always 0:0:0.
        /// If year of the date was not found then it accepts the current year. 
        /// </summary>
        /// <param name="str">string that contains date</param>
        /// <param name="default_format">format that must be used preferably in ambivalent instances</param>
        /// <param name="parsed_date">parsed date output</param>
        /// <returns>true if date was found, else false</returns>
        static public bool TryParseDate(string str, DateTimeFormat default_format, out ParsedDateTime parsed_date)
        {
            lock (lock_variable)
            {
                parsed_date = null;

                if (string.IsNullOrEmpty(str))
                    return false;

                //look for dd/mm/yy
                Match m = Regex.Match(str, @"(?<=^|[^\d])(?'day'\d{1,2})\s*(?'separator'[\\/\.])+\s*(?'month'\d{1,2})\s*\'separator'+\s*(?'year'\d{2,4})(?=$|[^\d])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                if (m.Success && m.Groups["year"].Value.Length != 3)
                {
                    DateTime date;
                    if ((default_format ^ DateTimeFormat.USA_DATE) == DateTimeFormat.USA_DATE)
                    {
                        if (!convert_to_date(int.Parse(m.Groups["year"].Value), int.Parse(m.Groups["day"].Value), int.Parse(m.Groups["month"].Value), out date))
                            return false;
                    }
                    else
                    {
                        if (!convert_to_date(int.Parse(m.Groups["year"].Value), int.Parse(m.Groups["month"].Value), int.Parse(m.Groups["day"].Value), out date))
                            return false;
                    }
                    parsed_date = new ParsedDateTime(m.Index, m.Length, -1, -1, date);
                    return true;
                }

                //look for yy-mm-dd
                m = Regex.Match(str, @"(?<=^|[^\d])(?'year'\d{2,4})\s*(?'separator'[\-])\s*(?'month'\d{1,2})\s*\'separator'+\s*(?'day'\d{1,2})(?=$|[^\d])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                if (m.Success && m.Groups["year"].Value.Length != 3)
                {
                    DateTime date;
                    if (!convert_to_date(int.Parse(m.Groups["year"].Value), int.Parse(m.Groups["month"].Value), int.Parse(m.Groups["day"].Value), out date))
                        return false;
                    parsed_date = new ParsedDateTime(m.Index, m.Length, -1, -1, date);
                    return true;
                }

                //look for month dd yyyy
                m = Regex.Match(str, @"(?:^|[^\d\w])(?'month'Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)[uarychilestmbro]*\s+(?'day'\d{1,2})(?:-?st|-?th)?\s*,?\s*(?'year'\d{4})(?=$|[^\d\w])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                if (!m.Success)
                    //look for dd month [yy]yy
                    m = Regex.Match(str, @"(?:^|[^\d\w:])(?'day'\d{1,2})(?:-?st|-?th)?\s+(?'month'Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)[uarychilestmbro]*(?:\s*,?\s*(?:'?(?'year'\d{2})|(?'year'\d{4})))?(?=$|[^\d\w])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                if (!m.Success)
                    //look for yyyy month dd
                    m = Regex.Match(str, @"(?:^|[^\d\w])(?'year'\d{4})\s+(?'month'Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)[uarychilestmbro]*\s+(?'day'\d{1,2})(?:-?st|-?th)?(?=$|[^\d\w])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                if (!m.Success)
                    //look for  month dd [yyyy]
                    m = Regex.Match(str, @"(?:^|[^\d\w])(?'month'Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)[uarychilestmbro]*\s+(?'day'\d{1,2})(?:-?st|-?th)?(?:\s*,?\s*(?'year'\d{4}))?(?=$|[^\d\w])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    int month = -1;
                    int index_of_date = m.Index;
                    int length_of_date = m.Length;

                    switch (m.Groups["month"].Value)
                    {
                        case "Jan":
                            month = 1;
                            break;
                        case "Feb":
                            month = 2;
                            break;
                        case "Mar":
                            month = 3;
                            break;
                        case "Apr":
                            month = 4;
                            break;
                        case "May":
                            month = 5;
                            break;
                        case "Jun":
                            month = 6;
                            break;
                        case "Jul":
                            month = 7;
                            break;
                        case "Aug":
                            month = 8;
                            break;
                        case "Sep":
                            month = 9;
                            break;
                        case "Oct":
                            month = 10;
                            break;
                        case "Nov":
                            month = 11;
                            break;
                        case "Dec":
                            month = 12;
                            break;
                    }

                    int year;
                    if (!string.IsNullOrEmpty(m.Groups["year"].Value))
                        year = int.Parse(m.Groups["year"].Value);
                    else
                        year = DefaultDate.Year;

                    DateTime date;
                    if (!convert_to_date(year, month, int.Parse(m.Groups["day"].Value), out date))
                        return false;
                    parsed_date = new ParsedDateTime(index_of_date, length_of_date, -1, -1, date);
                    return true;
                }

                return false;
            }
        }

        static bool convert_to_date(int year, int month, int day, out DateTime date)
        {
            if (year >= 100)
            {
                if (year < 1000)
                {
                    date = new DateTime(1, 1, 1);
                    return false;
                }
            }
            else
                if (year > 30)
                    year += 1900;
                else
                    year += 2000;

            try
            {
                date = new DateTime(year, month, day);
            }
            catch
            {
                date = new DateTime(1, 1, 1);
                return false;
            }
            return true;
        }

        #endregion
    }
}