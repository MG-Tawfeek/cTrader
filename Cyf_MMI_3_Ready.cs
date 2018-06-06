/*
-Set the Period to 75
-Apply a Simple MA and set its value to 20
*/

using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Cyf_MMI_3 : Indicator
    {


        private double[] mpIndex;
        private double TheMedianPrice;
        private SimpleMovingAverage SMA;

        [Parameter("Period", DefaultValue = 10, MinValue = 2)]
        public int Period { get; set; }

        [Parameter("Smooth", DefaultValue = 20, MinValue = 1)]
        public int Smooth { get; set; }

        [Output("MMI")]
        public IndicatorDataSeries MMI { get; set; }

        [Output("SM_Curve", Color = Colors.Yellow)]
        public IndicatorDataSeries SMC { get; set; }
        /////////////////////////////////////////
        public void TheMedian(int mIndex)
        {

            for (int i = 0; i < Period; i++)
            {
                mpIndex.SetValue(MarketSeries.Close[mIndex - i], i);
            }
            Array.Sort(mpIndex);

            if (Period % 2 != 0)
            {
                TheMedianPrice = mpIndex[((Period - 1) / 2)];
            }
            else
            {
                TheMedianPrice = (mpIndex[(Period / 2)] + mpIndex[(Period / 2) - 1]) / 2;

            }
        }
        /////////////////////////////////////////
        protected override void Initialize()
        {
            //Sm_Curve = CreateDataSeries();
            SMA = Indicators.SimpleMovingAverage(MMI, Smooth);
            //MMI = CreateDataSeries();
            mpIndex = new double[Period];
            mpIndex.Initialize();

        }
        /////////////////////////////////////////
        public override void Calculate(int index)
        {
            TheMedian(index);
            int nl = 0, nh = 0;

            for (int i = 0; i < Period; i++)
            {
                if (MarketSeries.Close[index - i] > TheMedianPrice && MarketSeries.Close[index - i] > MarketSeries.Close[index - i + 1])
                {
                    nl++;

                }
                else if (MarketSeries.Close[index - i] < TheMedianPrice && MarketSeries.Close[index - i] < MarketSeries.Close[index - i + 1])
                {
                    nh++;

                }

            }
            MMI[index] = 100 * (nl + nh) / (Period - 1);
            SMC[index] = SMA.Result[index];
        }
    }
}
