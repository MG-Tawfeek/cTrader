using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Cyf_Strength_Arrow_MTF : Indicator
    {

        private VerticalAlignment vAlign = VerticalAlignment.Center;
        private HorizontalAlignment hAlign = HorizontalAlignment.Center;

        private RelativeStrengthIndex RSI;
        private AverageTrueRange ATR;
        private int count;

        [Parameter("Period", DefaultValue = 5)]
        public int Period { get; set; }

        [Parameter("History", DefaultValue = 20)]
        public int Historical_Bars { get; set; }

        protected override void Initialize()
        {
            RSI = Indicators.RelativeStrengthIndex(MarketSeries.Close, Period);
            ATR = Indicators.AverageTrueRange(14, MovingAverageType.Simple);
            count = MarketSeries.Open.Count;
        }

        public override void Calculate(int index)
        {
            if (index < count - Historical_Bars)
            {
                return;
            }

            if (RSI.Result[index] > 25 && RSI.Result[index - 1] < 25)
            {
                ChartObjects.DrawText("BuySignal" + index, "⤊", index, MarketSeries.Low[index] - ATR.Result[index], vAlign, hAlign, Colors.Green);
            }

            if (RSI.Result[index] < 75 && RSI.Result[index - 1] > 75)
            {
                ChartObjects.DrawText("SellSignal" + index, "⤋", index, MarketSeries.High[index] + ATR.Result[index], vAlign, hAlign, Colors.Red);
            }

        }
    }
}
