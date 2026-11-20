using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using DataFac.Storage.Testing;
using MemoryPack;
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
        var orig = new Testing.Models.MemPack.Simple();
        orig.Field1 = 123456L;
        orig.Field2 = -23456L;
        orig.Field3 = "abcdefghijklmnopqrstuvwxyz";
        await orig.Pack(_dataStore, CancellationToken.None);
        var copy = orig.ShallowCopy() as Testing.Models.MemPack.Simple;
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
        var orig = new Testing.Models.MsgPack2.Simple();
        orig.Field1 = 123456L;
        orig.Field2 = -23456L;
        orig.Field3 = "abcdefghijklmnopqrstuvwxyz";
        orig.Freeze();
        var copy = orig.ShallowCopy() as Testing.Models.MsgPack2.Simple;
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
        var orig = new Testing.Models.NetStrux.Simple();
        orig.Field1 = 123456L;
        orig.Field2 = -23456L;
        orig.Field3 = "abcdefghijklmnopqrstuvwxyz";
        await orig.Pack(_dataStore, CancellationToken.None);
        var copy = orig.ShallowCopy() as Testing.Models.NetStrux.Simple;
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return 1;
    }

    //[Benchmark]
    //public async ValueTask<long> HyperSer()
    //{
    //    var orig = new Testing.Models.HyperSer.Simple();
    //    orig.Field1 = 123456L;
    //    orig.Field2 = -23456L;
    //    orig.Field3 = "abcdefghijklmnopqrstuvwxyz";
    //    await orig.Pack(_dataStore, CancellationToken.None);
    //    var copy = orig.ShallowCopy() as Testing.Models.HyperSer.Simple;
    //    if (_checkValues)
    //    {
    //        if (copy is null) throw new Exception("Roundtrip entity is null!");
    //        if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
    //    }
    //    return 1;
    //}

    // todo Nino is buggy - try V4 again after release
    //[Benchmark]
    //public async ValueTask<long> NinoSer()
    //{
    //    var orig = new Testing.Models.NinoSer.Simple();
    //    orig.Field1 = 123456L;
    //    orig.Field2 = -23456L;
    //    orig.Field3 = "abcdefghijklmnopqrstuvwxyz";
    //    await orig.Pack(_dataStore, CancellationToken.None);
    //    Span<byte> buffer = NinoSerializer.Serialize(orig);
    //    var copy = NinoDeserializer.Deserialize<Testing.Models.NinoSer.Simple>(buffer);
    //    if (_checkValues)
    //    {
    //        if (copy is null) throw new Exception("Roundtrip entity is null!");
    //        if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
    //    }
    //    return buffer.Length;
    //}

    [Benchmark]
    public async ValueTask<long> MemBlocks()
    {
        var orig = new Testing.Models.MemBlocks.Simple();
        orig.Field1 = 123456L;
        orig.Field2 = -23456L;
        orig.Field3 = "abcdefghijklmnopqrstuvwxyz";
        await orig.Pack(_dataStore, CancellationToken.None);
         var copy = orig.ShallowCopy() as Testing.Models.MemBlocks.Simple;
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
        var orig = new Testing.Models.JsonSystemText.Simple();
        orig.Field1 = 123456L;
        orig.Field2 = -23456L;
        orig.Field3 = "abcdefghijklmnopqrstuvwxyz";
        orig.Freeze();
        var copy = orig.ShallowCopy() as Testing.Models.JsonSystemText.Simple;
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
        var orig = new Testing.Models.JsonNewtonSoft.Simple();
        orig.Field1 = 123456L;
        orig.Field2 = -23456L;
        orig.Field3 = "abcdefghijklmnopqrstuvwxyz";
        orig.Freeze();
        var copy = orig.ShallowCopy() as Testing.Models.JsonNewtonSoft.Simple;
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return 1;
    }
}
