using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None, ScalePrecision = 0)]
    [Levels(25, 50, 75)]
    public class Cyf_DTosc : Indicator
    {
        //The standard settings for DTosc is: 13,8,5,3
        //Other settings for DTosc are: (8,5,3,3), (21,13,8,8), (34,21,13,13)

        private RelativeStrengthIndex RSI;
        private MovingAverage MA_SK;
        private MovingAverage MA_SD;
        private IndicatorDataSeries STO_RSI;

        [Parameter("RSI_Period", DefaultValue = 13)]
        public int Rsi_Period { get; set; }

        [Parameter("Stoch_Period", DefaultValue = 8)]
        public int STOCh_Period { get; set; }

        [Parameter("PeriodSK", DefaultValue = 5)]
        public int PeriodSK { get; set; }

        [Parameter("PeriodSD", DefaultValue = 3)]
        public int PeriodSD { get; set; }

        [Parameter("MA_Type")]
        public MovingAverageType MA_Type { get; set; }

        [Output("MA_SK", Color = Colors.Yellow)]
        public IndicatorDataSeries MA_K { get; set; }

        [Output("MA_SD", Color = Colors.DodgerBlue)]
        public IndicatorDataSeries MA_D { get; set; }

        protected override void Initialize()
        {
            STO_RSI = CreateDataSeries();
            RSI = Indicators.RelativeStrengthIndex(MarketSeries.Close, Rsi_Period);
            MA_SK = Indicators.MovingAverage(STO_RSI, PeriodSK, MA_Type);
            MA_SD = Indicators.MovingAverage(MA_SK.Result, PeriodSD, MA_Type);
        }

        public override void Calculate(int index)
        {
            var HHV = Functions.Maximum(RSI.Result, STOCh_Period);
            var LLV = Functions.Minimum(RSI.Result, STOCh_Period);

            STO_RSI[index] = (100 * (RSI.Result[index] - LLV) / (HHV - LLV));

            MA_K[index] = MA_SK.Result[index];
            MA_D[index] = MA_SD.Result[index];

        }
    }
}
