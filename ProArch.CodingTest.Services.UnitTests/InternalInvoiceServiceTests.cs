using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProArch.CodingTest.Common.Models;
using ProArch.CodingTest.Common.Repository;
using System;
using System.Linq;

namespace ProArch.CodingTest.Services.UnitTests
{
    [TestClass]
    public class InternalInvoiceServiceTests
    {
        private InternalInvoiceService GetInstance(IQueryable<Invoice> testInvoices = null)
        {
            // setup invoice repo
            var invoiceRepositoryMock = new Mock<IInvoiceRepository>();
            invoiceRepositoryMock.Setup(repo => repo.WithInvoices(It.IsAny<Action<IQueryable<Invoice>>>()))
                    .Callback<Action<IQueryable<Invoice>>>(a =>
                    {
                        a(testInvoices ?? (new Invoice[0]).AsQueryable());
                    });

            return new InternalInvoiceService(invoiceRepositoryMock.Object);
        }

        [TestMethod]
        public void ShouldReturnCorrectSpendForSupplier()
        {
            // arrange
            var invoices = new[] {
                new Invoice()
            {
                Id = 1,
                InvoiceDate = new DateTime(2018,1,1),
                SupplierId = 1,
                Amount = 10
            },
                new Invoice()
            {
                Id = 1,
                InvoiceDate = new DateTime(2018,1,2),
                SupplierId = 1,
                Amount = 20
            },
                new Invoice()
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
            Assert.AreEqual(InvoiceServiceCategory.Internal, instance.ServiceType);
        }
    }
}
