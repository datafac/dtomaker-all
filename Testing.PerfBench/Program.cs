using BenchmarkDotNet.Running;

namespace Testing.PerfBench;

public partial class Program
{
    public static void Main(string[] args)
    {
        // var summary = BenchmarkRunner.Run<Roundtrip_Int64>();
        var summary = BenchmarkRunner.Run<Roundtrip_String>();
    }
}
