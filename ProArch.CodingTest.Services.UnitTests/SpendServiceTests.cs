using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Polly;
using Polly.CircuitBreaker;
using ProArch.CodingTest.Common.DTO;
using ProArch.CodingTest.Common.Exception;
using ProArch.CodingTest.Common.Repository;
using ProArch.CodingTest.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ProArch.CodingTest.Services.UnitTests
{
    [TestClass]
    public class SpendServiceTests
    {
        private SpendService GetInstance(SupplierData[] testSuppliers, Dictionary<InvoiceServiceType, IInvoiceService> strategyMapping, Policy policy = null)
        {
            // setup SupplierRepository
            var supplierRepositoryMock = new Mock<ISupplierRespository>();
            foreach (var supplier in testSuppliers)
            {
                supplierRepositoryMock.Setup(repo => repo.WithSupplierCompany(It.IsAny<Action<SupplierData>>(), supplier.Id))
                    .Callback<Action<SupplierData>, int>((a, id) =>
                     {
                         a(supplier);
                     });
            }

            // setup IInvoiceServiceStrategy
            var strategyMock = new Mock<IInvoiceServiceStrategy>();
            foreach (var kvp in strategyMapping)
            {
                strategyMock.Setup(s => s.GetService(kvp.Key))
                .Returns(kvp.Value);
            }

            // setup policy.
            if (policy == null)
            {
                policy = Policy.Handle<ExternalInvoiceServiceException>()
                   .CircuitBreaker(int.MaxValue, TimeSpan.FromSeconds(int.MaxValue));
            }


            return new SpendService(supplierRepositoryMock.Object, strategyMock.Object, policy);
        }

        [TestMethod]
        public void ShouldReturnCorrectInternalTotalSpend()
        {
            // arrange
            var expectedSupplier = new SupplierData()
            {
                Id = 1,
                IsExternal = false,
                Name = "Test Internal Suppliing Co",
            };
            var expectedYears = new List<SpendDetail>()
            {
                new SpendDetail() { Year = 2018, TotalSpend = 100},
                new SpendDetail() { Year = 2017, TotalSpend = 500},
            };
            var mockInvoiceService = new Mock<IInvoiceService>();
            mockInvoiceService.Setup(svc => svc.GetSpendDetails(1))
                .Returns(expectedYears);

            var strategyMapping = new Dictionary<InvoiceServiceType, IInvoiceService>(){
                { InvoiceServiceType.Internal, mockInvoiceService.Object } };


            var instance = GetInstance(new[] { expectedSupplier }, strategyMapping);

            // act
            var actual = instance.GetTotalSpend(1);

            // assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expectedSupplier.Name, actual.Name);
            Assert.AreEqual(expectedYears.Count, actual.Years.Count);

            var year2018 = actual.Years.FirstOrDefault(y => y.Year == 2018);
            Assert.AreEqual(year2018.TotalSpend, expectedYears[0].TotalSpend);

            var year2017 = actual.Years.FirstOrDefault(y => y.Year == 2017);
            Assert.AreEqual(year2017.TotalSpend, expectedYears[1].TotalSpend);

        }

        [TestMethod]
        public void ShouldReturnCorrectExtrernalTotalSpend()
        {
            // arrange
            var expectedSupplier = new SupplierData()
            {
                Id = 1,
                IsExternal = true,
                Name = "Test External Suppliing Co",
            };
            var expectedYears = new List<SpendDetail>()
            {
                new SpendDetail() { Year = 2018, TotalSpend = 900},
                new SpendDetail() { Year = 2017, TotalSpend = 1200},
            };
            var mockInvoiceService = new Mock<IInvoiceService>();
            mockInvoiceService.Setup(svc => svc.GetSpendDetails(1))
                .Returns(expectedYears);

            var strategyMapping = new Dictionary<InvoiceServiceType, IInvoiceService>(){
                { InvoiceServiceType.External, mockInvoiceService.Object } };


            var instance = GetInstance(new[] { expectedSupplier }, strategyMapping);

            // act
            var actual = instance.GetTotalSpend(1);

            // assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expectedSupplier.Name, actual.Name);
            Assert.AreEqual(expectedYears.Count, actual.Years.Count);

            var year2018 = actual.Years.FirstOrDefault(y => y.Year == 2018);
            Assert.AreEqual(year2018.TotalSpend, expectedYears[0].TotalSpend);

            var year2017 = actual.Years.FirstOrDefault(y => y.Year == 2017);
            Assert.AreEqual(year2017.TotalSpend, expectedYears[1].TotalSpend);

        }

        [TestMethod]
        public void ShouldReturnFailoverDataOnExternalExcpetion()
        {
            // arrange
            var expectedSupplier = new SupplierData()
            {
                Id = 1,
                IsExternal = true,
                Name = "Test External Suppliing Co",
            };
            var expectedYears = new List<SpendDetail>()
            {
                new SpendDetail() { Year = 2018, TotalSpend = 1200},
                new SpendDetail() { Year = 2017, TotalSpend = 2400},
            };

            // external invoice service throws exception
            var mockExternalInvoiceService = new Mock<IInvoiceService>();
            mockExternalInvoiceService.Setup(svc => svc.GetSpendDetails(1))
                .Throws(new ExternalInvoiceServiceException());

            var mockFailoverInvoiceService = new Mock<IInvoiceService>();
            mockFailoverInvoiceService.Setup(svc => svc.GetSpendDetails(1))
                .Returns(expectedYears);

            var strategyMapping = new Dictionary<InvoiceServiceType, IInvoiceService>(){
                { InvoiceServiceType.External, mockExternalInvoiceService.Object },
                { InvoiceServiceType.Failover, mockFailoverInvoiceService.Object }
            };

            var instance = GetInstance(new[] { expectedSupplier }, strategyMapping);

            // act
            var actual = instance.GetTotalSpend(1);

            // assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expectedSupplier.Name, actual.Name);
            Assert.AreEqual(expectedYears.Count, actual.Years.Count);

            var year2018 = actual.Years.FirstOrDefault(y => y.Year == 2018);
            Assert.AreEqual(year2018.TotalSpend, expectedYears[0].TotalSpend);

            var year2017 = actual.Years.FirstOrDefault(y => y.Year == 2017);
            Assert.AreEqual(year2017.TotalSpend, expectedYears[1].TotalSpend);

        }

        [TestMethod]
        public void ShouldReturnFailoverDataAfterConfiguredNumberOfExternalExcpetions()
        {
            // arrange
            var goodSuplier = new SupplierData()
            {
                Id = 1,
                IsExternal = true,
                Name = "Test Good External Suppliing Co",
            };
            var badSuplier = new SupplierData()
            {
                Id = 2,
                IsExternal = true,
                Name = "Test Bad External Suppliing Co",
            };
            var externalInvoiceYears = new List<SpendDetail>()
            {
                new SpendDetail() { Year = 2018, TotalSpend = 200},
                new SpendDetail() { Year = 2017, TotalSpend = 400},
            };

            var failoverInvoiceYears = new List<SpendDetail>()
            {
                new SpendDetail() { Year = 2018, TotalSpend = 1200},
                new SpendDetail() { Year = 2017, TotalSpend = 2400},
            };


            // external invoice service throws exception for suppler id 1
            // , but return good value for supplier id 2
            var mockExternalInvoiceService = new Mock<IInvoiceService>();
            mockExternalInvoiceService.Setup(svc => svc.GetSpendDetails(goodSuplier.Id))
              .Returns(externalInvoiceYears);
            mockExternalInvoiceService.Setup(svc => svc.GetSpendDetails(badSuplier.Id))
                .Throws(new ExternalInvoiceServiceException());

            // failover invoice service
            var mockFailoverInvoiceService = new Mock<IInvoiceService>();
            mockFailoverInvoiceService.Setup(svc => svc.GetSpendDetails(It.IsAny<int>()))
                .Returns(failoverInvoiceYears);

            var strategyMapping = new Dictionary<InvoiceServiceType, IInvoiceService>(){
                { InvoiceServiceType.External, mockExternalInvoiceService.Object },
                { InvoiceServiceType.Failover, mockFailoverInvoiceService.Object }
            };

            // setup to fall back after 2 external excpetions.
            var policy = Policy.Handle<ExternalInvoiceServiceException>()
                .CircuitBreaker(2, TimeSpan.FromSeconds(int.MaxValue));

            var instance = GetInstance(new[] { goodSuplier, badSuplier }, strategyMapping, policy);

            // act
            var firstGoodCall = instance.GetTotalSpend(goodSuplier.Id);
            var firstBadCall = instance.GetTotalSpend(badSuplier.Id); 
            var secondBadCall = instance.GetTotalSpend(badSuplier.Id);
            var secondGoodCall = instance.GetTotalSpend(goodSuplier.Id); 
            
            // assert -- first good call, gets from external
            Assert.IsNotNull(firstGoodCall);
            Assert.AreEqual(goodSuplier.Name, firstGoodCall.Name);
            Assert.AreEqual(externalInvoiceYears.Count, firstGoodCall.Years.Count);
            var firstGoodCallYear2018 = firstGoodCall.Years.FirstOrDefault(y => y.Year == 2018);
            Assert.AreEqual(firstGoodCallYear2018.TotalSpend, externalInvoiceYears[0].TotalSpend);
            var firstGoodCallYear2017 = firstGoodCall.Years.FirstOrDefault(y => y.Year == 2017);
            Assert.AreEqual(firstGoodCallYear2017.TotalSpend, externalInvoiceYears[1].TotalSpend);


            // assert -- second good call, gets from fall back data
            Assert.IsNotNull(secondGoodCall);
            Assert.AreEqual(goodSuplier.Name, secondGoodCall.Name);
            Assert.AreEqual(externalInvoiceYears.Count, secondGoodCall.Years.Count);
            var secondGoodCallYear2018 = secondGoodCall.Years.FirstOrDefault(y => y.Year == 2018);
            Assert.AreEqual(secondGoodCallYear2018.TotalSpend, failoverInvoiceYears[0].TotalSpend);
            var secondGoodCallYear2017 = secondGoodCall.Years.FirstOrDefault(y => y.Year == 2017);
            Assert.AreEqual(secondGoodCallYear2017.TotalSpend, failoverInvoiceYears[1].TotalSpend);

        }


        [TestMethod]
        public void ShouldReturnExternalDataAfterConfiguredNumberOfExternalExcpetionsAndConfiguredWaitPeriod()
        {
            // arrange
            var goodSuplier = new SupplierData()
            {
                Id = 1,
                IsExternal = true,
                Name = "Test Good External Suppliing Co",
            };
            var badSuplier = new SupplierData()
            {
                Id = 2,
                IsExternal = true,
                Name = "Test Bad External Suppliing Co",
            };
            var externalInvoiceYears = new List<SpendDetail>()
            {
                new SpendDetail() { Year = 2018, TotalSpend = 200},
                new SpendDetail() { Year = 2017, TotalSpend = 400},
            };

            var failoverInvoiceYears = new List<SpendDetail>()
            {
                new SpendDetail() { Year = 2018, TotalSpend = 1200},
                new SpendDetail() { Year = 2017, TotalSpend = 2400},
            };


            // external invoice service throws exception for suppler id 1
            // , but return good value for supplier id 2
            var mockExternalInvoiceService = new Mock<IInvoiceService>();
            mockExternalInvoiceService.Setup(svc => svc.GetSpendDetails(goodSuplier.Id))
              .Returns(externalInvoiceYears);
            mockExternalInvoiceService.Setup(svc => svc.GetSpendDetails(badSuplier.Id))
                .Throws(new ExternalInvoiceServiceException());

            // failover invoice service
            var mockFailoverInvoiceService = new Mock<IInvoiceService>();
            mockFailoverInvoiceService.Setup(svc => svc.GetSpendDetails(It.IsAny<int>()))
                .Returns(failoverInvoiceYears);

            var strategyMapping = new Dictionary<InvoiceServiceType, IInvoiceService>(){
                { InvoiceServiceType.External, mockExternalInvoiceService.Object },
                { InvoiceServiceType.Failover, mockFailoverInvoiceService.Object }
            };
            // setup to fall back after given no of external excpetions and for given wait duration
            int waitDurationInSecond = 5;
            var policy = Policy.Handle<ExternalInvoiceServiceException>()
                .CircuitBreaker(2, TimeSpan.FromSeconds(waitDurationInSecond));

            var instance = GetInstance(new[] { goodSuplier, badSuplier }, strategyMapping, policy);

            // act
            var firstGoodCall = instance.GetTotalSpend(goodSuplier.Id);
            var firstBadCall = instance.GetTotalSpend(badSuplier.Id);
            var secondBadCall = instance.GetTotalSpend(badSuplier.Id);
            var secondGoodCall = instance.GetTotalSpend(goodSuplier.Id);
            // wait for some time
            Thread.Sleep(waitDurationInSecond * 1000);
            // then call again
            var thirdGoodCall = instance.GetTotalSpend(goodSuplier.Id);

            // assert -- first good call, gets from external
            Assert.IsNotNull(firstGoodCall);
            Assert.AreEqual(goodSuplier.Name, firstGoodCall.Name);
            Assert.AreEqual(externalInvoiceYears.Count, firstGoodCall.Years.Count);
            var firstGoodCallYear2018 = firstGoodCall.Years.FirstOrDefault(y => y.Year == 2018);
            Assert.AreEqual(firstGoodCallYear2018.TotalSpend, externalInvoiceYears[0].TotalSpend);
            var firstGoodCallYear2017 = firstGoodCall.Years.FirstOrDefault(y => y.Year == 2017);
            Assert.AreEqual(firstGoodCallYear2017.TotalSpend, externalInvoiceYears[1].TotalSpend);


            // assert -- second good call, gets from fall back data
            Assert.IsNotNull(secondGoodCall);
            Assert.AreEqual(goodSuplier.Name, secondGoodCall.Name);
            Assert.AreEqual(externalInvoiceYears.Count, secondGoodCall.Years.Count);
            var secondGoodCallYear2018 = secondGoodCall.Years.FirstOrDefault(y => y.Year == 2018);
            Assert.AreEqual(secondGoodCallYear2018.TotalSpend, failoverInvoiceYears[0].TotalSpend);
            var secondGoodCallYear2017 = secondGoodCall.Years.FirstOrDefault(y => y.Year == 2017);
            Assert.AreEqual(secondGoodCallYear2017.TotalSpend, failoverInvoiceYears[1].TotalSpend);

            // assert -- third good call, gets from external again
            Assert.IsNotNull(thirdGoodCall);
            Assert.AreEqual(goodSuplier.Name, thirdGoodCall.Name);
            Assert.AreEqual(externalInvoiceYears.Count, thirdGoodCall.Years.Count);
            var thirdGoodCallYear2018 = thirdGoodCall.Years.FirstOrDefault(y => y.Year == 2018);
            Assert.AreEqual(thirdGoodCallYear2018.TotalSpend, externalInvoiceYears[0].TotalSpend);
            var thirdGoodCallYear2017 = thirdGoodCall.Years.FirstOrDefault(y => y.Year == 2017);
            Assert.AreEqual(thirdGoodCallYear2017.TotalSpend, externalInvoiceYears[1].TotalSpend);

        }
    }
}
