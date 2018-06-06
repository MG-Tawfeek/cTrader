using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, AutoRescale = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Cyf_Elder_Ray : Indicator
    {

        private ExponentialMovingAverage EMA;

        [Parameter("(Y)Bull/(N)Bear", DefaultValue = true)]
        public bool Draw { get; set; }

        [Parameter("Period", DefaultValue = 22)]
        public int Period { get; set; }

        [Output("Up Power", PlotType = PlotType.Histogram, Color = Colors.White, Thickness = 3)]
        public IndicatorDataSeries Bull { get; set; }

        [Output("Dn Power", PlotType = PlotType.Histogram, Color = Colors.DodgerBlue, Thickness = 3)]
        public IndicatorDataSeries Bear { get; set; }

        protected override void Initialize()
        {
            EMA = Indicators.ExponentialMovingAverage(MarketSeries.Close, Period);

        }

        public override void Calculate(int index)
        {
            var TheBull = double.NaN;
            var TheBear = double.NaN;
            if (Draw)
            {
                TheBull = (MarketSeries.High[index] - EMA.Result[index]);
                if (TheBull > 0)
                {
                    Bull[index] = TheBull;
                }
                else
                    Bear[index] = TheBull;




            }
            else if (Draw == false)
            {
                TheBear = (MarketSeries.Low[index] - EMA.Result[index]);
                if (TheBear > 0)
                {
                    Bull[index] = TheBear;
                }
                else
                    Bear[index] = TheBear;

            }

        }

    }
}



