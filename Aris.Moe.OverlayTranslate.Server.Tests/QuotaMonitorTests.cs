using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aris.Moe.OverlayTranslate.Server.QuotaMonitoring;
using NSubstitute;
using Xunit;

namespace Aris.Moe.OverlayTranslate.Server.Tests
{
    public class QuotaMonitorTests
    {
        private readonly IQuotaRepository _repository;
        private readonly IQuotaMonitor _monitor;
        
        public QuotaMonitorTests()
        {
            _repository = Substitute.For<IQuotaRepository>();
            _monitor = new TestMonitor(_repository, "Test");
        }
        
        [Theory]
        [InlineData(0, 99)]
        [InlineData(50, 49)]
        [InlineData(98, 1)]
        [InlineData(99, 0)]
        public async Task VerifyQuota_EverythingCool_DoesNotThrowException(long alreadyUsed, long usage)
        {
            _repository.BillableUnitsSum(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<string?>())
                .Returns(alreadyUsed);

            await _monitor.VerifyQuotaSpace(usage);
        }
        
        [Theory]
        [InlineData(0, 100)]
        [InlineData(50, 51)]
        [InlineData(99, 1)]
        public async Task VerifyQuota_WillExceed_ThrowsException(long alreadyUsed, long usage)
        {
            _repository.BillableUnitsSum(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<string?>())
                .Returns(alreadyUsed);

            await Assert.ThrowsAsync<QuotaWouldBeExceededException>(async () =>
            {
                await _monitor.VerifyQuotaSpace(usage);
            });
        }
        
        [Theory]
        [InlineData(100)]
        [InlineData(101)]
        public async Task VerifyQuota_HasAlreadyExceeded_ThrowsException(long alreadyUsed)
        {
            _repository.BillableUnitsSum(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<string?>())
                .Returns(alreadyUsed);

            await Assert.ThrowsAsync<QuotaExceededException>(async () =>
            {
                await _monitor.VerifyQuotaSpace(0);
            });
        }
    }
    
    internal class TestMonitor : AbstractQuotaMonitor
    {
        public TestMonitor(IQuotaRepository quotaRepository, string type) : base(quotaRepository, new List<IQuotaConfig>
        {
            new TestQuota(),
            new TestQuota2()
        }, type)
        {
        }
    }
    
    internal class TestQuota : IQuotaConfig
    {
        public string Type { get; set; } = "Test";
        public double MonthlyEuroLimit { get; set; } = 10;
        public double EstimatedUnitCost { get; set; } = 0.1;
    }
    
    internal class TestQuota2 : IQuotaConfig
    {
        public string Type { get; set; } = "Test2";
        public double MonthlyEuroLimit { get; set; } = 100;
        public double EstimatedUnitCost { get; set; } = 0.1;
    }
}