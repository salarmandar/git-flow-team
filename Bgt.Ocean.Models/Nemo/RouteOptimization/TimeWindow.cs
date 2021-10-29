
namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class TimeWindow
    {
        public TimeWindow()
        {
            this.Start = 0;//seconds
            this.End = 86400;//seconds
        }

        public TimeWindow(long start, long end)
        {
            this.Start = start;
            this.End = end;
        }

        public long Start { get; set; }
        public long End { get; set; }
    }
}
