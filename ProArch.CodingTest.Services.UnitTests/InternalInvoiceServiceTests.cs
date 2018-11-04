using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Polly;
using Polly.CircuitBreaker;
using ProArch.CodingTest.Common.Models;
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
    public class InternalInvoiceServiceTests
    {
        private InternalInvoiceService GetInstance(IQueryable<InvoiceData> testInvoices = null)
        {
            // setup invoice repo
            var invoiceRepositoryMock = new Mock<IInvoiceRespository>();
            invoiceRepositoryMock.Setup(repo => repo.WithInvoices(It.IsAny<Action<IQueryable<InvoiceData>>>()))
                    .Callback<Action<IQueryable<InvoiceData>>>(a =>
                    {
                        a(testInvoices ?? (new InvoiceData[0]).AsQueryable());
                    });

            return new InternalInvoiceService(invoiceRepositoryMock.Object);
        }

        [TestMethod]
        public void ShouldReturnCorrectSpendForSupplier()
        {
            // arrange
            var invoices = new[] {
                new InvoiceData()
            {
                Id = 1,
                InvoiceDate = new DateTime(2018,1,1),
                SupplierId = 1,
                Amount = 10
            },
                new InvoiceData()
            {
                Id = 1,
                InvoiceDate = new DateTime(2018,1,2),
                SupplierId = 1,
                Amount = 20
            },
                new InvoiceData()
            {
                Id = 1,
                InvoiceDate = new DateTime(2017,1,2),
                SupplierId = 1,
                Amount = 5
            }
            };


            var instance = GetInstance(invoices.AsQueryable());

            // act
            var actual = instance.GetSpendDetails(1).ToList();

            // assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Count);

            var year2018 = actual.First(y => y.Year == 2018);
            Assert.AreEqual(year2018.TotalSpend, 30);

            var year2017 = actual.First(y => y.Year == 2017);
            Assert.AreEqual(year2017.TotalSpend, 5);

        }

        [TestMethod]
        public void ShouldBeInternalInvoiceServiceType()
        {
            // arrange, act
            var instance = GetInstance();

            // assert
            Assert.AreEqual(InvoiceServiceType.Internal, instance.ServiceType);
        }
    }
}
