using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, ScalePrecision = 4, AccessRights = AccessRights.None)]
    public class Cyf_Thermometer : Indicator
    {

        private ExponentialMovingAverage EMA;
        private IndicatorDataSeries Temperature;


        [Parameter("Periods", DefaultValue = 22)]
        public int Periods { get; set; }

        [Parameter("Idle_Threshold", DefaultValue = 3)]
        public int Idle_Threshold { get; set; }

        [Parameter("Exaustion_Threshold", DefaultValue = 3)]
        public int Exaustion_Threshold { get; set; }

        [Output("Temp1", PlotType = PlotType.Histogram, Thickness = 3, Color = Colors.Yellow)]
        public IndicatorDataSeries Temp1 { get; set; }

        [Output("Temp2", PlotType = PlotType.Histogram, Thickness = 3, Color = Colors.DodgerBlue)]
        public IndicatorDataSeries Temp2 { get; set; }

        [Output("Temp3", PlotType = PlotType.Histogram, Thickness = 3, Color = Colors.DarkOrange)]
        public IndicatorDataSeries Temp3 { get; set; }

        [Output("Temp4", PlotType = PlotType.Histogram, Thickness = 3, Color = Colors.Red)]
        public IndicatorDataSeries Temp4 { get; set; }

        [Output("MA", Color = Colors.White)]
        public IndicatorDataSeries MA { get; set; }


        protected override void Initialize()
        {
            Temperature = CreateDataSeries();
            EMA = Indicators.ExponentialMovingAverage(Temperature, Periods);
        }

        public override void Calculate(int index)
        {
            Temperature[index] = 0;
            //TemperatureUp[index] = 0;
            //TemperatureDn[index] = 0;

            var Highs = Math.Abs(MarketSeries.High[index] - MarketSeries.High[index - 1]);
            var Lows = Math.Abs(MarketSeries.Low[index - 1] - MarketSeries.Low[index]);
            if (Highs > Lows)
            {
                Temperature[index] = Highs;
                //TemperatureUp[index] = Highs;
            }
            else
            {
                Temperature[index] = Lows;
                //TemperatureDn[index] = Lows;
            }

            if (Temperature[index] < EMA.Result[index])
            {
                if (EMA.Result[index] / Temperature[index] >= Idle_Threshold)
                {
                    Temp1[index] = Temperature[index];
                }
                else
                {
                    Temp2[index] = Temperature[index];
                }
            }
            else if (Temperature[index] > EMA.Result[index])
            {
                if (Temperature[index] / EMA.Result[index] >= Exaustion_Threshold)
                {
                    Temp4[index] = Temperature[index];
                }
                else
                {
                    Temp3[index] = Temperature[index];
                }

            }
            MA[index] = EMA.Result[index];

        }
    }
}
