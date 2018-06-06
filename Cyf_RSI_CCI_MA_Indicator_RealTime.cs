using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None, ScalePrecision = 0, AutoRescale = true)]
    public class Cyf_RSI_CCI_MA_Indicator_RealTime : Indicator
    {
        private const cAlgo.API.VerticalAlignment vAlign = cAlgo.API.VerticalAlignment.Center;
        private const cAlgo.API.HorizontalAlignment hAlign = cAlgo.API.HorizontalAlignment.Center;
        private string bullet = "♦";
        private string UP = "▲";
        private string Down = "▼";
        private bool emailWasSent = false;
        private int thecount;

        // Get Moving Averages for Different TimeFrames
        private MovingAverage MA;


        // Get RSI For Different TimeFrames
        private RelativeStrengthIndex RSI;


        // Get CCI For Different TimeFrames
        private CommodityChannelIndex CCI;

        //*************
        private LinearRegressionSlope LR_Slope;
        private ExponentialMovingAverage MA20;
        private ExponentialMovingAverage MA50;
        private ExponentialMovingAverage MA100;

        private MarketSeries LTF_Candle;
        private MarketSeries MTF_Candle;
        private MarketSeries STF_Candle;

        private bool DailyCandleUp;
        private bool HourlyCandleUp;
        private bool m5CandleUp;
        //*************

        //****************
        //[Parameter(DefaultValue = 12)]
        //public int Periods { get; set; }

        [Output("LR_Down", Color = Colors.Red, PlotType = PlotType.Histogram, Thickness = 3)]
        public IndicatorDataSeries moScillatorDown { get; set; }

        [Output("LR_Up", Color = Colors.Blue, PlotType = PlotType.Histogram, Thickness = 3)]
        public IndicatorDataSeries moScillatorUp { get; set; }


        [Output("LR_Down2", Color = Colors.LightPink, PlotType = PlotType.Histogram, Thickness = 3)]
        public IndicatorDataSeries moScillatorDown2 { get; set; }

        [Output("LR_Up2", Color = Colors.CornflowerBlue, PlotType = PlotType.Histogram, Thickness = 3)]
        public IndicatorDataSeries moScillatorUp2 { get; set; }
        //****************

        [Parameter(DefaultValue = true)]
        public bool AllSignals { get; set; }
        [Parameter(DefaultValue = true)]
        public bool SoundAlert { get; set; }

        [Parameter(DefaultValue = "C:\\Sounds\\Bike Horn.wav")]
        public string SoundFile { get; set; }

        [Parameter(DefaultValue = "mtawfeek@gmail.com")]
        public string Email_To { get; set; }

        [Parameter(DefaultValue = 34)]
        public int MA_Periods { get; set; }

        [Parameter("MA Type", DefaultValue = MovingAverageType.Exponential)]
        public MovingAverageType MAType { get; set; }

        [Parameter(DefaultValue = 14)]
        public int RSI_Periods { get; set; }

        [Parameter(DefaultValue = 14)]
        public int CCI_Periods { get; set; }

        [Parameter(DefaultValue = 90)]
        public int CCI_Value { get; set; }

        [Parameter(DefaultValue = 50)]
        public int RSI_Value { get; set; }


        [Parameter(DefaultValue = 1)]
        public int CCI_Lag { get; set; }

        [Parameter(DefaultValue = 1)]
        public int RSI_Lag { get; set; }
        ///////////////////////////////////////////////////

        //string fromAddress = "Ctrader@gmail.com";
        //Sender's Address
        //string toAdress = "yourEmail@gmail.com";
        // Recipient's Address
        string subject = "Testing email";
        //Email Subject
        string Email_text = "This email was sent from cAlgo.";

        ///////////////////////////////////////////////////


        [Output("CCI", Color = Colors.Red, PlotType = PlotType.Line, Thickness = 0)]
        public IndicatorDataSeries CCI_Line { get; set; }

        [Output("RSI", Color = Colors.Blue, PlotType = PlotType.Line, Thickness = 0)]
        public IndicatorDataSeries RSI_Line { get; set; }

        //******************************************
        [Parameter("LTF_Candle", DefaultValue = "Daily")]
        public TimeFrame LTF { get; set; }

        [Parameter("MTF_Candle", DefaultValue = "Hour4")]
        public TimeFrame MTF { get; set; }

        [Parameter("STF_Candle", DefaultValue = "Hour")]
        public TimeFrame STF { get; set; }
        //******************************************
        public void CheckCandles()
        {
            if (LTF_Candle.Open.Last(0) > LTF_Candle.Close.Last(0))
            {
                DailyCandleUp = false;
                ChartObjects.RemoveObject("theText");
                ChartObjects.DrawText("theText", LTF + "_Candle" + Down, StaticPosition.BottomLeft, Colors.Red);
            }
            else if (LTF_Candle.Open.Last(0) < LTF_Candle.Close.Last(0))
            {
                DailyCandleUp = true;
                ChartObjects.RemoveObject("theText");
                ChartObjects.DrawText("theText", LTF + "_Candle" + " " + UP, StaticPosition.BottomLeft, Colors.Green);
            }

            if (MTF_Candle.Open.Last(0) > MTF_Candle.Close.Last(0))
            {
                HourlyCandleUp = false;
                ChartObjects.RemoveObject("theText2");
                ChartObjects.DrawText("theText2", MTF + "_Candle" + " " + Down, StaticPosition.BottomCenter, Colors.Red);
            }
            else if (MTF_Candle.Open.Last(0) < MTF_Candle.Close.Last(0))
            {
                HourlyCandleUp = true;
                ChartObjects.RemoveObject("theText2");
                ChartObjects.DrawText("theText2", MTF + "_Candle" + " " + UP, StaticPosition.BottomCenter, Colors.Green);
            }

            if (STF_Candle.Open.Last(0) > STF_Candle.Close.Last(0))
            {
                m5CandleUp = false;
                ChartObjects.RemoveObject("theText3");
                ChartObjects.DrawText("theText3", STF + "_Candle" + " " + Down, StaticPosition.BottomRight, Colors.Red);
            }
            else if (STF_Candle.Open.Last(0) < STF_Candle.Close.Last(0))
            {
                m5CandleUp = true;
                ChartObjects.RemoveObject("theText3");
                ChartObjects.DrawText("theText3", STF + "_Candle" + " " + UP, StaticPosition.BottomRight, Colors.Green);
            }

        }
        //******************************************

        protected override void Initialize()
        {
            //**************************************
            LR_Slope = Indicators.LinearRegressionSlope(MarketSeries.Close, 12);
            MA20 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 20);
            MA50 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 50);
            MA100 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 100);

            LTF_Candle = MarketData.GetSeries(LTF);
            MTF_Candle = MarketData.GetSeries(MTF);
            STF_Candle = MarketData.GetSeries(STF);
            //**************************************

            ///////////////////////////////////////////////////////////

            // Initialize and create nested indicators

            MA = Indicators.MovingAverage(MarketSeries.Close, MA_Periods, MovingAverageType.Exponential);
            // Initalize RSI
            RSI = Indicators.RelativeStrengthIndex(MarketSeries.Close, RSI_Periods);

            // Initalize CCI
            CCI = Indicators.CommodityChannelIndex(MarketSeries, CCI_Periods);

            thecount = MarketSeries.Close.Count;
            Print("theCount = " + thecount);

        }

        public override void Calculate(int index)
        {

            CheckCandles();
            ///*******************************
            if (LR_Slope.Result[index] >= 0.0)
            {
                if (LR_Slope.Result.IsRising())
                {
                    moScillatorUp[index] = LR_Slope.Result[index] * 100;
                }
                else
                {
                    moScillatorUp2[index] = LR_Slope.Result[index] * 100;
                }
                
            }
            else
            {
                if (LR_Slope.Result.IsFalling())
                {
                    moScillatorDown[index] = (LR_Slope.Result[index] * 50);
                }

                else
                {
                    moScillatorDown2[index] = (LR_Slope.Result[index] * 50);
                }

                
            }
            ///*******************************
            if (index == thecount + 1)
            {
                Print("the New Count is : " + index);
                thecount += 1;
                emailWasSent = false;
            }

            CCI_Line[index] = CCI.Result.LastValue;
            RSI_Line[index] = RSI.Result.LastValue;


            if (MarketSeries.Close[index - 1] > MA.Result[index - 1] && MarketSeries.Close[index] < MA.Result[index])
            {

                if (CCI.Result.Last(CCI_Lag) > CCI_Value && RSI.Result.Last(RSI_Lag) > RSI_Value)
                {
                    ChartObjects.DrawText("Sell" + index, bullet, index, 0, vAlign, hAlign, Colors.Red);


                    subject = ("[Sell Signal From " + this.Symbol + "chart]");
                    Email_text = ("Symbol : " + this.Symbol + "\n" + "TimeFram : " + this.TimeFrame + "\n" + "Type : " + "Sell Signal" + "\n" + "Data : " + "\n" + "CCI Value : " + CCI.Result.Last(CCI_Lag) + "\n" + "RSI Value : " + RSI.Result.Last(RSI_Lag));

                    //Notifications.PlaySound("C:\\Sounds\\Bike Horn.wav");
                    if (SoundAlert)
                    {
                        Notifications.PlaySound(SoundFile);
                    }
                    //if (!emailWasSent)
                    if (emailWasSent == false)
                    {
                        Notifications.SendEmail(Email_To, Email_To, subject, Email_text);
                        emailWasSent = true;
                    }
                }
            }
            else
            {
                
            }
            

            if (MarketSeries.Close[index - 1] < MA.Result[index - 1] && MarketSeries.Close[index] > MA.Result[index])
            {

                if (CCI.Result.Last(CCI_Lag) < -CCI_Value && RSI.Result.Last(RSI_Lag) < RSI_Value)
                {
                    ChartObjects.DrawText("buy5" + index, bullet, index, 0, vAlign, hAlign, Colors.Green);

                    subject = ("[Buy Signal From " + this.Symbol + "chart]");
                    Email_text = ("Symbol : " + this.Symbol + "\n" + "TimeFram : " + this.TimeFrame + "\n" + "Type : " + "Buy Signal" + "\n" + "Data : " + "\n" + "CCI Value : " + CCI.Result.Last(CCI_Lag) + "\n" + "RSI Value : " + RSI.Result.Last(RSI_Lag));

                    
                    if (SoundAlert)
                    {
                        Notifications.PlaySound(SoundFile);
                    }
                    
                    if (emailWasSent == false)
                    {
                        Notifications.SendEmail(Email_To, Email_To, subject, Email_text);
                        emailWasSent = true;
                    }
                }
            }
            else
            {
                
            }




        }

    }

}
