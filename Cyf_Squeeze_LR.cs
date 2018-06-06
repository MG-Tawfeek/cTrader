using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;


namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Cyf_Squeeze_LR : Indicator
    {
        private string Bullet = "●●";
        private string Squeeze = "●";
        private string UP = "▲";
        private string Down = "▼";
        private const VerticalAlignment vAlign = VerticalAlignment.Center;
        private const HorizontalAlignment hAlign = HorizontalAlignment.Center;
        private bool Squeeze_State;
        //private System.Windows.Forms.Form form1;
        /// Keltner channel Stuff ///

        /// Momentum Stuff ///////////
        private MomentumOscillator _momentum;
        private LinearRegressionSlope LR_Slope;
        private MacdHistogram MacD;

        private MarketSeries MacD_Series;

        [Parameter(DefaultValue = true)]
        public bool LR_Based { get; set; }

        //{ get; set; }
        [Parameter(DefaultValue = 12)]
        public int Periods { get; set; }

        [Parameter("MacD_TF")]
        public TimeFrame MacD_TF { get; set; }

        [Parameter(DefaultValue = true)]
        public bool SigOrHist { get; set; }

        [Parameter("LongCycle", DefaultValue = 26)]
        public int LngCycle { get; set; }

        [Parameter("ShortCycle", DefaultValue = 12)]
        public int ShrtCycle { get; set; }

        [Parameter("SigPeriod", DefaultValue = 9)]
        public int SigPeriod { get; set; }

        [Parameter("MCD_Factor", DefaultValue = 0.1, MaxValue = 1, MinValue = 0.01, Step = 0.01)]
        public double Mac_factor { get; set; }

        [Output("Mac_D", Color = Colors.Red, PlotType = PlotType.Histogram, Thickness = 1)]
        public IndicatorDataSeries Mac_D { get; set; }

        [Output("LR_Down", Color = Colors.Red, PlotType = PlotType.Histogram, Thickness = 3)]
        public IndicatorDataSeries moScillatorDown { get; set; }

        [Output("LR_Up", Color = Colors.Blue, PlotType = PlotType.Histogram, Thickness = 3)]
        public IndicatorDataSeries moScillatorUp { get; set; }


        [Output("LR_Down2", Color = Colors.LightPink, PlotType = PlotType.Histogram, Thickness = 3)]
        public IndicatorDataSeries moScillatorDown2 { get; set; }

        [Output("LR_Up2", Color = Colors.CornflowerBlue, PlotType = PlotType.Histogram, Thickness = 3)]
        public IndicatorDataSeries moScillatorUp2 { get; set; }

        /////////////////////////////////

        private KeltnerChannels Klt;
        private BollingerBands bb;
        //// should be removed in Final Version 

        //[Output("1", Color = Colors.Blue, PlotType = PlotType.Line, Thickness = 0)]
        //public IndicatorDataSeries klt_main { get; set; }

        //[Output("2", Color = Colors.Blue, PlotType = PlotType.Line, Thickness = 2)]
        //public IndicatorDataSeries klt_top { get; set; }

        //[Output("3", Color = Colors.Blue, PlotType = PlotType.Line, Thickness = 2)]
        //public IndicatorDataSeries klt_bottom { get; set; }

        ///////////////////////////////////

        /// Bollinger Band Stuff ///


        [Parameter("Source")]
        public DataSeries bb_Source { get; set; }


        //public IndicatorDataSeries bb_main { get; set; }
        //[Output("4", Color = Colors.Red, PlotType = PlotType.Line, Thickness = 2)]
        //public IndicatorDataSeries bb_top { get; set; }

        //[Output("5", Color = Colors.Red, PlotType = PlotType.Line, Thickness = 2)]
        //public IndicatorDataSeries bb_bottom { get; set; }

        //public IndicatorDataSeries bb_bottom;

        //////////////////////////////////



        protected override void Initialize()
        {
            MacD_Series = MarketData.GetSeries(MacD_TF);
            // Initialize keltner Channels 
            LR_Slope = Indicators.LinearRegressionSlope(MarketSeries.Close, Periods);
            _momentum = Indicators.MomentumOscillator(MarketSeries.Close, Periods);
            MacD = Indicators.MacdHistogram(MacD_Series.Close, LngCycle, ShrtCycle, SigPeriod);

            Klt = Indicators.KeltnerChannels(20, MovingAverageType.Exponential, 10, MovingAverageType.Simple, 1.5);
            bb = Indicators.BollingerBands(bb_Source, 20, 2.0, MovingAverageType.Simple);
            //form1 = new System.Windows.Forms.Form();

        }

        public override void Calculate(int index)
        {

            // Calculate value at specified index
            // Result[index] = ...
            //main[index] = Klt.Main[index];
            if (SigOrHist)
            {
                Mac_D[index] = MacD.Signal[index] * Mac_factor;
            }
            else
            {
                Mac_D[index] = MacD.Histogram[index] * Mac_factor;
            }

            if (LR_Based == true)
            {
                LR_Slope.Result[index] = _momentum.Result[index];

                RefreshData();


                //moScillatorUp[index] = _momentum.Result[index];

                //double momentum = LR_Slope.Result[index];
                if (LR_Slope.Result[index] >= 0.0)
                {
                    if (LR_Slope.Result.IsRising())
                    {
                        moScillatorUp[index] = LR_Slope.Result[index];
                    }
                    else
                    {
                        moScillatorUp2[index] = LR_Slope.Result[index];
                    }
                    ChartObjects.RemoveObject("Direction");
                    ChartObjects.DrawText("Direction", UP, StaticPosition.BottomRight, Colors.Green);
                    ChartObjects.DrawText("Direction1", Bullet, index, -1.2 * moScillatorUp[index], vAlign, hAlign, Colors.Green);

                }
                else
                {
                    if (LR_Slope.Result.IsFalling())
                    {
                        moScillatorDown[index] = (LR_Slope.Result[index]);
                    }

                    else
                    {
                        moScillatorDown2[index] = (LR_Slope.Result[index]);
                    }

                    ChartObjects.RemoveObject("Direction");
                    ChartObjects.DrawText("Direction", Down, StaticPosition.BottomRight, Colors.Red);
                    ChartObjects.DrawText("Direction1", Bullet, index, 1.2 * moScillatorDown[index], vAlign, hAlign, Colors.Red);

                }
            }
            else
            {
                if (_momentum.Result[index] >= 100.0)
                {
                    if (_momentum.Result.IsRising())
                    {
                        moScillatorUp[index] = (_momentum.Result[index] - 100.0);

                    }
                    else
                    {
                        moScillatorUp2[index] = (_momentum.Result[index] - 100.0);
                    }
                    moScillatorUp[index] = (_momentum.Result[index] - 100.0);
                    ChartObjects.RemoveObject("Direction");
                    ChartObjects.DrawText("Direction", UP, StaticPosition.BottomRight, Colors.Green);
                    //ChartObjects.DrawText("Direction1", Bullet, index, -1.2 * moScillatorUp[index], vAlign, hAlign, Colors.Green);

                }
                else
                {
                    if (_momentum.Result.IsFalling())
                    {
                        moScillatorDown[index] = (_momentum.Result[index] - 100.0);
                    }
                    else
                    {
                        moScillatorDown2[index] = (_momentum.Result[index] - 100.0);
                    }

                    moScillatorDown[index] = (_momentum.Result[index] - 100.0);
                    ChartObjects.RemoveObject("Direction");
                    ChartObjects.DrawText("Direction", Down, StaticPosition.BottomRight, Colors.Red);
                    // ChartObjects.DrawText("Direction1", Bullet, index, 1.2 * moScillatorDown[index], vAlign, hAlign, Colors.Red);

                }




            }

            /// Keltner & Bollinger Calculations
            /// Seems like they need a separate Thread ? 
            //klt_main[index] = Klt.Main[index];
            //klt_top[index] = Klt.Top[index];
            //klt_bottom[index] = Klt.Bottom[index];

            //bb_main[index] = bb.Main[index];
            //bb_top[index] = bb.Top[index];
            //bb_bottom[index] = bb.Bottom[index];

            if (bb.Top[index] < Klt.Top[index] && bb.Bottom[index] > Klt.Bottom[index])
            {
                Squeeze_State = true;
                //ChartObjects.DrawText("Inside", Squeeze, StaticPosition.BottomCenter, Colors.Black);
                ChartObjects.DrawText("Direction1" + index, Squeeze, index, 0, vAlign, hAlign, Colors.Red);
            }
            else if (Squeeze_State)
            {
                ChartObjects.DrawText("Direction1" + index, Squeeze, index, 0, vAlign, hAlign, Colors.Green);
                //ChartObjects.RemoveObject("Inside");
                //Timer.Stop();
                Squeeze_State = false;
                Notifications.PlaySound("C:\\Sounds\\Bike Horn.wav");
                //form1.ShowDialog();
            }


        }

        //////////////////////////////////////////////////


        protected override void OnTimer()
        {
            //base.OnTimer();
            //ChartObjects.DrawText("Inside", Squeeze, StaticPosition.BottomCenter, Colors.Red);
            ChartObjects.RemoveObject("Inside");
        }
    }

    ///
}
