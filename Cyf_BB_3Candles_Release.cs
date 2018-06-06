using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Cyf_BB_3Candles_Release : Indicator
    {


        private string UP = "▲";
        private string Down = "▼";
        private VerticalAlignment vAlign = VerticalAlignment.Center;
        private HorizontalAlignment hAlign = HorizontalAlignment.Center;

        private BollingerBands BB;

        private bool FireSoundAlarm;
        private int theCount;

        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 20)]
        public int Periods { get; set; }

        [Parameter(DefaultValue = 2)]
        public double Std_Deviation { get; set; }

        [Parameter()]
        public MovingAverageType MA_Type { get; set; }

        [Parameter("Test Candles", DefaultValue = 3, MinValue = 1)]
        public int Test_Candles { get; set; }

        [Parameter("Signal Offset", DefaultValue = 5, MinValue = 1)]
        public int offset { get; set; }

        [Parameter(DefaultValue = true)]
        public bool SoundAlert { get; set; }

        [Parameter(DefaultValue = "C:\\Sounds\\Bike Horn.wav")]
        public string SoundFile { get; set; }

        [Parameter("Alert Once", DefaultValue = true)]
        public bool Alert_Once { get; set; }



        [Output("Main")]
        public IndicatorDataSeries Main { get; set; }

        [Output("Top")]
        public IndicatorDataSeries Top { get; set; }

        [Output("Bottom")]
        public IndicatorDataSeries Bottom { get; set; }


        protected override void Initialize()
        {

            BB = Indicators.BollingerBands(Source, Periods, Std_Deviation, MA_Type);

            FireSoundAlarm = true;
            theCount = MarketSeries.Close.Count;
        }

        public override void Calculate(int index)
        {
            if (index == theCount + 1)
            {

                theCount += 1;
                FireSoundAlarm = true;
            }

            Main[index] = BB.Main[index];
            Top[index] = BB.Top[index];
            Bottom[index] = BB.Bottom[index];

            for (int i = 1; i <= Test_Candles; i++)
            {
                if (MarketSeries.Close[index - i] > BB.Top[index - i])
                {
                    if (i == Test_Candles)
                    {
                        ChartObjects.DrawText("SellSignal" + index, Down, index - 1, MarketSeries.High[index - 1] + offset * Symbol.PipSize, vAlign, hAlign, Colors.Red);

                        if (SoundAlert && FireSoundAlarm)
                        {
                            Notifications.PlaySound(SoundFile);
                            if (Alert_Once)
                                FireSoundAlarm = false;
                        }
                    }
                }

                else if (MarketSeries.Close[index - i] < BB.Bottom[index - i])
                {
                    if (i == Test_Candles)
                    {
                        ChartObjects.DrawText("BuySignal" + index, UP, index - 1, MarketSeries.Low[index - 1] - offset * Symbol.PipSize, vAlign, hAlign, Colors.Lime);

                        if (SoundAlert && FireSoundAlarm)
                        {
                            Notifications.PlaySound(SoundFile);
                            if (Alert_Once)
                                FireSoundAlarm = false;

                        }

                    }
                }
                else
                    break;
            }

        }
    }
}
