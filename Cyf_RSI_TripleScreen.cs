using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Levels(30, 70)]
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, ScalePrecision = 0, AccessRights = AccessRights.None)]
    public class Cyf_RSI_TripleScreen_T7_2 : Indicator
    {
       
        private RelativeStrengthIndex RSI;
        private ExponentialMovingAverage EMA;
        private MacdHistogram Mac;
        private MarketSeries LTF_Series;

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
        private string Bullet = "●";
        private const cAlgo.API.VerticalAlignment vAlign = cAlgo.API.VerticalAlignment.Center;
        private const cAlgo.API.HorizontalAlignment hAlign = cAlgo.API.HorizontalAlignment.Center;


        [Parameter(DefaultValue = true)]
        public bool SoundAlert { get; set; }

        [Parameter(DefaultValue = "C:\\Sounds\\Bike Horn.wav")]
        public string SoundFile { get; set; }

        [Parameter("Shape", DefaultValue = 3, MinValue = 0, MaxValue = 15)]
        public int Draw_String { get; set; }

        [Parameter("RSI_Periods", DefaultValue = 7)]
        public int Periods { get; set; }

        [Parameter("MA_Period", DefaultValue = 13)]
        public int MA_Period { get; set; }

        [Parameter("OverBought at", DefaultValue = 70)]
        public int OverBought { get; set; }

        [Parameter("OverSold at", DefaultValue = 30)]
        public int OverSold { get; set; }

        [Parameter("LTF", DefaultValue = "Daily")]
        public TimeFrame LTF { get; set; }

        [Output("Main")]
        public IndicatorDataSeries Result { get; set; }


        protected override void Initialize()
        {
            ExpiryDate = new DateTime(2017, 2, 7, 0, 0, 0);
            if (DateTime.Now > ExpiryDate)
            {
                ChartObjects.DrawText("TheText", Expired, StaticPosition.Center, Colors.Red);
                return;

            }
            Bullet = theBullet[Draw_String];

            LTF_Series = MarketData.GetSeries(LTF);
            RSI = Indicators.RelativeStrengthIndex(MarketSeries.Close, Periods);
            EMA = Indicators.ExponentialMovingAverage(LTF_Series.Close, MA_Period);
            Mac = Indicators.MacdHistogram(LTF_Series.Close, 26, 12, 9);
        }

        public override void Calculate(int index)
        {
            var indexDaily = LTF_Series.OpenTime.GetIndexByTime(MarketSeries.OpenTime.LastValue);
            Result[index] = RSI.Result[index];
           
            if (Mac.Histogram[indexDaily] > Mac.Histogram[indexDaily - 1] && EMA.Result[indexDaily] > EMA.Result[indexDaily - 1])
            {
                if (Result[index] < OverSold)
                {
                    ChartObjects.DrawText("Buy_Signal" + index, Bullet, index, 30, vAlign, hAlign, Colors.Yellow);
                    if (SoundAlert)
                    {
                        Notifications.PlaySound(SoundFile);

                    }
                }
            }


            if (Mac.Histogram.Last(0) < Mac.Histogram.Last(1) && EMA.Result.Last(0) < EMA.Result.Last(1))
            {
                if (Result[index] > OverBought)
                {
                    ChartObjects.DrawText("Sell_Signal" + index, Bullet, index, 70, vAlign, hAlign, Colors.Red);
                    if (SoundAlert)
                    {
                        Notifications.PlaySound(SoundFile);

                    }
                }
            }
            //////////////////////////////////

        }
    }
}
