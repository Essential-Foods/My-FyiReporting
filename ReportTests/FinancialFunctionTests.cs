using System;
using Majorsilence.Reporting.Rdl;
using NUnit.Framework;

namespace ReportTests
{
    [TestFixture]
    public class FinancialFunctionTests
    {
        private const double Tolerance = 0.0001;

        // DDB — double-declining balance depreciation

        [Test]
        public void DDB_FirstPeriod_ReturnsCorrectDepreciation()
        {
            // cost=1000, salvage=0, life=5, period=1 → 1000 * 2/5 = 400
            Assert.That(Financial.DDB(1000, 0, 5, 1), Is.EqualTo(400.0).Within(Tolerance));
        }

        [Test]
        public void DDB_SecondPeriod_ReturnsCorrectDepreciation()
        {
            // (1000-400) * 2/5 = 240
            Assert.That(Financial.DDB(1000, 0, 5, 2), Is.EqualTo(240.0).Within(Tolerance));
        }

        [Test]
        public void DDB_LastPeriod_ForcesDepreciationToSalvage()
        {
            // Final period: cost - salvage - accumulated = forced to make book value = salvage
            double result = Financial.DDB(1000, 100, 5, 5);
            Assert.That(result, Is.GreaterThan(0));
            Assert.That(double.IsNaN(result), Is.False);
        }

        [Test]
        public void DDB_CustomFactor_UsesProvidedFactor()
        {
            // factor=1.5: (1000-0) * 1.5/5 = 300
            Assert.That(Financial.DDB(1000, 0, 5, 1, 1.5), Is.EqualTo(300.0).Within(Tolerance));
        }

        [Test]
        public void DDB_PeriodExceedsLife_ReturnsNaN()
        {
            Assert.That(double.IsNaN(Financial.DDB(1000, 0, 5, 6)), Is.True);
        }

        [Test]
        public void DDB_PeriodZero_ReturnsNaN()
        {
            Assert.That(double.IsNaN(Financial.DDB(1000, 0, 5, 0)), Is.True);
        }

        // SLN — straight-line depreciation

        [Test]
        public void SLN_Standard_ReturnsCorrectDepreciation()
        {
            // (30000 - 7500) / 10 = 2250
            Assert.That(Financial.SLN(30000, 7500, 10), Is.EqualTo(2250.0).Within(Tolerance));
        }

        [Test]
        public void SLN_ZeroCostAndSalvage_ReturnsZero()
        {
            Assert.That(Financial.SLN(0, 0, 5), Is.EqualTo(0.0).Within(Tolerance));
        }

        [Test]
        public void SLN_ZeroLife_ReturnsNaN()
        {
            Assert.That(double.IsNaN(Financial.SLN(1000, 0, 0)), Is.True);
        }

        // SYD — sum-of-years-digits depreciation

        [Test]
        public void SYD_FirstPeriod_OddLife_ReturnsCorrectValue()
        {
            // life=3 (odd): sumOfPeriods = (3+1)/2 * 3 = 6
            // period=1: (3+1-1) * (1000-0) / 6 = 3000/6 = 500
            Assert.That(Financial.SYD(1000, 0, 3, 1), Is.EqualTo(500.0).Within(Tolerance));
        }

        [Test]
        public void SYD_AllPeriodsSumToCostMinusSalvage()
        {
            double total = Financial.SYD(1000, 100, 5, 1)
                         + Financial.SYD(1000, 100, 5, 2)
                         + Financial.SYD(1000, 100, 5, 3)
                         + Financial.SYD(1000, 100, 5, 4)
                         + Financial.SYD(1000, 100, 5, 5);
            Assert.That(total, Is.EqualTo(900.0).Within(Tolerance));
        }

        [Test]
        public void SYD_LaterPeriodIsLess_ThanEarlierPeriod()
        {
            double period1 = Financial.SYD(1000, 0, 5, 1);
            double period5 = Financial.SYD(1000, 0, 5, 5);
            Assert.That(period1, Is.GreaterThan(period5));
        }

        // FV — future value

        [Test]
        public void FV_ZeroRate_ReturnsCorrectFutureValue()
        {
            // FV(0, 12, -100, 0, true) = -(-100*12 + 0) = 1200
            Assert.That(Financial.FV(0, 12, -100, 0, true), Is.EqualTo(1200.0).Within(Tolerance));
        }

        [Test]
        public void FV_NonZeroRate_EndOfPeriod_ReturnsCorrectValue()
        {
            // $100 invested at 5% for 1 period: FV = 105
            // FV(0.05, 1, 0, -100, true): type=0, temp=1.05
            // fv = -((-100*1.05) + 0) = 105
            Assert.That(Financial.FV(0.05, 1, 0, -100, true), Is.EqualTo(105.0).Within(Tolerance));
        }

        // PV — present value

        [Test]
        public void PV_ZeroRate_ReturnsCorrectPresentValue()
        {
            // PV(0, 12, 100, 0, true) = -(100*12 + 0) = -1200
            Assert.That(Financial.PV(0, 12, 100, 0, true), Is.EqualTo(-1200.0).Within(Tolerance));
        }

        [Test]
        public void PV_NonZeroRate_EndOfPeriod_ReturnsCorrectValue()
        {
            // PV of receiving $1100 in 1 period at 10%: PV = -1000
            // PV(0.10, 1, 0, 1100, true): type=0, temp=1.10
            // pv = -(0 + 1100)/1.10 = -1000
            Assert.That(Financial.PV(0.10, 1, 0, 1100, true), Is.EqualTo(-1000.0).Within(Tolerance));
        }

        [Test]
        public void PV_FV_AreInverse()
        {
            // PV of what FV produces should round-trip back to original
            double rate = 0.05;
            int periods = 10;
            double originalPV = -1000.0;
            double fv = Financial.FV(rate, periods, 0, originalPV, true);
            double pv = Financial.PV(rate, periods, 0, fv, true);
            Assert.That(pv, Is.EqualTo(originalPV).Within(Tolerance));
        }

        // Pmt — periodic payment

        [Test]
        public void Pmt_ZeroRate_ReturnsCorrectPayment()
        {
            // Pmt(0, 12, 1200, 0, true) = -(1200+0)/12 = -100
            Assert.That(Financial.Pmt(0, 12, 1200, 0, true), Is.EqualTo(-100.0).Within(Tolerance));
        }

        [Test]
        public void Pmt_NonZeroRate_EndOfPeriod_ReturnsCorrectPayment()
        {
            // $1000 loan at 10% for 1 period: payment = $1100
            // Pmt(0.10, 1, 1000, 0, true): type=0, temp=1.10
            // pmt = -((1000*1.10) + 0) / ((1+0)*((1.10-1)/0.10)) = -(1100)/(1) = -1100
            Assert.That(Financial.Pmt(0.10, 1, 1000, 0, true), Is.EqualTo(-1100.0).Within(Tolerance));
        }

        [Test]
        public void Pmt_ZeroPeriods_ReturnsNaN()
        {
            Assert.That(double.IsNaN(Financial.Pmt(0.05, 0, 1000, 0, true)), Is.True);
        }

        // NPer — number of periods

        [Test]
        public void NPer_ZeroRate_ReturnsCorrectPeriods()
        {
            // NPer(0, 100, -1200, 0, true) = -(-1200+0)/100 = 12
            Assert.That(Financial.NPer(0, 100, -1200, 0, true), Is.EqualTo(12.0).Within(Tolerance));
        }

        [Test]
        public void NPer_ZeroPayment_ReturnsNaN()
        {
            Assert.That(double.IsNaN(Financial.NPer(0.05, 0, 1000, 0, true)), Is.True);
        }

        // IPmt — interest portion of payment

        [Test]
        public void IPmt_SinglePeriodLoan_ReturnsFullInterest()
        {
            // $1000 loan at 10% for 1 period: interest = -100
            Assert.That(Financial.IPmt(0.10, 1, 1, 1000, 0, true), Is.EqualTo(-100.0).Within(Tolerance));
        }

        [Test]
        public void IPmt_BeginningOfPeriod_ThrowsException()
        {
            Assert.Throws<Exception>(() => Financial.IPmt(0.10, 1, 12, 1000, 0, false));
        }

        // Rate — interest rate per period

        [Test]
        public void Rate_SimpleLoan_ReturnsApproximateRate()
        {
            // 1-period loan: borrow 1000, pay 1100 → rate ≈ 10%
            // Rate(1, -1100, 1000, 0, true, 0.1)
            double rate = Financial.Rate(1, -1100, 1000, 0, true, 0.1);
            Assert.That(rate, Is.EqualTo(0.10).Within(0.001));
        }
    }
}
