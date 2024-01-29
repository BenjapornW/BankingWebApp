using System;
using Assignment2.Services;
using DataModelLibrary.Data;
using DataModelLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Assignment2.Test
{
	public class BillPayServiceTest
	{
        private readonly McbaContext _context;

        public BillPayServiceTest()
		{
            _context = new McbaContext(new DbContextOptionsBuilder<McbaContext>().
UseSqlite($"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared").Options);
            _context.Database.EnsureCreated();
            SeedData.Initialize(_context);
        }

        [Fact]
        public async Task PayScheduledBills_Should_Pay_Bills_With_ScheduleTime_Less_Than_CurrentTime()
        {
            // Arrange
            var currentTime = DateTime.Now;
            var bill1 = new BillPay
            {
                AccountNumber = 4100,
                Amount = 100, // Assuming sufficient balance
                PayeeID = 1,
                ScheduleTimeUtc = currentTime.AddMinutes(-1), // Scheduled in the past
                Period = PeriodType.OneOff
            };
            var bill2 = new BillPay
            {
                AccountNumber = 4101,
                Amount = 10, // Assuming sufficient balance
                PayeeID = 2,
                ScheduleTimeUtc = currentTime.AddMinutes(1), // Scheduled in the future
                Period = PeriodType.Monthly
            };
            _context.BillPays.AddRange(new List<BillPay> { bill1, bill2 });
            _context.SaveChanges();

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.CreateScope()).Returns(Mock.Of<IServiceScope>());
            serviceProviderMock.Setup(x => x.GetService(typeof(McbaContext))).Returns(_context);

            var billPayService = new BillPayService(serviceProviderMock.Object);

            // Act
            await billPayService.PayScheduledBills();

            // Assert
            Assert.Equal(StatusType.Scheduled, bill1.Status);
            Assert.Null(await _context.BillPays.FindAsync(bill2.BillPayID));
        }
    }
}
    


	


