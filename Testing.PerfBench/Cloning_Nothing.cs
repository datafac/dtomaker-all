using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using DataFac.Storage.Testing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Testing.PerfBench;

//[NothingJob(RuntimeMoniker.Net80)]
//[NothingJob(RuntimeMoniker.Net90)]
[SimpleJob(RuntimeMoniker.Net10_0)]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class Cloning_Nothing
{
    public Cloning_Nothing() => _checkValues = false;
    public Cloning_Nothing(bool checkValues) => _checkValues = checkValues;

    /// <summary>
    /// Unit tests should set this to true to validate that the values are correctly roundtripped.
    /// </summary>
    private readonly bool _checkValues;

    private readonly TestBlobStore _dataStore = new TestBlobStore();

    [Benchmark(Baseline = true)]
    public async ValueTask<long> MemoryPack()
    {
        var orig = new Testing.ModelsBasic.MemPack.Nothing();
        await orig.Pack(_dataStore, CancellationToken.None);
        var copy = orig.ShallowCopy() as Testing.ModelsBasic.MemPack.Nothing;
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return 1;
    }

    [Benchmark]
    public async ValueTask<long> MsgPack2()
    {
        var orig = new Testing.ModelsBasic.MsgPack2.Nothing();
        orig.Freeze();
        var copy = orig.ShallowCopy() as Testing.ModelsBasic.MsgPack2.Nothing;
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return 1;
    }

    [Benchmark]
    public async ValueTask<long> NetStrux()
    {
        var orig = new Testing.ModelsBasic.NetStrux.Nothing();
        await orig.Pack(_dataStore, CancellationToken.None);
        var copy = orig.ShallowCopy() as Testing.ModelsBasic.NetStrux.Nothing;
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return 1;
    }

    [Benchmark]
    public async ValueTask<long> MemBlocks()
    {
        var orig = new Testing.ModelsBasic.MemBlocks.Nothing();
        await orig.Pack(_dataStore, CancellationToken.None);
        var copy = orig.ShallowCopy() as Testing.ModelsBasic.MemBlocks.Nothing;
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return 1;
    }

    [Benchmark]
    public async ValueTask<long> JsonSystemText()
    {
        var orig = new Testing.ModelsBasic.JsonSystemText.Nothing();
        orig.Freeze();
        var copy = orig.ShallowCopy() as Testing.ModelsBasic.JsonSystemText.Nothing;
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return 1;
    }

    [Benchmark]
    public async ValueTask<long> JsonNewtonSoft()
    {
        var orig = new Testing.ModelsBasic.JsonNewtonSoft.Nothing();
        orig.Freeze();
        var copy = orig.ShallowCopy() as Testing.ModelsBasic.JsonNewtonSoft.Nothing;
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return 1;
    }
}
