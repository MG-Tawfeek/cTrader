using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{

    [Indicator("ADX+", IsOverlay = false, TimeZone = TimeZones.UTC, ScalePrecision = 0, AutoRescale = true, AccessRights = AccessRights.None)]
    [Levels(20)]
    public class Cyf_ADX_P : Indicator
    {

        private string Bullet = "◾";
        private VerticalAlignment vAlign = VerticalAlignment.Center;
        private HorizontalAlignment hAlign = HorizontalAlignment.Center;



        public int Draw_String { get; set; }
        private DirectionalMovementSystem _dms;
        private SimpleMovingAverage DI_Plus;
        private SimpleMovingAverage DI_Minus;


        [Parameter(DefaultValue = 14)]
        public int ADX_Periods { get; set; }

        [Parameter(DefaultValue = 20)]
        public int ADX_Threshold { get; set; }




        [Output("DI_Plus", Color = Colors.Cyan, PlotType = PlotType.Histogram, Thickness = 1)]
        public IndicatorDataSeries buy { get; set; }

        [Output("DI_Minus", Color = Colors.Red, PlotType = PlotType.Histogram, Thickness = 1)]
        public IndicatorDataSeries sell { get; set; }

        [Output("ADX", Color = Colors.Yellow, PlotType = PlotType.Line, Thickness = 2)]
        public IndicatorDataSeries ADX { get; set; }



        protected override void Initialize()
        {


            _dms = Indicators.DirectionalMovementSystem(ADX_Periods);
            DI_Plus = Indicators.SimpleMovingAverage(buy, ADX_Periods);
            DI_Minus = Indicators.SimpleMovingAverage(sell, ADX_Periods);
        }
        public override void Calculate(int index)
        {
            ADX[index] = (int)_dms.ADX.LastValue;
            buy[index] = (int)_dms.DIPlus[index];
            sell[index] = (int)_dms.DIMinus[index];

            if (_dms.ADX.Last(0) > ADX_Threshold)
            {
                ChartObjects.DrawText("Signal" + index, Bullet, index, 0.5, vAlign, hAlign, Colors.GreenYellow);
            }
            else
            {
                ChartObjects.DrawText("Signal" + index, Bullet, index, 0.5, vAlign, hAlign, Colors.Green);
            }
        }
    }

}
