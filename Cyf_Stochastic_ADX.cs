using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, ScalePrecision = 0, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Cyf_Stochastic_ADX : Indicator
    {
        private DirectionalMovementSystem dms;
        private StochasticOscillator stoc;

        [Parameter(DefaultValue = 14)]
        public int ADX_Periods { get; set; }

        [Parameter("K_Periods", DefaultValue = 8, MinValue = 1)]
        public int K_Period { get; set; }

        [Parameter("Slow_K", DefaultValue = 3, MinValue = 2)]
        public int Slow_K { get; set; }

        [Parameter("D_Period", DefaultValue = 3, MinValue = 0)]
        public int D_Period { get; set; }

        [Parameter("MA Type", DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType MAType { get; set; }

        [Output("ADX", Color = Colors.Gray, PlotType = PlotType.Histogram, Thickness = 3)]
        public IndicatorDataSeries ADX { get; set; }

        [Output("%D", Color = Colors.Blue, PlotType = PlotType.Line, LineStyle = LineStyle.Lines)]
        public IndicatorDataSeries Percent_D { get; set; }

        [Output("%K", Color = Colors.Red)]
        public IndicatorDataSeries Percent_K { get; set; }


        protected override void Initialize()
        {
            // Initialize and create nested indicators
            dms = Indicators.DirectionalMovementSystem(ADX_Periods);
            stoc = Indicators.StochasticOscillator(K_Period, Slow_K, D_Period, MAType);
        }

        public override void Calculate(int index)
        {
            // Calculate value at specified index
            // Result[index] = ...
            ADX[index] = dms.ADX.LastValue;
            Percent_K[index] = stoc.PercentK.LastValue;
            Percent_D[index] = stoc.PercentD.LastValue;

        }
    }
}
