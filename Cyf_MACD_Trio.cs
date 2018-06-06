using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Cyf_MACD_Trio : Indicator
    {
        private MacdHistogram MacD1;
        private MacdHistogram MacD2;
        private MacdHistogram MacD3;

        private MarketSeries Mid_Series;
        private MarketSeries Lng_Series;

        [Parameter("LongCycle", DefaultValue = 26)]
        public int LngCycle { get; set; }

        [Parameter("ShortCycle", DefaultValue = 12)]
        public int ShrtCycle { get; set; }

        [Parameter("SigPeriod", DefaultValue = 9)]
        public int SigPeriod { get; set; }

        [Parameter("MacD2_TF", DefaultValue = "Hour4")]
        public TimeFrame MacD2_TF { get; set; }

        [Parameter("MacD3_TF", DefaultValue = "Daily")]
        public TimeFrame MacD3_TF { get; set; }

        [Output("MaC3", PlotType = PlotType.Line, Color = Colors.Red)]
        public IndicatorDataSeries Mac3 { get; set; }

        [Output("MaC2", PlotType = PlotType.Line, Color = Colors.Green)]
        public IndicatorDataSeries Mac2 { get; set; }

        [Output("MaC1", PlotType = PlotType.Line, Color = Colors.AliceBlue)]
        public IndicatorDataSeries Mac1 { get; set; }

        protected override void Initialize()
        {

            Lng_Series = MarketData.GetSeries(MacD3_TF);
            Mid_Series = MarketData.GetSeries(MacD2_TF);
            // Initialize and create nested indicators
            MacD3 = Indicators.MacdHistogram(Lng_Series.Close, LngCycle, ShrtCycle, SigPeriod);
            MacD2 = Indicators.MacdHistogram(Mid_Series.Close, LngCycle, ShrtCycle, SigPeriod);
            MacD1 = Indicators.MacdHistogram(MarketSeries.Close, LngCycle, ShrtCycle, SigPeriod);

        }

        public override void Calculate(int index)
        {

            var indexMidTF = Mid_Series.OpenTime.GetIndexByTime(MarketSeries.OpenTime.LastValue);
            var indexLngTF = Lng_Series.OpenTime.GetIndexByTime(MarketSeries.OpenTime.LastValue);

            Mac3[index] = MacD3.Signal[indexLngTF];
            Mac2[index] = MacD2.Signal[indexMidTF];
            Mac1[index] = MacD1.Signal[index];

        }
    }
}
