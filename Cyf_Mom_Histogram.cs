using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Levels(0)]
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Cyf_Mom_Histogram : Indicator
    {
        //////// Drawing on the Indicator ///////
        private string UP = "⤒";
        private string Down = "⤓";
        private VerticalAlignment vAlign = VerticalAlignment.Center;
        private HorizontalAlignment hAlign = HorizontalAlignment.Center;
        ////////////////////////////////////////

        private MomentumOscillator _momentum;
        

        [Parameter(DefaultValue = 12)]
        public int Periods { get; set; }


        protected override void Initialize()
        {

            _momentum = Indicators.MomentumOscillator(MarketSeries.Close, Periods);


        }
        public override void Calculate(int index)
        {
            _momentum.Result[index] = double.NaN;
            double momentum = _momentum.Result[index];

            if (_momentum.Result[index] >= 100.0)
            {
                // moScillatorUp[index] = (_momentum.Result[index] - 100.0);

                ChartObjects.DrawText("UpArrows" + index, UP, index, MarketSeries.Low[index] - 10 * (Symbol.PipSize), vAlign, hAlign, Colors.Green);
              

            }
            else
            {
                ChartObjects.DrawText("DnArrows" + index, Down, index, MarketSeries.High[index] + 10 * (Symbol.PipSize), vAlign, hAlign, Colors.Red);

            }
        }


    }
}
