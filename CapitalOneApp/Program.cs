using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CapitalOneApp
{
    class Program
    {
        public static string ApiKey = "s-GMZ_xkw6CrkGYUWs1p";

        public static string Columns = "ticker,date,open,close,low,high,volume";
        static void Main(string[] args)
        {
           var securitiesList = new [] {"GOOGL", "COF", "MSFT"};
            QuandlSecurityInfo biggestLoser = null;
            foreach (var security in securitiesList)
            {
                var securityInfo = ReadStreamForSecurityData(security);
                securityInfo.PrintSecurityInfo();
                if (args.Contains("--max-daily-profit"))
                    securityInfo.PrintMaxDailyProfit();
                if (args.Contains("--busy-day"))
                    securityInfo.PrintBusyDays();

                if (biggestLoser == null)
                    biggestLoser = securityInfo;
                else if (securityInfo.PriceDiffCount > biggestLoser.PriceDiffCount)
                    biggestLoser = securityInfo;
            }

            if (args.Contains("--biggest-loser"))
            {
                biggestLoser?.PrintBiggestLoser();
            }
        }

        public static QuandlSecurityInfo ReadStreamForSecurityData(string ticker)
        {
            string url =
                "https://www.quandl.com/api/v3/datatables/WIKI/PRICES.csv?ticker=" + ticker + "&qopts.columns=" + Columns + "&api_key=" + ApiKey;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);//
            request.Method = "Get";
            request.KeepAlive = true;
            request.ContentType = "appication/json";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (ReferenceEquals(response, null))
                throw new NullReferenceException(nameof(response));
            var stream = response.GetResponseStream();
            if (ReferenceEquals(stream, null))
                throw new NullReferenceException(nameof(stream));

            System.IO.StreamReader sr = new System.IO.StreamReader(stream);
            sr.ReadLine();
            QuandlSecurityInfo securityInfo = new QuandlSecurityInfo { SecurtiyName = ticker };
            while (!sr.EndOfStream)
            {
                var readLine = sr.ReadLine();
                if (readLine != null)
                {
                   securityInfo.ProcessCsvInfo(readLine);
                }
            }            
            return securityInfo;

        }
    }
}
