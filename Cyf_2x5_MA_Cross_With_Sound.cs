using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Cyf_2x5_MA_Cross_With_Sound : Indicator
    {
        private VerticalAlignment vAlign = VerticalAlignment.Center;
        private HorizontalAlignment hAlign = HorizontalAlignment.Center;
        private int count;

        private MovingAverage FastMA, SlowMA;
        private AverageTrueRange ATR;

        [Parameter(DefaultValue = true)]
        public bool SoundAlert { get; set; }

        [Parameter(DefaultValue = "")]
        public string SoundFile { get; set; }

        [Parameter("Fast_Period", DefaultValue = 2)]
        public int Fast_Period { get; set; }

        [Parameter("Slow_Period", DefaultValue = 5)]
        public int Slow_Period { get; set; }

        [Parameter("MA_Type", DefaultValue = 5)]
        public MovingAverageType MA_Type { get; set; }


        [Parameter("History", DefaultValue = 200)]
        public int Historical_Bars { get; set; }

        protected override void Initialize()
        {
            FastMA = Indicators.MovingAverage(MarketSeries.Close, Fast_Period, MA_Type);
            SlowMA = Indicators.MovingAverage(MarketSeries.Open, Slow_Period, MA_Type);
            ATR = Indicators.AverageTrueRange(14, MovingAverageType.Simple);
            count = MarketSeries.Open.Count;


        }

        public override void Calculate(int index)
        {
            if (index < count - Historical_Bars)
            {
                return;
            }


            if (FastMA.Result[index] > SlowMA.Result[index] && FastMA.Result[index - 1] < SlowMA.Result[index - 1])
            {

                ChartObjects.DrawText("BuySignal" + index, "▲", index, MarketSeries.Low[index] - ATR.Result[index], vAlign, hAlign, Colors.Green);
                if (SoundAlert)
                {
                    Notifications.PlaySound(SoundFile);
                }
            }

            if (FastMA.Result[index] < SlowMA.Result[index] && FastMA.Result[index - 1] > SlowMA.Result[index - 1])
            {

                ChartObjects.DrawText("SellSignal" + index, "▼", index, MarketSeries.High[index] + ATR.Result[index], vAlign, hAlign, Colors.Red);
                if (SoundAlert)
                {
                    Notifications.PlaySound(SoundFile);
                }
            }

        }
    }
}
