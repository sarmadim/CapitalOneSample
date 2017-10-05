using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalOneApp
{
    public class SecurityTableData
    {
        public DateTime Date { get; set; }
        public double ClosePrice { get; set; }
        public double OpenPrice { get; set; }
        public double LowPrice { get; set; }
        public double HighPrice { get; set; }
        public double Volume { get; set; }
        public double HighLowDiffPrice => HighPrice - LowPrice;
    }
    public class QuandlSecurityInfo
    {
        public string SecurtiyName { get; set; }
        public int PriceDiffCount { get; set; }
        public SecurityTableData MaxPriceDate { get; set; }

        private Dictionary<int, double> _averageOpenPrice;
        public IDictionary<int, double> AverageOpenPrice => _averageOpenPrice ?? (_averageOpenPrice = new Dictionary<int, double>());

        private Dictionary<int, double> _averageClosePrice;
        public IDictionary<int, double> AverageClosePrice => _averageClosePrice ?? (_averageClosePrice = new Dictionary<int, double>());

        private Dictionary<int, double> _averageVolume;
        public IDictionary<int, double> AverageVolume => _averageVolume ?? (_averageVolume = new Dictionary<int, double>());


        private Dictionary<int, List<SecurityTableData>> _tableData;
        public IDictionary<int, List<SecurityTableData>> TableData => _tableData ?? (_tableData = new Dictionary<int, List<SecurityTableData>>());

        public void ProcessCsvInfo(string info)
        {
            string[] token = info.Split(',');
            var date = DateTime.Parse(token[1]);
            if (date.Year == 2017 && date.Month >= 1 && date.Month <= 7)
            {
                var tableData = new SecurityTableData
                {
                    Date = date,
                    OpenPrice = Convert.ToDouble(token[2]),
                    ClosePrice = Convert.ToDouble(token[3]),
                    LowPrice = Convert.ToDouble(token[4]),
                    HighPrice = Convert.ToDouble(token[5]),
                    Volume = Convert.ToDouble(token[6])
                };

                if (tableData.OpenPrice > tableData.ClosePrice)
                    PriceDiffCount += 1;

                if (TableData.ContainsKey(date.Month))
                {
                    TableData[date.Month].Add(tableData);
                    AverageOpenPrice[date.Month] += tableData.OpenPrice;
                    AverageClosePrice[date.Month] += tableData.ClosePrice;
                    AverageVolume[date.Month] += tableData.Volume;
                }
                else
                {
                    TableData.Add(date.Month, new List<SecurityTableData> { tableData });
                    AverageOpenPrice.Add(date.Month, tableData.OpenPrice);
                    AverageClosePrice.Add(date.Month, tableData.ClosePrice);
                    AverageVolume.Add(date.Month, tableData.Volume);
                }

                if (MaxPriceDate == null)
                    MaxPriceDate = tableData;
                else if (tableData.HighLowDiffPrice > MaxPriceDate.HighLowDiffPrice)
                    MaxPriceDate = tableData;
            }
        }
        public double AverageOpenForMonth(int month)
        {
            return _averageOpenPrice[month] / TableData[month].Count;
        }

        public double AverageCloseForMonth(int month)
        {
            return _averageClosePrice[month] / TableData[month].Count;
        }

        public double AverageVolumeForMonth(int month)
        {
            return _averageVolume[month] / TableData[month].Count;
        }

        public IList<SecurityTableData> CalculateBusyDays()
        {
            var tableData = new List<SecurityTableData>();
            foreach (var key in TableData.Keys)
            {
                var avgVolume = AverageVolumeForMonth(key);
                foreach (var securityTableData in TableData[key])
                {
                    var newVolume = (avgVolume * 1.1);
                    if(newVolume < securityTableData.Volume)
                        tableData.Add(securityTableData);
                }
            }
            return tableData;
        }
        public void PrintSecurityInfo()
        {
            System.Console.WriteLine("**************Average Open&Close for Security**************");
            foreach (var key in TableData.Keys)
            {
                var data = TableData[key];
                System.Console.WriteLine(SecurtiyName + "-> [" + data.First().Date.Year + "-" + data.First().Date.Month + "] [Average Open : " + 
                    AverageOpenForMonth(key).ToString("N2") + "] [Average Close : " + AverageCloseForMonth(key).ToString("N2") + "]");
                
            }
        }

        public void PrintMaxDailyProfit()
        {
            System.Console.WriteLine("**************Max Daily Profit**************");
            System.Console.WriteLine(SecurtiyName + "-> [" + MaxPriceDate.Date.ToShortDateString() + "] [Max Profit : " + MaxPriceDate.HighLowDiffPrice.ToString("N2") + "]");
        }

        public void PrintBusyDays()
        {
            var data = CalculateBusyDays();
            System.Console.WriteLine("**************Printing Busy Days**************");
            foreach (var tableData in data)
            {
                System.Console.WriteLine(SecurtiyName + "-> [" + tableData.Date.ToShortDateString() + "] [Volume : " + tableData.Volume.ToString("N2") + "]"
                                         + " [Avg Volume for the month: " + AverageVolumeForMonth(tableData.Date.Month).ToString("N2") + "]");
            }

        }

        public void PrintBiggestLoser()
        {
            System.Console.WriteLine("**************Biggest Loser**************");
            System.Console.WriteLine(SecurtiyName + "->" + " [No Of Days : " + PriceDiffCount + "]");
        }
    }
}
