using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;
/// download the CSV File by adding a day to the current date "http://www.dailyfx.com/files/Calendar-22-10-2016.csv"
namespace cAlgo
{
    [Indicator(IsOverlay = false, ScalePrecision = 0, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Cyf_Impulse : Indicator
    {
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
            "◓",
            "▌"
        };

        private string getTimeFrame(MarketSeries theTime)
        {
            switch (theTime.TimeFrame.ToString())
            {
                case "Minute":
                    return "m1";
                case "Minute2":
                    return "m2";
                case "Minute3":
                    return "m3";
                case "Minute4":
                    return "m4";
                case "Minute5":
                    return "m5";
                case "Minute6":
                    return "m6";
                case "Minute7":
                    return "m7";
                case "Minute8":
                    return "m8";
                case "Minute9":
                    return "m9";
                case "Minute10":
                    return "m10";
                case "Minute15":
                    return "m15";
                case "Minute20":
                    return "m20";
                case "Minute30":
                    return "m30";
                case "Minute45":
                    return "m45";
                case "Hour":
                    return "H1";
                case "Hour2":
                    return "H2";
                case "Hour3":
                    return "H3";
                case "Hour4":
                    return "H4";
                case "Hour6":
                    return "H6";
                case "Hour8":
                    return "H8";
                case "Hour12":
                    return "H12";
                case "Daily":
                    return "D1";
                case "Day2":
                    return "D2";
                case "Day3":
                    return "D3";
                case "Weekly":
                    return "W1";
                case "Monthly":
                    return "M1";
                default:
                    return " ";
            }

        }
        private string Bullet = "◼";
        private VerticalAlignment vAlign = VerticalAlignment.Center;
        private HorizontalAlignment hAlign = HorizontalAlignment.Center;

        private Colors UpColor = Colors.DeepSkyBlue;
        private Colors DnColor = Colors.SeaGreen;

        /////////////////////////////////////////
        private MacdHistogram Mac;
        private MacdHistogram Mac_veryLongTF;
        private MacdHistogram Mac_LongTF;
        private MacdHistogram Mac_MedTF;
        private MacdHistogram Mac_SmallTF;

        private ExponentialMovingAverage EMA;
        private ExponentialMovingAverage EMA_veryLongTF;
        private ExponentialMovingAverage EMA_LongTF;
        private ExponentialMovingAverage EMA_MedTF;
        private ExponentialMovingAverage EMA_SmallTF;

        private MarketSeries veryLongTF_Series;
        private MarketSeries LongTF_Series;
        private MarketSeries MedTF_Series;
        private MarketSeries SmallTF_Series;

        /////////////////////////////////////////
        [Parameter("Shape", DefaultValue = 16, MinValue = 0, MaxValue = 16)]
        public int Draw_String { get; set; }

        [Parameter("UpColor", DefaultValue = "DeepSkyBlue")]
        public string upColor { get; set; }

        [Parameter("DnColor", DefaultValue = "SeaGreen")]
        public string dnColor { get; set; }

        ///////****************************************
        [Parameter("Num TF", DefaultValue = 3, MaxValue = 5, MinValue = 1)]
        public int Num_TF { get; set; }

        [Parameter("vLong_TF", DefaultValue = "Daily")]
        public TimeFrame vRef_TF { get; set; }

        [Parameter("Long_TF", DefaultValue = "Hour4")]
        public TimeFrame Ref_TF { get; set; }

        [Parameter("Med_TF", DefaultValue = "Hour")]
        public TimeFrame Med_TF { get; set; }

        [Parameter("Small_TF", DefaultValue = "Minute15")]
        public TimeFrame Lil_TF { get; set; }
        ///////*****************************************
        [Parameter(DefaultValue = 13)]
        public int Period { get; set; }

        [Parameter("LongCycle", DefaultValue = 26)]
        public int LongCycle { get; set; }

        [Parameter("ShrtCycle", DefaultValue = 12)]
        public int ShrtCycle { get; set; }

        [Parameter("Signal", DefaultValue = 9)]
        public int Signal { get; set; }


        protected override void Initialize()
        {
            Bullet = theBullet[Draw_String];
            Enum.TryParse(upColor, out UpColor);
            Enum.TryParse(dnColor, out DnColor);


            ///////*******************************************************
            Mac = Indicators.MacdHistogram(MarketSeries.Close, LongCycle, ShrtCycle, Signal);
            EMA = Indicators.ExponentialMovingAverage(MarketSeries.Close, Period);

            if (Num_TF > 4)
            {
                veryLongTF_Series = MarketData.GetSeries(vRef_TF);
                Mac_veryLongTF = Indicators.MacdHistogram(veryLongTF_Series.Close, LongCycle, ShrtCycle, Signal);
                EMA_veryLongTF = Indicators.ExponentialMovingAverage(veryLongTF_Series.Close, Period);
            }

            if (Num_TF > 3)
            {
                LongTF_Series = MarketData.GetSeries(Ref_TF);
                Mac_LongTF = Indicators.MacdHistogram(LongTF_Series.Close, LongCycle, ShrtCycle, Signal);
                EMA_LongTF = Indicators.ExponentialMovingAverage(LongTF_Series.Close, Period);
            }

            if (Num_TF > 2)
            {
                MedTF_Series = MarketData.GetSeries(Med_TF);
                Mac_MedTF = Indicators.MacdHistogram(MedTF_Series.Close, LongCycle, ShrtCycle, Signal);
                EMA_MedTF = Indicators.ExponentialMovingAverage(MedTF_Series.Close, Period);
            }

            if (Num_TF > 1)
            {
                SmallTF_Series = MarketData.GetSeries(Lil_TF);
                Mac_SmallTF = Indicators.MacdHistogram(SmallTF_Series.Close, LongCycle, ShrtCycle, Signal);
                EMA_SmallTF = Indicators.ExponentialMovingAverage(SmallTF_Series.Close, Period);
            }





            ////////////***********************************************

        }

        public override void Calculate(int index)
        {
            var indexVeryLongTF = 0;
            var indexLongTF = 0;
            var indexMedTF = 0;
            var indexSmallTF = 0;

            if (Num_TF > 4)
                indexVeryLongTF = veryLongTF_Series.OpenTime.GetIndexByTime(MarketSeries.OpenTime.LastValue);
            if (Num_TF > 3)
                indexLongTF = LongTF_Series.OpenTime.GetIndexByTime(MarketSeries.OpenTime.LastValue);
            if (Num_TF > 2)
                indexMedTF = MedTF_Series.OpenTime.GetIndexByTime(MarketSeries.OpenTime.LastValue);
            if (Num_TF > 1)
                indexSmallTF = SmallTF_Series.OpenTime.GetIndexByTime(MarketSeries.OpenTime.LastValue);

            ////////////////
            if (Num_TF > 4)
                ChartObjects.DrawText("verylong_TF", getTimeFrame(veryLongTF_Series), index + 5, 0.0, VerticalAlignment.Center, HorizontalAlignment.Center, Colors.Gray);
            if (Num_TF > 3)
                ChartObjects.DrawText("long_TF", getTimeFrame(LongTF_Series), index + 5, 0.2, VerticalAlignment.Center, HorizontalAlignment.Center, Colors.Gray);
            if (Num_TF > 2)
                ChartObjects.DrawText("Med_TF", getTimeFrame(MedTF_Series), index + 5, 0.4, VerticalAlignment.Center, HorizontalAlignment.Center, Colors.Gray);
            if (Num_TF > 1)
                ChartObjects.DrawText("small_TF", getTimeFrame(SmallTF_Series), index + 5, 0.6, VerticalAlignment.Center, HorizontalAlignment.Center, Colors.Gray);
            ChartObjects.DrawText("Cur_TF", getTimeFrame(MarketSeries), index + 5, 0.8, VerticalAlignment.Center, HorizontalAlignment.Center, Colors.Gray);
            //////////////////
            if (EMA.Result[index] > EMA.Result[index - 1] && Mac.Histogram[index] > Mac.Histogram[index - 1])
            {
                ChartObjects.DrawText("EMA_Dots" + index, Bullet, index, 0.8, vAlign, hAlign, UpColor);

            }

            else if (EMA.Result[index] < EMA.Result[index - 1] && Mac.Histogram[index] < Mac.Histogram[index - 1])
            {
                ChartObjects.DrawText("EMA_Dots" + index, Bullet, index, 0.8, vAlign, hAlign, DnColor);

            }
            else
            {
                ChartObjects.DrawText("EMA_Dots" + index, Bullet, index, 0.8, vAlign, hAlign, Colors.Black);


            }
            /////////********************************Very Long Series **************
            if (Num_TF > 4)
            {
                if (EMA_veryLongTF.Result[indexVeryLongTF] > EMA_veryLongTF.Result[indexVeryLongTF - 1] && Mac_veryLongTF.Histogram[indexVeryLongTF] > Mac_veryLongTF.Histogram[indexVeryLongTF - 1])
                {
                    ChartObjects.DrawText("veryLng_MAC_EMA" + index, Bullet, index, 0.0, vAlign, hAlign, UpColor);

                }

                else if (EMA_veryLongTF.Result[indexVeryLongTF] < EMA_veryLongTF.Result[indexVeryLongTF - 1] && Mac_veryLongTF.Histogram[indexVeryLongTF] < Mac_veryLongTF.Histogram[indexVeryLongTF - 1])
                {
                    ChartObjects.DrawText("veryLng_MAC_EMA" + index, Bullet, index, 0.0, vAlign, hAlign, DnColor);

                }
                else
                {
                    ChartObjects.DrawText("veryLng_MAC_EMA" + index, Bullet, index, 0.0, vAlign, hAlign, Colors.Black);
                }
            }
            /////////********************************Long Series **************
            if (Num_TF > 3)
            {
                if (EMA_LongTF.Result[indexLongTF] > EMA_LongTF.Result[indexLongTF - 1] && Mac_LongTF.Histogram[indexLongTF] > Mac_LongTF.Histogram[indexLongTF - 1])
                {
                    ChartObjects.DrawText("Lng_MAC_EMA" + index, Bullet, index, 0.2, vAlign, hAlign, UpColor);

                }

                else if (EMA_LongTF.Result[indexLongTF] < EMA_LongTF.Result[indexLongTF - 1] && Mac_LongTF.Histogram[indexLongTF] < Mac_LongTF.Histogram[indexLongTF - 1])
                {
                    ChartObjects.DrawText("Lng_MAC_EMA" + index, Bullet, index, 0.2, vAlign, hAlign, DnColor);

                }
                else
                {
                    ChartObjects.DrawText("Lng_MAC_EMA" + index, Bullet, index, 0.2, vAlign, hAlign, Colors.Black);
                }
            }
            /////////********************************Med Series **************
            if (Num_TF > 2)
            {
                if (EMA_MedTF.Result[indexMedTF] > EMA_MedTF.Result[indexMedTF - 1] && Mac_MedTF.Histogram[indexMedTF] > Mac_MedTF.Histogram[indexMedTF - 1])
                {
                    ChartObjects.DrawText("Med_MAC_EMA" + index, Bullet, index, 0.4, vAlign, hAlign, UpColor);

                }

                else if (EMA_MedTF.Result[indexMedTF] < EMA_MedTF.Result[indexMedTF - 1] && Mac_MedTF.Histogram[indexMedTF] < Mac_MedTF.Histogram[indexMedTF - 1])
                {
                    ChartObjects.DrawText("Med_MAC_EMA" + index, Bullet, index, 0.4, vAlign, hAlign, DnColor);

                }
                else
                {
                    ChartObjects.DrawText("Med_MAC_EMA" + index, Bullet, index, 0.4, vAlign, hAlign, Colors.Black);
                }
            }
            /////////********************************Small Series**************
            if (Num_TF > 1)
            {
                if (EMA_SmallTF.Result[indexSmallTF] > EMA_SmallTF.Result[indexSmallTF - 1] && Mac_SmallTF.Histogram[indexSmallTF] > Mac_SmallTF.Histogram[indexSmallTF - 1])
                {
                    ChartObjects.DrawText("small_MAC_EMA" + index, Bullet, index, 0.6, vAlign, hAlign, UpColor);

                }

                else if (EMA_SmallTF.Result[indexSmallTF] < EMA_SmallTF.Result[indexSmallTF - 1] && Mac_SmallTF.Histogram[indexSmallTF] < Mac_SmallTF.Histogram[indexSmallTF - 1])
                {
                    ChartObjects.DrawText("small_MAC_EMA" + index, Bullet, index, 0.6, vAlign, hAlign, DnColor);

                }
                else
                {
                    ChartObjects.DrawText("small_MAC_EMA" + index, Bullet, index, 0.6, vAlign, hAlign, Colors.Black);
                }

            }


        }
    }
}
