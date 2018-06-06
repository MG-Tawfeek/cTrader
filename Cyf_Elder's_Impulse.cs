using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, ScalePrecision = 0, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Cyf_Impulse : Indicator
    {
        private string Bullet = "●";
        private VerticalAlignment vAlign = VerticalAlignment.Center;
        private HorizontalAlignment hAlign = HorizontalAlignment.Center;
        private ExponentialMovingAverage EMA;
        private MacdHistogram Mac;
        private Colors UpColor = Colors.White;
        private Colors DnColor = Colors.Crimson;


        [Parameter("UpColor", DefaultValue = "White")]
        public string upColor { get; set; }

        [Parameter("DnColor", DefaultValue = "Crimson")]
        public string dnColor { get; set; }

        [Parameter(DefaultValue = 13)]
        public int Period { get; set; }

        [Parameter("LongCycle", DefaultValue = 26)]
        public int LongCycle { get; set; }

        [Parameter("ShrtCycle", DefaultValue = 12)]
        public int ShrtCycle { get; set; }

        [Parameter("Signal", DefaultValue = 9)]
        public int Signal { get; set; }


        protected override void Initialize()
        {
            Enum.TryParse(upColor, out UpColor);
            Enum.TryParse(dnColor, out DnColor);
            EMA = Indicators.ExponentialMovingAverage(MarketSeries.Close, Period);
            Mac = Indicators.MacdHistogram(LongCycle, ShrtCycle, Signal);

        }

        public override void Calculate(int index)
        {
            if (EMA.Result[index] > EMA.Result[index - 1] && Mac.Histogram[index] > Mac.Histogram[index - 1])
            {
                ChartObjects.DrawText("EMA_Dots" + index, Bullet, index, 0.5, vAlign, hAlign, UpColor);
                ChartObjects.DrawText("MAC_Dots" + index, Bullet, index, 0.7, vAlign, hAlign, UpColor);
            }

            else if (EMA.Result[index] < EMA.Result[index - 1] && Mac.Histogram[index] < Mac.Histogram[index - 1])
            {
                ChartObjects.DrawText("EMA_Dots" + index, Bullet, index, 0.5, vAlign, hAlign, DnColor);
                ChartObjects.DrawText("MAC_Dots" + index, Bullet, index, 0.7, vAlign, hAlign, DnColor);
            }
            else
            {
                ChartObjects.DrawText("EMA_Dots" + index, Bullet, index, 0.5, vAlign, hAlign, Colors.Black);
                ChartObjects.DrawText("MAC_Dots" + index, Bullet, index, 0.7, vAlign, hAlign, Colors.Black);

            }



        }
    }
}
