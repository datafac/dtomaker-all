using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using DataFac.Storage.Testing;
using DTOMaker.Runtime.MsgPack2;
using MemoryPack;
using System;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Testing.PerfBench;

//[SimpleJob(RuntimeMoniker.Net80)]
//[SimpleJob(RuntimeMoniker.Net90)]
[SimpleJob(RuntimeMoniker.Net10_0)]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class Roundtrip_Simple
{
    public Roundtrip_Simple() => _checkValues = false;
    public Roundtrip_Simple(bool checkValues) => _checkValues = checkValues;

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
        var buffer = MemoryPackSerializer.Serialize<Testing.ModelsBasic.MemPack.Simple>(orig);
        var copy = MemoryPackSerializer.Deserialize<Testing.ModelsBasic.MemPack.Simple>(buffer);
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffer.Length;
    }

    [Benchmark]
    public async ValueTask<long> MsgPack2()
    {
        var orig = new Testing.ModelsBasic.MsgPack2.Simple();
        orig.Field1 = 123456L;
        orig.Field2 = -23456L;
        orig.Field3 = "abcdefghijklmnopqrstuvwxyz";
        orig.Freeze();
        var buffer = orig.SerializeToMessagePack<Testing.ModelsBasic.MsgPack2.Simple>();
        var copy = buffer.DeserializeFromMessagePack<Testing.ModelsBasic.MsgPack2.Simple>();
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffer.Length;
    }

    [Benchmark]
    public async ValueTask<long> NetStrux()
    {
        var orig = new Testing.ModelsBasic.NetStrux.Simple();
        orig.Field1 = 123456L;
        orig.Field2 = -23456L;
        orig.Field3 = "abcdefghijklmnopqrstuvwxyz";
        await orig.Pack(_dataStore, CancellationToken.None);
        var buffer = orig.SerializeSync();
        var copy = Testing.ModelsBasic.NetStrux.Simple.DeserializeFrom(buffer.Span);
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffer.Length;
    }

    [Benchmark]
    public async ValueTask<long> MemBlocks()
    {
        var orig = new Testing.ModelsBasic.MemBlocks.Simple();
        orig.Field1 = 123456L;
        orig.Field2 = -23456L;
        orig.Field3 = "abcdefghijklmnopqrstuvwxyz";
        await orig.Pack(_dataStore, CancellationToken.None);
        var buffer = orig.SerializeSync();
        var copy = new Testing.ModelsBasic.MemBlocks.Simple(buffer);
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffer.Length;
    }

    [Benchmark]
    public async ValueTask<long> JsonSystemText()
    {
        var orig = new Testing.ModelsBasic.JsonSystemText.Simple();
        orig.Field1 = 123456L;
        orig.Field2 = -23456L;
        orig.Field3 = "abcdefghijklmnopqrstuvwxyz";
        orig.Freeze();
        string buffer = DTOMaker.Runtime.JsonSystemText.SerializationHelpers.SerializeToJson(orig);
        var copy = DTOMaker.Runtime.JsonSystemText.SerializationHelpers.DeserializeFromJson<Testing.ModelsBasic.JsonSystemText.Simple>(buffer);
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffer.Length;
    }

    [Benchmark]
    public async ValueTask<long> JsonNewtonSoft()
    {
        var orig = new Testing.ModelsBasic.JsonNewtonSoft.Simple();
        orig.Field1 = 123456L;
        orig.Field2 = -23456L;
        orig.Field3 = "abcdefghijklmnopqrstuvwxyz";
        orig.Freeze();
        string buffer = DTOMaker.Runtime.JsonNewtonSoft.SerializationHelpers.SerializeToJson(orig);
        var copy = DTOMaker.Runtime.JsonNewtonSoft.SerializationHelpers.DeserializeFromJson<Testing.ModelsBasic.JsonNewtonSoft.Simple>(buffer);
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffer.Length;
    }
}
