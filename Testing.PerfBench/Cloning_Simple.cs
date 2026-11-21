using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using DataFac.Storage.Testing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Testing.PerfBench;

//[SimpleJob(RuntimeMoniker.Net80)]
//[SimpleJob(RuntimeMoniker.Net90)]
[SimpleJob(RuntimeMoniker.Net10_0)]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class Cloning_Simple
{
    public Cloning_Simple() => _checkValues = false;
    public Cloning_Simple(bool checkValues) => _checkValues = checkValues;

    /// <summary>
    /// Unit tests should set this to true to validate that the values are correctly roundtripped.
    /// </summary>
    private readonly bool _checkValues;

    private readonly TestBlobStore _dataStore = new TestBlobStore();

    [Benchmark(Baseline = true)]
    public async ValueTask<long> MemoryPack()
    {
        var orig = new Testing.ModelsBasic.MemPack.Simple();
        orig.Field1 = 123456L;
        orig.Field2 = -23456L;
        orig.Field3 = "abcdefghijklmnopqrstuvwxyz";
        await orig.Pack(_dataStore, CancellationToken.None);
        var copy = orig.ShallowCopy() as Testing.ModelsBasic.MemPack.Simple;
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
        var orig = new Testing.ModelsBasic.MsgPack2.Simple();
        orig.Field1 = 123456L;
        orig.Field2 = -23456L;
        orig.Field3 = "abcdefghijklmnopqrstuvwxyz";
        orig.Freeze();
        var copy = orig.ShallowCopy() as Testing.ModelsBasic.MsgPack2.Simple;
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
        var orig = new Testing.ModelsBasic.NetStrux.Simple();
        orig.Field1 = 123456L;
        orig.Field2 = -23456L;
        orig.Field3 = "abcdefghijklmnopqrstuvwxyz";
        await orig.Pack(_dataStore, CancellationToken.None);
        var copy = orig.ShallowCopy() as Testing.ModelsBasic.NetStrux.Simple;
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
        var orig = new Testing.ModelsBasic.MemBlocks.Simple();
        orig.Field1 = 123456L;
        orig.Field2 = -23456L;
        orig.Field3 = "abcdefghijklmnopqrstuvwxyz";
        await orig.Pack(_dataStore, CancellationToken.None);
         var copy = orig.ShallowCopy() as Testing.ModelsBasic.MemBlocks.Simple;
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            await copy.Pack(_dataStore, CancellationToken.None);
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return 1;
    }

    [Benchmark]
    public async ValueTask<long> JsonSystemText()
    {
        var orig = new Testing.ModelsBasic.JsonSystemText.Simple();
        orig.Field1 = 123456L;
        orig.Field2 = -23456L;
        orig.Field3 = "abcdefghijklmnopqrstuvwxyz";
        orig.Freeze();
        var copy = orig.ShallowCopy() as Testing.ModelsBasic.JsonSystemText.Simple;
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
        var orig = new Testing.ModelsBasic.JsonNewtonSoft.Simple();
        orig.Field1 = 123456L;
        orig.Field2 = -23456L;
        orig.Field3 = "abcdefghijklmnopqrstuvwxyz";
        orig.Freeze();
        var copy = orig.ShallowCopy() as Testing.ModelsBasic.JsonNewtonSoft.Simple;
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return 1;
    }
}
