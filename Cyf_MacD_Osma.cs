/* 
-Re-Written For cTrader by Mohab Gamal
-same MacD but with Colored Histogram ??
*/
using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Cyf_MacD_Osma : Indicator
    {
        private MacdCrossOver MAC;
        private IndicatorDataSeries OSMA;

        [Parameter("osMA_Factor", DefaultValue = 1.5)]
        public double osMA_Factor { get; set; }

        [Parameter("Long_Cycle", DefaultValue = 26)]
        public int Long_Cycle { get; set; }

        [Parameter("Short_Cycle", DefaultValue = 12)]
        public int Short_Cycle { get; set; }

        [Parameter("Signal", DefaultValue = 9)]
        public int Signal { get; set; }

        [Output("MACD", Color = Colors.White)]
        public IndicatorDataSeries MACD { get; set; }

        [Output("Signal", Color = Colors.DodgerBlue)]
        public IndicatorDataSeries Sig_Line { get; set; }

        [Output("OSMA_UP1", Color = Colors.LimeGreen, IsHistogram = true, Thickness = 3)]
        public IndicatorDataSeries OS_Line_UP1 { get; set; }

        [Output("OSMA_UP2", Color = Colors.DarkGreen, IsHistogram = true, Thickness = 3)]
        public IndicatorDataSeries OS_Line_UP2 { get; set; }

        [Output("OSMA_Dn1", Color = Colors.Red, IsHistogram = true, Thickness = 3)]
        public IndicatorDataSeries OS_Line_Dn1 { get; set; }

        [Output("OSMA_Dn2", Color = Colors.PaleVioletRed, IsHistogram = true, Thickness = 3)]
        public IndicatorDataSeries OS_Line_Dn2 { get; set; }


        protected override void Initialize()
        {
            OSMA = CreateDataSeries();
            MAC = Indicators.MacdCrossOver(MarketSeries.Close, Long_Cycle, Short_Cycle, Signal);
        }

        public override void Calculate(int index)
        {
            OS_Line_UP1[index] = 0;
            OS_Line_UP2[index] = 0;
            OS_Line_Dn2[index] = 0;
            OS_Line_Dn1[index] = 0;

            MACD[index] = MAC.MACD[index];
            Sig_Line[index] = MAC.Signal[index];
            OSMA[index] = (MAC.MACD[index] - MAC.Signal[index]) * osMA_Factor;

            if (OSMA[index] > OSMA[index - 1] && OSMA[index] >= 0)
            {
                OS_Line_UP1[index] = OSMA[index];
            }
            else if (OSMA[index] < OSMA[index - 1] && OSMA[index] >= 0)
            {
                OS_Line_UP2[index] = OSMA[index];
            }
            if (OSMA[index] < OSMA[index - 1] && OSMA[index] < 0)
            {
                OS_Line_Dn1[index] = OSMA[index];
            }
            else if (OSMA[index] > OSMA[index - 1] && OSMA[index] < 0)
            {
                OS_Line_Dn2[index] = OSMA[index];
            }


        }
    }
}
