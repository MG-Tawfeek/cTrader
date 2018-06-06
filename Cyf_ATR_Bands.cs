using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Cyf_ATR_Bands : Indicator
    {
        private AverageTrueRange ATR;
        private ExponentialMovingAverage EMA;

        [Parameter("ATR_Period", DefaultValue = 14)]
        public int ATR_Period { get; set; }

        [Output("Up_ATR", Color = Colors.White)]
        public IndicatorDataSeries Up_ATR { get; set; }

        [Output("Dn_ATR", Color = Colors.Red)]
        public IndicatorDataSeries Dn_ATR { get; set; }




        protected override void Initialize()
        {
            ATR = Indicators.AverageTrueRange(ATR_Period, MovingAverageType.Exponential);
            EMA = Indicators.ExponentialMovingAverage(MarketSeries.Close, 13);
        }

        public override void Calculate(int index)
        {
            Up_ATR[index] = EMA.Result[index] + (ATR.Result[index] * 3);
            Dn_ATR[index] = EMA.Result[index] - (ATR.Result[index] * 3);
        }
    }
}
