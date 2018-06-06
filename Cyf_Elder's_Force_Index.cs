using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class NewIndicator : Indicator
    {
        private IndicatorDataSeries FI;
        private ExponentialMovingAverage EMA;


        [Parameter("(Y)Line/(N)Histogram", DefaultValue = true)]
        public bool shape { get; set; }

        [Parameter("EMA_Period", DefaultValue = 2)]
        public int EMA_Period { get; set; }

        [Output("FI_Line", PlotType = PlotType.Line)]
        public IndicatorDataSeries FI_Line { get; set; }

        [Output("FI_HistUp", PlotType = PlotType.Histogram, Thickness = 3, Color = Colors.White)]
        public IndicatorDataSeries FI_H_Up { get; set; }

        [Output("FI_HistDn", PlotType = PlotType.Histogram, Thickness = 3, Color = Colors.DodgerBlue)]
        public IndicatorDataSeries FI_H_Dn { get; set; }

        protected override void Initialize()
        {
            FI = CreateDataSeries();
            EMA = Indicators.ExponentialMovingAverage(FI, EMA_Period);
        }

        public override void Calculate(int index)
        {
            FI_Line[index] = double.NaN;
            FI_H_Up[index] = double.NaN;
            FI_H_Dn[index] = double.NaN;

            FI[index] = (MarketSeries.Close[index] - MarketSeries.Close[index - 1]) * MarketSeries.TickVolume[index];
            if (shape)
            {
                FI_Line[index] = EMA.Result[index] / MarketSeries.Close[index];
            }
            else
            {
                if (EMA.Result[index] > 0)
                {
                    FI_H_Up[index] = EMA.Result[index] / MarketSeries.Close[index];
                }
                else if (EMA.Result[index] < 0)
                {
                    FI_H_Dn[index] = EMA.Result[index] / MarketSeries.Close[index];
                }

            }



        }
    }
}
