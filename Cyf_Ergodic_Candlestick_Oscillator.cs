using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None, ScalePrecision = 0)]
    public class Cyf_Ergodic_Candlestick_Oscillator : Indicator
    {
        private ExponentialMovingAverage EMA_CO;
        private ExponentialMovingAverage EMA_CO_S;
        private ExponentialMovingAverage EMA_HL;
        private ExponentialMovingAverage EMA_HL_S;
        private IndicatorDataSeries CO_Series;
        private IndicatorDataSeries HL_Series;
        private double CO_Val;
        private double HL_Val;
        private double ECO;

        [Parameter(DefaultValue = 32, MinValue = 1)]
        public int Interval_1 { get; set; }

        [Parameter(DefaultValue = 12, MinValue = 1)]
        public int Interval_2 { get; set; }

        [Parameter("(Y)Line/(N)Hist", DefaultValue = false)]
        public bool Line { get; set; }

        [Output("ECO_Line", PlotType = PlotType.Line)]
        public IndicatorDataSeries ECO_Line { get; set; }

        [Output("ECO_Hist_Up", Color = Colors.White, PlotType = PlotType.Histogram, Thickness = 3)]
        public IndicatorDataSeries ECO_Hist_Up { get; set; }

        [Output("ECO_Hist_Dn", Color = Colors.DodgerBlue, PlotType = PlotType.Histogram, Thickness = 3)]
        public IndicatorDataSeries ECO_Hist_Dn { get; set; }

        protected override void Initialize()
        {
            CO_Series = CreateDataSeries();
            HL_Series = CreateDataSeries();

            EMA_CO = Indicators.ExponentialMovingAverage(CO_Series, Interval_1);
            EMA_CO_S = Indicators.ExponentialMovingAverage(EMA_CO.Result, Interval_2);

            EMA_HL = Indicators.ExponentialMovingAverage(HL_Series, Interval_1);
            EMA_HL_S = Indicators.ExponentialMovingAverage(EMA_HL.Result, Interval_2);

        }

        public override void Calculate(int index)
        {
            CO_Val = double.NaN;
            HL_Val = double.NaN;

            CO_Val = (MarketSeries.Close[index] - MarketSeries.Open[index]) * MarketSeries.TickVolume[index];
            HL_Val = (MarketSeries.High[index] - MarketSeries.Low[index]) * MarketSeries.TickVolume[index];

            CO_Series[index] = CO_Val;
            HL_Series[index] = HL_Val;

            ECO = 100 * (EMA_CO_S.Result[index] / EMA_HL_S.Result[index]);

            if (Line)
            {
                ECO_Line[index] = ECO;
            }
            else
            {
                if (ECO > 0)
                {
                    ECO_Hist_Up[index] = ECO;
                }
                else
                {
                    ECO_Hist_Dn[index] = ECO;
                }

            }


        }
    }
}
