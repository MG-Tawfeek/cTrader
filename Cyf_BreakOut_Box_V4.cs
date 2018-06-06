using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;
using System.Collections.Generic;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Cyf_BreakOut_Box_V4 : Indicator
    {

        DateTime ExpiryDate;
        string Expired = "E X P I R E D";
        private string theDot = "♦";

        public DateTime TheStart;
        public DateTime TheFinish;


        [Parameter("Session Start", DefaultValue = "7:00")]
        public string Cust_session_Start { get; set; }

        [Parameter("Duration(Hours)", MinValue = 1, DefaultValue = 14)]
        public int Session_Hours { get; set; }

        [Parameter("Pre Session", MinValue = 1, DefaultValue = 10)]
        public int Pre_Session { get; set; }

        [Parameter("Look Back", MinValue = 1, DefaultValue = 5)]
        public int Look_Back { get; set; }

        [Parameter("Buffer Pips", MinValue = 0, DefaultValue = 10)]
        public int Buffer_Pips { get; set; }

        [Parameter("P^L Levels", MinValue = 0, DefaultValue = 1)]
        public int PL_Levels { get; set; }

        [Parameter("TP1", MinValue = 1, DefaultValue = 15)]
        public double TP1 { get; set; }

        [Parameter("TP2", MinValue = 1, DefaultValue = 30)]
        public double TP2 { get; set; }

        [Parameter("TP3", MinValue = 1, DefaultValue = 45)]
        public double TP3 { get; set; }


        protected override void Initialize()
        {
            //+------------------------------------------------
            ExpiryDate = new DateTime(2018, 5, 18, 0, 0, 0);
            if (DateTime.Now > ExpiryDate)
            {
                ChartObjects.DrawText("TheText", Expired, StaticPosition.Center, Colors.Red);
                return;

            }
            //+------------------------------------------------

            TheStart = DateTime.Parse(Cust_session_Start);
            ChartObjects.RemoveAllObjects();

            //+--------------------------------------------------------------------------------+
        }


        public override void Calculate(int index)
        {


            var Idx_Lon_Start = MarketSeries.OpenTime.GetIndexByTime(TheStart);
            var Idx_Lon_End = Idx_Lon_Start + index_Projection(TimeFrame.ToString(), Session_Hours);
            

            var Idx_pre_session = Idx_Lon_Start - index_Projection(TimeFrame.ToString(), Pre_Session);
           
            //+------------------------------------------------
            //Code to Draw The Sessions Every Day
            //There should be a Look-Back Period to Limit the drawing and Calculations--> Done !
            //Should be Tested while the market is Open

            //+----------- Look Back Period -----------+
            if (MarketSeries.OpenTime[index] < TheStart.AddDays(-Look_Back))
                return;
            //+----------- Look Back Period -----------+

            
            double Lowest = 50000;
            double Highest = 0;
            double BoxHeight = 0;
            if (MarketSeries.OpenTime[index].Hour == TheStart.Hour && MarketSeries.OpenTime[index].Minute < TheStart.AddMinutes(1).Minute)
            {
             
                Lowest = 50000;
                Highest = 0;
                BoxHeight = 0;

                var diff = index_Projection(TimeFrame.ToString(), Pre_Session);
                for (int i = Idx_Lon_Start - diff; i <= Idx_Lon_Start; i++)
                {
                    if (MarketSeries.High[i] > Highest)
                        Highest = MarketSeries.High[i];

                    if (MarketSeries.Low[i] < Lowest)
                        Lowest = MarketSeries.Low[i];
                }
                BoxHeight = (Highest - Lowest);
               
                var TP1_Buy = (TP1 * Symbol.PipSize) + Highest + (Buffer_Pips * Symbol.PipSize);
                var TP2_Buy = (TP2 * Symbol.PipSize) + Highest + (Buffer_Pips * Symbol.PipSize);
                var TP3_Buy = (TP3 * Symbol.PipSize) + Highest + (Buffer_Pips * Symbol.PipSize);

                var TP1_Sell = Lowest - (Buffer_Pips * Symbol.PipSize) - (TP1 * Symbol.PipSize);
                var TP2_Sell = Lowest - (Buffer_Pips * Symbol.PipSize) - (TP2 * Symbol.PipSize);
                var TP3_Sell = Lowest - (Buffer_Pips * Symbol.PipSize) - (TP3 * Symbol.PipSize);

                if (Lowest == 50000)
                    return;
                ChartObjects.DrawText("Highest" + index, theDot, Idx_Lon_Start, Highest, VerticalAlignment.Center, HorizontalAlignment.Center, Colors.DarkTurquoise);
                ChartObjects.DrawText("Lowest" + index, theDot, Idx_Lon_Start, Lowest, VerticalAlignment.Center, HorizontalAlignment.Center, Colors.DarkTurquoise);

                DrawRectangle("London_Session" + index, Idx_Lon_Start, Lowest, Idx_Lon_End - Idx_Lon_Start, (Highest - Lowest), Colors.DodgerBlue, 1, LineStyle.LinesDots);
                DrawRectangle("Pre_Session Bounds" + index, Idx_pre_session, Lowest, Idx_Lon_Start - Idx_pre_session, Math.Abs(BoxHeight), Colors.Yellow, 1, LineStyle.DotsRare);
                //Print(index_Projection(TimeFrame.ToString()));

                //+--------- Draw Buy & Sell Breakout level ------------------------+

                ChartObjects.DrawLine("Buy_Level" + index, Idx_Lon_Start, Highest + (Buffer_Pips * Symbol.PipSize), Idx_Lon_Start + index_Projection(TimeFrame.ToString(), Session_Hours), Highest + (Buffer_Pips * Symbol.PipSize), Colors.LimeGreen, 1, LineStyle.DotsRare);
                ChartObjects.DrawText("Buy" + index, "Buy Level", index + 2, Highest + (Buffer_Pips * Symbol.PipSize), VerticalAlignment.Top, HorizontalAlignment.Right, Colors.LimeGreen);

                ChartObjects.DrawLine("Sell_Level" + index, Idx_Lon_Start, Lowest - (Buffer_Pips * Symbol.PipSize), Idx_Lon_Start + index_Projection(TimeFrame.ToString(), Session_Hours), Lowest - (Buffer_Pips * Symbol.PipSize), Colors.Crimson, 1, LineStyle.DotsRare);
                ChartObjects.DrawText("Sell" + index, "Sell Level", index + 2, Lowest - (Buffer_Pips * Symbol.PipSize), VerticalAlignment.Top, HorizontalAlignment.Right, Colors.Red);
                if (PL_Levels > 0)
                {
                    //+--------- Draw TP 1 level ------------------------+
                    ChartObjects.DrawLine("TP1 BUY" + index, Idx_Lon_Start, TP1_Buy, Idx_Lon_Start + index_Projection(TimeFrame.ToString(), Session_Hours), TP1_Buy, Colors.Green, 1, LineStyle.Lines);
                    ChartObjects.DrawText("TP1_Level_buy" + index, "bTP1", Idx_Lon_Start, TP1_Buy, VerticalAlignment.Top, HorizontalAlignment.Center, Colors.Green);

                    //+--------- Draw SL 1 level ------------------------+
                    ChartObjects.DrawLine("TP1 Sell" + index, Idx_Lon_Start, TP1_Sell, Idx_Lon_Start + index_Projection(TimeFrame.ToString(), Session_Hours), TP1_Sell, Colors.DarkOrange, 1, LineStyle.Lines);
                    ChartObjects.DrawText("TP1_Level_Sell" + index, "sTP1", Idx_Lon_Start, TP1_Sell, VerticalAlignment.Top, HorizontalAlignment.Center, Colors.DarkOrange);
                }
                if (PL_Levels > 1)
                {
                    //+--------- Draw TP 2 level ------------------------+
                    ChartObjects.DrawLine("TP2 BUY" + index, Idx_Lon_Start, TP2_Buy, Idx_Lon_Start + index_Projection(TimeFrame.ToString(), Session_Hours), TP2_Buy, Colors.Green, 1, LineStyle.Lines);
                    ChartObjects.DrawText("TP2_Level_buy" + index, "bTP2", Idx_Lon_Start, TP2_Buy, VerticalAlignment.Top, HorizontalAlignment.Center, Colors.Green);

                    //+--------- Draw SL 2 level ------------------------+
                    ChartObjects.DrawLine("TP2 Sell" + index, Idx_Lon_Start, TP2_Sell, Idx_Lon_Start + index_Projection(TimeFrame.ToString(), Session_Hours), TP2_Sell, Colors.DarkOrange, 1, LineStyle.Lines);
                    ChartObjects.DrawText("TP2_Level_Sell" + index, "sTP2", Idx_Lon_Start, TP2_Sell, VerticalAlignment.Top, HorizontalAlignment.Center, Colors.DarkOrange);
                }
                if (PL_Levels > 2)
                {
                    //+--------- Draw TP 3 level ------------------------+
                    ChartObjects.DrawLine("TP3 BUY" + index, Idx_Lon_Start, TP3_Buy, Idx_Lon_Start + index_Projection(TimeFrame.ToString(), Session_Hours), TP3_Buy, Colors.Green, 1, LineStyle.Lines);
                    ChartObjects.DrawText("TP3_Level_buy" + index, "bTP3", Idx_Lon_Start, TP3_Buy, VerticalAlignment.Top, HorizontalAlignment.Center, Colors.Green);

                    //+--------- Draw SL 3 level ------------------------+
                    ChartObjects.DrawLine("TP3 Sell" + index, Idx_Lon_Start, TP3_Sell, Idx_Lon_Start + index_Projection(TimeFrame.ToString(), Session_Hours), TP3_Sell, Colors.DarkOrange, 1, LineStyle.Lines);
                    ChartObjects.DrawText("TP3_Level_Sell" + index, "sTP3", Idx_Lon_Start, TP3_Sell, VerticalAlignment.Top, HorizontalAlignment.Center, Colors.DarkOrange);
                }
            }

        }
        



        //+-------------------------------------------------------------------------------------------------
        public int index_Projection(string TF, int Duration)
        {
            var Factor = 0;
            switch (TF)
            {
                case "Minute":
                    Factor = Duration * 60;
                    break;
                case "Minute2":
                    Factor = (Duration * 60) / 2;
                    break;
                case "Minute3":
                    Factor = (Duration * 60) / 3;
                    break;
                case "Minute4":
                    Factor = (Duration * 60) / 4;
                    break;
                case "Minute5":
                    Factor = (Duration * 60) / 5;
                    break;
                case "Minute6":
                    Factor = (Duration * 60) / 6;
                    break;
                case "Minute7":
                    Factor = (Duration * 60) / 7;
                    break;
                case "Minute8":
                    Factor = (Duration * 60) / 8;
                    break;
                case "Minute9":
                    Factor = (Duration * 60) / 9;
                    break;
                case "Minute10":
                    Factor = (Duration * 60) / 10;
                    break;
                case "Minute15":
                    Factor = (Duration * 60) / 15;
                    break;
                case "Minute20":
                    Factor = (Duration * 60) / 20;
                    break;
                case "Minute30":
                    Factor = (Duration * 60) / 30;
                    break;
                case "Minute45":
                    Factor = (Duration * 60) / 45;
                    break;
                case "Hour":
                    Factor = Duration;
                    break;

            }

            return Factor;
        }
        //+-------------------------------------------------------------------------------------------------+
        private void DrawRectangle(string objectName, int x, double y, int width, double height, Colors color, int thickness, LineStyle style)
        {
            ChartObjects.DrawLine(objectName + "a", x, y, x + width, y, color, thickness, style);
            ChartObjects.DrawLine(objectName + "b", x + width, y, x + width, y + height, color, thickness, style);
            ChartObjects.DrawLine(objectName + "c", x + width, y + height, x, y + height, color, thickness, style);
            ChartObjects.DrawLine(objectName + "d", x, y + height, x, y, color, thickness, style);
        }
        //+-------------------------------------------------------------------------------------------------
    }
}
