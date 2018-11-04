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
    public class ExternalInvoiceServiceTests
    {

        [TestMethod]
        public void ShouldReturnCorrectExternalData()
        {
            // arranage
            External.ExternalInvoiceService.IsServiceDown = false;
            External.ExternalInvoiceService.Invoices = new Dictionary<string, External.ExternalInvoice[]>()
            {
                { "1001", new [] { new External.ExternalInvoice(){ Year = 2018, TotalAmount = 100 },
                new External.ExternalInvoice(){ Year = 2017, TotalAmount = 200 }} }
            };

            //act
            var instance = new ExternalInvoiceService();
            var actual = instance.GetSpendDetails(1001);

            // asset
            Assert.IsNotNull(actual);
            var year2018 = actual.Where(y => y.Year == 2018).First();
            Assert.AreEqual(100, year2018.TotalSpend);
            var year2017 = actual.Where(y => y.Year == 2017).First();
            Assert.AreEqual(200, year2017.TotalSpend);
        }

        [ExpectedException(typeof(ExternalInvoiceServiceException))]
        [TestMethod]
        public void ShouldThowExternalServiceExcpetionWhenExternalServieIsDown()
        {
            // arranage
            External.ExternalInvoiceService.IsServiceDown = true;

            //act
            var instance = new ExternalInvoiceService();
            var actual = instance.GetSpendDetails(1001);

            // assert by attribure decoration

        }

        [TestMethod]
        public void ShouldBeExternalInvoiceServiceType()
        {
            // arrange, act
            var instance = new ExternalInvoiceService();

            // assert
            Assert.AreEqual(InvoiceServiceType.External, instance.ServiceType);
        }
    }
}
