using System;
using System.Collections.Generic;
using System.Text;
using CommonFinancial;
using System.IO;
using CommonSupport;
using System.Collections.ObjectModel;

namespace CommonFinancial
{
    /// <summary>
    ///  Class designed to read MT4 HST historical dataDelivery files.
    ///  
    /// MT4 HST File Format
    ///  
    /// Header:
    /// Int - Version (current is 400)
    /// String - Copyright (64 symbols)
    /// String - BaseCurrency (12 symbols)
    /// Int - Period
    /// Int - Digits
    /// Int - Timesign
    /// Int - Last sync
    /// BYTE - 13 reserved bytes
    /// [some other empty bytes?]
    /// 
    /// Data entry format:
    /// Int - Time
    /// Double - Open
    /// Double - Low
    /// Double - High
    /// Double - Close
    /// Double - Volume
    /// </summary>
    
    public class HSTDataBarReaderWriter : DataReaderWriter<DataBar>
    {

        /// <summary>
        /// 
        /// </summary>
        public HSTDataBarReaderWriter(string filePath)
            : base(filePath)
        {
        }

        /// <summary>
        /// Helper, read HST file header.
        /// </summary>
        /// <param name="reader"></param>
        bool ReadHeader(BinaryReader reader)
        {
            long version = reader.ReadInt32();
            SystemMonitor.CheckWarning(version == 400, "HST file version may be different/incompatible.");

            Copyright = new string(reader.ReadChars(64)).TrimEnd('\0');
            Symbol = new string(reader.ReadChars(12)).TrimEnd('\0');
            Period = TimeSpan.FromMinutes(reader.ReadInt32());
            DecimalDigits = reader.ReadInt32();
            
            int timesign = reader.ReadInt32();
            int lastSync = reader.ReadInt32();
            
            // Read some reserved bytes.
            byte[] reserved = reader.ReadBytes(13);

            // There are some more empty bytes, skip trough them.
            // There seems to be a limit of 39 for these, bringing the total
            // reserved to 52.
            int chars = 0;
            while (reader.PeekChar() == 0 && chars < 39)
            {
                chars++;
                reader.ReadByte();
            }

            return true;
        }

        /// <summary>
        /// Write HST file header.
        /// </summary>
        bool WriteHeader(BinaryWriter writer)
        {
            if (Period.HasValue == false)
            {
                return false;
            }

            long version = 400;
            writer.Write(version);
            
            string copyright = "OpenForexPlatform";
            char[] copyrightChars = new char[64];
            copyright.CopyTo(0, copyrightChars, 0, copyright.Length);
            writer.Write(copyrightChars);

            char[] symbolChars = new char[12];
            Symbol.CopyTo(0, symbolChars, 0, Symbol.Length);
            writer.Write(Symbol);

            writer.Write((int)Period.Value.TotalMinutes);
            writer.Write((int)DecimalDigits);
            
            // Timesign.
            writer.Write((int)0);
            
            // Lastsync.
            writer.Write((int)0);

            // Reserved.
            writer.Write(new byte[13]);

            return true;
        }

        //public override int GetRowCount()
        //{
        //    int rowsRead = 0;
        //    if (DoRead(0, int.MaxValue, 
        //        out rowsRead, null))
        //    {
        //        return rowsRead;
        //    }

        //    return 0;
        //}

        /// <summary>
        /// 
        /// </summary>
        static bool WriteBarData(BinaryWriter writer, DataBar data)
        {
            int timeInt = (int)GeneralHelper.GenerateSecondsDateTimeFrom1970(data.DateTime);
            writer.Write(timeInt);
            writer.Write((double)data.Open);
            writer.Write((double)data.Low);
            writer.Write((double)data.High);
            writer.Write((double)data.Close);
            writer.Write((double)data.Volume);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        static DataBar ReadBarData(BinaryReader reader, int decimalDigits)
        {
            int timeInt = reader.ReadInt32();
            DateTime? time = GeneralHelper.GenerateDateTimeSecondsFrom1970(timeInt);
            
            decimal open = Math.Round((decimal)reader.ReadDouble(), decimalDigits);
            decimal low = Math.Round((decimal)reader.ReadDouble(), decimalDigits);
            decimal high = Math.Round((decimal)reader.ReadDouble(), decimalDigits);
            decimal close = Math.Round((decimal)reader.ReadDouble(), decimalDigits);
            decimal volume = Math.Round((decimal)reader.ReadDouble(), decimalDigits);
            return new DataBar(time.Value, open, high, low, close, volume);
        }

        /// <summary>
        /// Use this to read rows in "pages". Subscribe to the event DataPageReadEvent to receive pages read.
        /// </summary>
        /// <param name="?"></param>
        /// <returns>Count of rows read.</returns>
        public override bool ReadPaged(int startingRow, int rowCount, int rowsPerPage, out int totalRowsRead)
        {
            totalRowsRead = 0;
            using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs, Encoding.ASCII))
                {
                    try
                    {
                        if (ReadHeader(br) == false)
                        {
                            return false;
                        }

                        List<DataBar> pageResult = new List<DataBar>();
                        while (fs.Position < fs.Length - 1)
                        {
                            totalRowsRead++;

                            DataBar rowData = ReadBarData(br, base.DecimalDigits);

                            if (rowCount > 0 && rowCount <= Math.Max(0, totalRowsRead - startingRow))
                            {
                                break;
                            }

                            if (totalRowsRead >= startingRow)
                            {
                                pageResult.Add(rowData);
                            }

                            if (pageResult.Count >= rowsPerPage)
                            {// Send the next page.
                                RaiseDataPageReadEvent(pageResult);
                                pageResult.Clear();
                            }
                        }

                        if (pageResult.Count > 0)
                        {// Run the last page (incomplete page dataDelivery).
                            if (RaiseDataPageReadEvent(pageResult) == false)
                            {// Read stopped by consumer.
                                return true;
                            }
                            pageResult.Clear();
                        }
                    }
                    catch (Exception ex)
                    {
                        SystemMonitor.OperationError(string.Format("Failed to read HST file [{0}][{1}]", FilePath, ex.Message));
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool Read(int startingRow, int rowCount, out List<DataBar> data)
        {
            int rowsCount = 0;
            data = new List<DataBar>();
            return DoRead(startingRow, rowCount, out rowsCount, data);
        }
        
        /// <summary>
        /// Use this to read all the dataDelivery in memory and directly provide it (not very good for very big files).
        /// </summary>
        /// <param name="startRow">Row to start reading at.</param>
        /// <param name="rowCount">Count of rows to read (pass 0 or less for all).</param>
        /// <param name="dataDelivery">Data.</param>
        public bool DoRead(int startingRow, int rowCount, out int rowsRead, List<DataBar> data)
        {
            rowsRead = 0;

            using (FileStream fs = new FileStream(FilePath, FileMode.Open))
            {
                using(BinaryReader br = new BinaryReader(fs, Encoding.ASCII))
                {
                    try
                    {
                        if (ReadHeader(br) == false)
                        {
                            return false;
                        }

                        while (fs.Position < fs.Length - 1)
                        {
                            rowsRead++;
                            DataBar rowData = ReadBarData(br, DecimalDigits);

                            if (rowCount > 0 && rowCount <= Math.Max(0, rowsRead - startingRow))
                            {
                                rowsRead--;
                                break;
                            }

                            if (rowsRead >= startingRow && data != null)
                            {
                                data.Add(rowData);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        SystemMonitor.OperationError(string.Format("Failed to read HST file [{0}][{1}]", FilePath, ex.Message));
                        return false;
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public override bool Write(int startingRow, int rowCount, ReadOnlyCollection<DataBar> bars)
        {
            FileStream fs;
            if (startingRow == 0)
            {
                fs = new FileStream(FilePath, FileMode.CreateNew);
            }
            else
            {
                if (File.Exists(FilePath) == false)
                {
                    SystemMonitor.OperationError("Failed to find to write to.");
                    return false;
                }

                fs = new FileStream(FilePath, FileMode.Open, FileAccess.ReadWrite);
            }

            using (fs)
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    if (startingRow == 0)
                    {
                        if (WriteHeader(bw) == false)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        using (BinaryReader reader = new BinaryReader(fs))
                        {
                            try
                            {
                                for (int i = 0; i < startingRow; i++)
                                {
                                    ReadBarData(reader, DecimalDigits);
                                }
                            }
                            catch (Exception ex)
                            {
                                SystemMonitor.OperationError("Error in moving to required starting row [" + ex.Message + "].");
                                return false;
                            }
                        }
                    }

                    int count = 0;
                    foreach (DataBar bar in bars)
                    {
                        if (count > rowCount)
                        {
                            break;
                        }
                        WriteBarData(bw, bar);
                        count++;
                    }
                }
            }


            return true;
        }

    }
}
