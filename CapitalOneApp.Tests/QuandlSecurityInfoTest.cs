using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CapitalOneApp.Tests
{
    [TestClass]
    public class QuandlSecurityInfoTest
    {
        [TestMethod]
        public void ProcessCsvInfo_Works()
        {
            //Arrange
            var info = new []{"Test,2017-02-01,10,20,5,35,1000", "Test,2017-02-05,20,40,0,100,1000"};

            var security = new QuandlSecurityInfo {SecurtiyName = "Test"};

            //Act
            foreach (var s in info)
            {
                security.ProcessCsvInfo(s);
            }
            
            //Assert
            Assert.AreEqual(security.TableData.Keys.Count, 1);
            Assert.AreEqual(security.TableData.Keys.ToList()[0], 2);
            Assert.AreEqual(security.AverageOpenForMonth(2), 15);
            Assert.AreEqual(security.AverageCloseForMonth(2), 30);
            Assert.AreEqual(security.PriceDiffCount, 0);
            Assert.AreEqual(security.MaxPriceDate.Date, new DateTime(2017, 2, 5));
            Assert.AreEqual(security.MaxPriceDate.HighLowDiffPrice, 100);
        }

        [TestMethod]
        public void HighLowLogic_Works()
        {
            //Arrange
            var info = new[] { "Test,2017-02-01,10,20,5,35,1000", "Test,2017-02-05,20,16,0,5,1000" };

            var security = new QuandlSecurityInfo { SecurtiyName = "Test" };

            //Act
            foreach (var s in info)
            {
                security.ProcessCsvInfo(s);
            }

            //Assert
            Assert.AreEqual(security.TableData.Keys.Count, 1);
            Assert.AreEqual(security.TableData.Keys.ToList()[0], 2);
            Assert.AreEqual(security.AverageOpenForMonth(2), 15);
            Assert.AreEqual(security.AverageCloseForMonth(2), 18);
            Assert.AreEqual(security.PriceDiffCount, 1);
            Assert.AreEqual(security.MaxPriceDate.Date, new DateTime(2017, 2, 1));
            Assert.AreEqual(security.MaxPriceDate.HighLowDiffPrice, 30);
        }

        [TestMethod]
        public void BusyDayLogic_Works()
        {
            //Arrange
            var info = new[]
            {
                "Test,2017-02-01,10,20,5,35,10", "Test,2017-02-05,20,16,0,5,20",
                "Test,2017-02-03,10,20,5,35,30", "Test,2017-02-07,10,20,5,35,40",
                "Test,2017-02-04,10,20,5,35,50", "Test,2017-02-08,10,20,5,35,60",
                "Test,2017-02-12,10,20,5,35,70", "Test,2017-02-09,10,20,5,35,80"
            };

            var security = new QuandlSecurityInfo { SecurtiyName = "Test" };

            //Act
            foreach (var s in info)
            {
                security.ProcessCsvInfo(s);
            }
            var avgVolume = security.AverageVolumeForMonth(2);
            var tableData = security.CalculateBusyDays();    
            //Assert
            Assert.AreEqual(tableData.Count, 4);
            Assert.AreEqual(tableData[0].Volume, 50);
            Assert.AreEqual(tableData[1].Volume, 60);
            Assert.AreEqual(tableData[2].Volume, 70);
            Assert.AreEqual(tableData[3].Volume, 80);
        }

        [TestMethod]
        public void BiggestLoserLogic_Works()
        {
            //Arrange
            var info = new[]
            {
                "Test,2017-02-01,10,20,5,35,10", "Test,2017-02-05,20,16,0,5,20",
                "Test,2017-02-03,30,31,5,35,30", "Test,2017-02-07,32,31,5,35,40",
                "Test,2017-02-04,10,11,5,35,50", "Test,2017-02-08,40,45,5,35,60",
                "Test,2017-02-12,100,50,5,35,70", "Test,2017-02-09,70,65,5,35,80"
            };

            var security = new QuandlSecurityInfo { SecurtiyName = "Test" };

            //Act
            foreach (var s in info)
            {
                security.ProcessCsvInfo(s);
            }
            //Assert
            Assert.AreEqual(security.PriceDiffCount, 4);
        }

        [TestMethod]
        public void MaxDailyProfitLogic_Works()
        {
            //Arrange
            var info = new[]
            {
                "Test,2017-02-01,10,20,30,35,10", "Test,2017-02-05,20,16,5,25,20",
                "Test,2017-02-03,30,31,5,35,30", "Test,2017-02-07,32,31,30,25,40",
                "Test,2017-02-04,10,11,5,50,50", "Test,2017-02-08,40,45,50,80,60",
                "Test,2017-02-12,100,50,5,35,70", "Test,2017-02-09,70,65,5,35,80",
                "Test,2017-05-11,100,50,5,100,70", "Test,2017-05-12,70,65,5,35,80",
            };

            var security = new QuandlSecurityInfo { SecurtiyName = "Test" };

            //Act
            foreach (var s in info)
            {
                security.ProcessCsvInfo(s);
            }
            //Assert
            Assert.AreEqual(security.MaxPriceDate.Date, new DateTime(2017, 5, 11));
            Assert.AreEqual(security.MaxPriceDate.HighLowDiffPrice, 95);
        }

        [TestMethod]
        public void AverageVolumeLogic_Works()
        {
            //Arrange
            var info = new[]
            {
                "Test,2017-02-01,10,20,30,35,10", "Test,2017-02-05,20,16,5,25,20",
                "Test,2017-02-03,30,31,5,35,30", "Test,2017-02-07,32,31,30,25,40",
                "Test,2017-02-04,10,11,5,50,50", "Test,2017-02-08,40,45,50,80,60",
                "Test,2017-02-12,100,50,5,35,70", "Test,2017-02-09,70,65,5,35,80",
                "Test,2017-05-11,100,50,5,100,70", "Test,2017-05-12,70,65,5,35,80",
            };

            var security = new QuandlSecurityInfo { SecurtiyName = "Test" };

            //Act
            foreach (var s in info)
            {
                security.ProcessCsvInfo(s);
            }
            //Assert
            Assert.AreEqual(security.AverageVolumeForMonth(5), 75);
            Assert.AreEqual(security.AverageVolumeForMonth(2), 45);
        }

        [TestMethod]
        public void AverageOpenPriceLogic_Works()
        {
            //Arrange
            var info = new[]
            {
                "Test,2017-02-01,10,20,30,35,10", "Test,2017-02-05,20,16,5,25,20",
                "Test,2017-02-03,30,31,5,35,30", "Test,2017-02-07,32,31,30,25,40",
                "Test,2017-02-04,10,11,5,50,50", "Test,2017-02-08,40,45,50,80,60",
                "Test,2017-02-12,100,50,5,35,70", "Test,2017-02-09,70,65,5,35,80",
                "Test,2017-05-11,100,50,5,100,70", "Test,2017-05-12,70,65,5,35,80",
            };

            var security = new QuandlSecurityInfo { SecurtiyName = "Test" };

            //Act
            foreach (var s in info)
            {
                security.ProcessCsvInfo(s);
            }
            //Assert
            Assert.AreEqual(security.AverageOpenForMonth(5), 85);
            Assert.AreEqual(security.AverageOpenForMonth(2), 39);
        }

        [TestMethod]
        public void AverageClosePriceLogic_Works()
        {
            //Arrange
            var info = new[]
            {
                "Test,2017-02-01,10,20,30,35,10", "Test,2017-02-05,20,16,5,25,20",
                "Test,2017-02-03,30,31,5,35,30", "Test,2017-02-07,32,31,30,25,40",
                "Test,2017-02-04,10,11,5,50,50", "Test,2017-02-08,40,45,50,80,60",
                "Test,2017-02-12,100,50,5,35,70", "Test,2017-02-09,70,65,5,35,80",
                "Test,2017-05-11,100,50,5,100,70", "Test,2017-05-12,70,65,5,35,80",
            };

            var security = new QuandlSecurityInfo { SecurtiyName = "Test" };

            //Act
            foreach (var s in info)
            {
                security.ProcessCsvInfo(s);
            }
            //Assert
            Assert.AreEqual(security.AverageCloseForMonth(5), 57.5);
            Assert.AreEqual(security.AverageCloseForMonth(2), 33.625);
        }
    }
}
