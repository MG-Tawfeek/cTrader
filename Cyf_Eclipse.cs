using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, ScalePrecision = 0, AccessRights = AccessRights.None)]
    public class Cyf_Eclipse_T7_2 : Indicator
    {
       
        private string UP = "▲";
        private string Down = "▼";

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
        //private string Bullet = "◾";
        private VerticalAlignment vAlign = VerticalAlignment.Center;
        private HorizontalAlignment hAlign = HorizontalAlignment.Center;

        private bool LongTF_IsUp;
        private bool MedTF_IsUp;
        private bool SmallTF_IsUp;

        ////////////////TVI Start///////////////
        private IndicatorDataSeries UpTick;
        private IndicatorDataSeries DnTick;
        private IndicatorDataSeries TVI_Calculate;

        private ExponentialMovingAverage EMA_UpTick;
        private ExponentialMovingAverage EMA_DnTick;

        private ExponentialMovingAverage DEMA_UpTick;
        private ExponentialMovingAverage DEMA_DnTick;

        private ExponentialMovingAverage TVI;
        ////////////////TVI End ////////////////

        private ExponentialMovingAverage MA50;
        private ExponentialMovingAverage MA50_LongTF;
        private ExponentialMovingAverage MA50_MedTF;
        private ExponentialMovingAverage MA50_SmallTF;


        private MarketSeries LongTF_Series;
        private MarketSeries MedTF_Series;
        private MarketSeries SmallTF_Series;

        [Parameter("Play Sound", DefaultValue = true)]
        public bool play_Sound { get; set; }

        [Parameter(DefaultValue = "C:\\Sounds\\Sonar_3.mp3")]
        public string SoundFile { get; set; }

        [Parameter("Draw", DefaultValue = true)]
        public bool Draw { get; set; }

        [Parameter("Shape", DefaultValue = 3, MinValue = 0, MaxValue = 15)]
        public int Draw_String { get; set; }

        [Parameter("Long_TF", DefaultValue = "Daily")]
        public TimeFrame Ref_TF { get; set; }

        [Parameter("Med_TF", DefaultValue = "Hour4")]
        public TimeFrame Med_TF { get; set; }

        [Parameter("Small_TF", DefaultValue = "Hour")]
        public TimeFrame Lil_TF { get; set; }

        [Parameter("MA Period", DefaultValue = 50)]
        public int Ma_Periods { get; set; }

        ////////////TVI Params Start ////////////////
        [Parameter("TVI.r", DefaultValue = 12)]
        public int EMA { get; set; }

        [Parameter("TVI.s", DefaultValue = 12)]
        public int DEMA { get; set; }

        [Parameter("TVI.u", DefaultValue = 5)]
        public int TEMA { get; set; }

        [Output("TVI_Up", PlotType = PlotType.Histogram, Color = Colors.AliceBlue, Thickness = 3)]
        public IndicatorDataSeries TVI_Draw_Up { get; set; }

        [Output("TVI_Dn", PlotType = PlotType.Histogram, Color = Colors.DarkSlateGray, Thickness = 3)]
        public IndicatorDataSeries TVI_Draw_Dn { get; set; }
        ////////////TVI Params End //////////////////



        protected override void Initialize()
        {
            
            //////////////TVI Init Start //////////////
            UpTick = CreateDataSeries();
            DnTick = CreateDataSeries();
            TVI_Calculate = CreateDataSeries();

            EMA_UpTick = Indicators.ExponentialMovingAverage(UpTick, EMA);
            EMA_DnTick = Indicators.ExponentialMovingAverage(DnTick, EMA);

            DEMA_UpTick = Indicators.ExponentialMovingAverage(EMA_UpTick.Result, DEMA);
            DEMA_DnTick = Indicators.ExponentialMovingAverage(EMA_DnTick.Result, DEMA);

            TVI = Indicators.ExponentialMovingAverage(TVI_Calculate, TEMA);

            //////////////TVI init End ///////////////
            // Initialize and create nested indicators
            LongTF_Series = MarketData.GetSeries(Ref_TF);
            MedTF_Series = MarketData.GetSeries(Med_TF);
            SmallTF_Series = MarketData.GetSeries(Lil_TF);

            MA50 = Indicators.ExponentialMovingAverage(MarketSeries.Close, Ma_Periods);
            MA50_LongTF = Indicators.ExponentialMovingAverage(LongTF_Series.Close, Ma_Periods);
            MA50_MedTF = Indicators.ExponentialMovingAverage(MedTF_Series.Close, Ma_Periods);
            MA50_SmallTF = Indicators.ExponentialMovingAverage(SmallTF_Series.Close, Ma_Periods);
        }

        public override void Calculate(int index)
        {

            // ChartObjects.DrawText("D2_Trend", "\t\t\t\t" + Ref_TF + " " + Down, StaticPosition.BottomLeft, Colors.Orange);
            /////////////////// TVI Calculate START ////////////////////////

            UpTick[index] = (MarketSeries.TickVolume[index] + (MarketSeries.Close[index] - MarketSeries.Open[index]) / Symbol.TickSize) / 2;
            DnTick[index] = MarketSeries.TickVolume[index] - UpTick[index];

            TVI_Calculate[index] = 100 * ((DEMA_UpTick.Result[index] - DEMA_DnTick.Result[index]) / (DEMA_UpTick.Result[index] + DEMA_DnTick.Result[index]));

            if (TVI.Result[index] > TVI.Result[index - 1])
            {
                TVI_Draw_Up[index] = TVI.Result[index];
            }
            else
            {
                TVI_Draw_Dn[index] = TVI.Result[index];
            }
            /////////////////// TVI Calculate END //////////////////////////


            if (MarketSeries.Close.Last(0) < MA50_LongTF.Result.Last(1))
            {
                ChartObjects.RemoveObject("D_Trend");
                ChartObjects.DrawText("D_Trend", "\n" + Ref_TF + " " + Down, StaticPosition.BottomLeft, Colors.Red);
                LongTF_IsUp = false;
            }
            else
            {
                ChartObjects.RemoveObject("D_Trend");
                ChartObjects.DrawText("D_Trend", "\n" + Ref_TF + " " + UP, StaticPosition.BottomLeft, Colors.Green);
                LongTF_IsUp = true;

            }

            if (MarketSeries.Close.Last(0) < MA50_MedTF.Result.Last(1))
            {
                ChartObjects.RemoveObject("4H_Trend");
                ChartObjects.DrawText("4H_Trend", Med_TF + " " + Down, StaticPosition.BottomCenter, Colors.Red);
                MedTF_IsUp = false;
            }
            else
            {
                ChartObjects.RemoveObject("4H_Trend");
                ChartObjects.DrawText("4H_Trend", Med_TF + " " + UP, StaticPosition.BottomCenter, Colors.Green);
                MedTF_IsUp = true;

            }

            if (MarketSeries.Close.Last(0) < MA50_SmallTF.Result.Last(1))
            {
                ChartObjects.RemoveObject("1H_Trend");
                ChartObjects.DrawText("1H_Trend", Lil_TF + " " + Down, StaticPosition.BottomRight, Colors.Red);
                SmallTF_IsUp = false;
            }
            else
            {
                ChartObjects.RemoveObject("1H_Trend");
                ChartObjects.DrawText("1H_Trend", Lil_TF + " " + UP, StaticPosition.BottomRight, Colors.Green);
                SmallTF_IsUp = true;

            }
            // Calculate value at specified index
            // Result[index] = ...
            if (MarketSeries.Close[index] < MA50.Result[index] && LongTF_IsUp && MedTF_IsUp && SmallTF_IsUp)
            {
                if (Draw)
                {
                    ChartObjects.DrawText("BuySetup" + index, theBullet[Draw_String], index, 0, vAlign, hAlign, Colors.Green);
                }
                if (play_Sound)
                    Notifications.PlaySound(SoundFile);
            }
            else if (MarketSeries.Close[index] > MA50.Result[index] && !LongTF_IsUp && !MedTF_IsUp && !SmallTF_IsUp)
            {
                if (Draw)
                {
                    ChartObjects.DrawText("SellSetup" + index, theBullet[Draw_String], index, 0, vAlign, hAlign, Colors.Orange);
                }
                if (play_Sound)
                    Notifications.PlaySound(SoundFile);
            }
        }
    }
}
