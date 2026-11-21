using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using DataFac.Storage.Testing;
using DTOMaker.Runtime.MsgPack2;
using MemoryPack;
using System;
using System.Threading;
using System.Threading.Tasks;
using Testing.ModelsGeneric;
using static System.Net.Mime.MediaTypeNames;

namespace Testing.PerfBench;

//[SimpleJob(RuntimeMoniker.Net80)]
//[SimpleJob(RuntimeMoniker.Net90)]
[SimpleJob(RuntimeMoniker.Net10_0)]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class Roundtrip_Generic
{
    public Roundtrip_Generic() => _checkValues = false;
    public Roundtrip_Generic(bool checkValues) => _checkValues = checkValues;

    /// <summary>
    /// Unit tests should set this to true to validate that the values are correctly roundtripped.
    /// </summary>
    private readonly bool _checkValues;

    private readonly TestBlobStore _dataStore = new TestBlobStore();

    [Benchmark(Baseline = true)]
    public async ValueTask<long> MemoryPack()
    {
        var orig = new ModelsGeneric.MemPack.MyCoupleA
        {
            Item1 = 123,
            Item2 = "abcdefghijklmnopqrstuvwxyz",
        };
        await orig.Pack(_dataStore, CancellationToken.None);
        ReadOnlySpan<byte> buffer = MemoryPackSerializer.Serialize<ModelsGeneric.MemPack.MyCoupleA>(orig);
        var copy = MemoryPackSerializer.Deserialize<ModelsGeneric.MemPack.MyCoupleA>(buffer);
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (copy is not ModelsGeneric.MemPack.MyCoupleA) throw new Exception("Roundtrip entity is not correct type!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffer.Length;
    }

}
