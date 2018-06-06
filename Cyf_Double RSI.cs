using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{

    [Indicator("DoubleRSI", ScalePrecision = 0, IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class DoubleRSI : Indicator
    {
        private RelativeStrengthIndex _rsi;
        private RelativeStrengthIndex _rsi2;
        private DirectionalMovementSystem _dms;


        // ADX Histogram 
        [Parameter(DefaultValue = 14)]
        public int ADX_Periods { get; set; }

        [Parameter(DefaultValue = 20)]
        public int ADX_Threshold { get; set; }

        [Parameter()]
        public DataSeries Source { get; set; }


        [Parameter(DefaultValue = 5)]
        public int FastRSI { get; set; }

        [Parameter(DefaultValue = 10)]
        public int SlowRSI { get; set; }


        //Colors
        [Output("ADX", Color = Colors.Black, PlotType = PlotType.Histogram, Thickness = 3)]
        public IndicatorDataSeries ADX { get; set; }

        [Output("Thrust", Color = Colors.DimGray, PlotType = PlotType.Histogram, Thickness = 3)]
        public IndicatorDataSeries ADX_Thrust { get; set; }



        [Output("FastRSI", Color = Colors.LightGreen)]
        public IndicatorDataSeries Result { get; set; }


        [Output("SlowRSI", Color = Colors.Blue)]
        public IndicatorDataSeries Result2 { get; set; }


        protected override void Initialize()
        {

            _dms = Indicators.DirectionalMovementSystem(ADX_Periods);
            _rsi = Indicators.RelativeStrengthIndex(Source, FastRSI);
            _rsi2 = Indicators.RelativeStrengthIndex(Source, SlowRSI);

        }

        public override void Calculate(int index)
        {
            if ((int)_dms.ADX.LastValue >= 0 && ((int)_dms.ADX.LastValue < ADX_Threshold))
            {
                ADX[index] = (int)_dms.ADX.LastValue;
            }

            else if ((int)_dms.ADX.LastValue >= ADX_Threshold)
            {
                ADX_Thrust[index] = (int)_dms.ADX.LastValue;
            }


            Result[index] = (int)_rsi.Result[index];
            Result2[index] = (int)_rsi2.Result[index];

        }


    }
}
