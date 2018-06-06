using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None, ScalePrecision = 0)]
    public class Cyf_ECO_Squeeze : Indicator
    {
        /////////////////////////////
        private string[] theBullet = 
        {
            "◾",
            "◼",
            "•",
            "●",
            "♦",
            "★",
            "▣",
            "☗",
            "♠",
            "♟",
            "♞",
            "♝",
            "☤",
            "♫",
            "٭",
            "◓"
        };
        private string Squeeze = "●";
        private string UP = "▲";
        private string Down = "▼";
        private const cAlgo.API.VerticalAlignment vAlign = cAlgo.API.VerticalAlignment.Center;
        private const cAlgo.API.HorizontalAlignment hAlign = cAlgo.API.HorizontalAlignment.Center;
        private bool Squeeze_State;
        /////////////////////////////
        private ExponentialMovingAverage EMA_CO;
        private ExponentialMovingAverage EMA_CO_S;
        private ExponentialMovingAverage EMA_HL;
        private ExponentialMovingAverage EMA_HL_S;
        private IndicatorDataSeries CO_Series;
        private IndicatorDataSeries HL_Series;
        private double CO_Val;
        private double HL_Val;
        //private double ECO;
        private IndicatorDataSeries ECO;


        [Parameter("Shape", DefaultValue = 3, MinValue = 0, MaxValue = 15)]
        public int Draw_String { get; set; }

        [Parameter(DefaultValue = 32, MinValue = 1)]
        public int Interval_1 { get; set; }

        [Parameter(DefaultValue = 12, MinValue = 1)]
        public int Interval_2 { get; set; }

        [Parameter("(Y)Line/(N)Hist", DefaultValue = false)]
        public bool Line { get; set; }

        [Output("ECO_Line", PlotType = PlotType.Line)]
        public IndicatorDataSeries ECO_Line { get; set; }

        [Output("ECO_Hist_Up", Color = Colors.Yellow, PlotType = PlotType.Histogram, Thickness = 3)]
        public IndicatorDataSeries ECO_Hist_Up { get; set; }

        [Output("ECO_Hist_Up2", Color = Colors.Olive, PlotType = PlotType.Histogram, Thickness = 3)]
        public IndicatorDataSeries ECO_Hist_Up2 { get; set; }

        [Output("ECO_Hist_Dn", Color = Colors.SaddleBrown, PlotType = PlotType.Histogram, Thickness = 3)]
        public IndicatorDataSeries ECO_Hist_Dn { get; set; }

        [Output("ECO_Hist_Dn2", Color = Colors.SandyBrown, PlotType = PlotType.Histogram, Thickness = 3)]
        public IndicatorDataSeries ECO_Hist_Dn2 { get; set; }

        ///////////////////////////////////

        [Parameter(DefaultValue = 10)]
        public int Klt_ATR_Periods { get; set; }

        [Parameter(DefaultValue = 20)]
        public int Klt_Periods { get; set; }

        [Parameter(DefaultValue = 1.5)]
        public double Klt_stdDev { get; set; }

        [Parameter(DefaultValue = 20)]
        public int bb_Periods { get; set; }

        [Parameter(DefaultValue = 2.0)]
        public double bb_stdDev { get; set; }
        ///////////////////////////////////
        private KeltnerChannels Klt;
        private BollingerBands bb;

        [Parameter("Source")]
        public DataSeries bb_Source { get; set; }
        //////////////////////////////////

        protected override void Initialize()
        {
            Squeeze = theBullet[Draw_String];
            ECO = CreateDataSeries();
            CO_Series = CreateDataSeries();
            HL_Series = CreateDataSeries();

            EMA_CO = Indicators.ExponentialMovingAverage(CO_Series, Interval_1);
            EMA_CO_S = Indicators.ExponentialMovingAverage(EMA_CO.Result, Interval_2);

            EMA_HL = Indicators.ExponentialMovingAverage(HL_Series, Interval_1);
            EMA_HL_S = Indicators.ExponentialMovingAverage(EMA_HL.Result, Interval_2);

            Klt = Indicators.KeltnerChannels(Klt_Periods, MovingAverageType.Exponential, Klt_ATR_Periods, MovingAverageType.Simple, Klt_stdDev);
            bb = Indicators.BollingerBands(bb_Source, bb_Periods, bb_stdDev, MovingAverageType.Simple);

        }

        public override void Calculate(int index)
        {
            CO_Val = double.NaN;
            HL_Val = double.NaN;

            CO_Val = MarketSeries.Close[index] - MarketSeries.Open[index];
            HL_Val = MarketSeries.High[index] - MarketSeries.Low[index];

            CO_Series[index] = CO_Val;
            HL_Series[index] = HL_Val;

            ECO[index] = 100 * (EMA_CO_S.Result[index] / EMA_HL_S.Result[index]);

            if (Line)
            {
                ECO_Line[index] = ECO[index];
            }
            else
            {
                if (ECO[index] > 0)
                {
                    if (Functions.IsRising(ECO))
                    {
                        ECO_Hist_Up[index] = ECO[index];
                    }
                    else
                    {
                        ECO_Hist_Up2[index] = ECO[index];
                    }
                    ChartObjects.RemoveObject("Direction");
                    ChartObjects.DrawText("Direction", UP, StaticPosition.BottomRight, Colors.Green);
                }
                else
                {
                    if (Functions.IsFalling(ECO))
                    {
                        ECO_Hist_Dn[index] = ECO[index];
                    }
                    else
                    {
                        ECO_Hist_Dn2[index] = ECO[index];
                    }
                    ChartObjects.RemoveObject("Direction");
                    ChartObjects.DrawText("Direction", Down, StaticPosition.BottomRight, Colors.Red);
                }

            }

            if (bb.Top[index] < Klt.Top[index] && bb.Bottom[index] > Klt.Bottom[index])
            {
                Squeeze_State = true;

                ChartObjects.DrawText("Direction1" + index, Squeeze, index, 0, vAlign, hAlign, Colors.Red);
            }
            else if (Squeeze_State)
            {
                ChartObjects.DrawText("Direction1" + index, Squeeze, index, 0, vAlign, hAlign, Colors.Green);

                Squeeze_State = false;
                Notifications.PlaySound("C:\\Sounds\\Bike Horn.wav");
                //Notifications.SendEmail(fromAddress, toAdress, subject, text);


            }


        }
    }
}
