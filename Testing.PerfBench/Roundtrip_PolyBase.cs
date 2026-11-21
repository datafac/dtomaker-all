using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using DataFac.Storage.Testing;
using DTOMaker.Runtime.MsgPack2;
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
public class Roundtrip_PolyBase
{
    public Roundtrip_PolyBase() => _checkValues = false;
    public Roundtrip_PolyBase(bool checkValues)
    {
        _checkValues = checkValues;
    }

    /// <summary>
    /// Unit tests should set this to true to validate that the values are correctly roundtripped.
    /// </summary>
    private readonly bool _checkValues;

    private readonly TestBlobStore _dataStore = new TestBlobStore();

    [Benchmark(Baseline = true)]
    public async ValueTask<long> MemoryPack()
    {
        var orig = new ModelsPolymorphic.MemPack.FloatNumber
        {
            Value = Math.PI
        };
        await orig.Pack(_dataStore, CancellationToken.None);
        ReadOnlySpan<byte> buffer = MemoryPackSerializer.Serialize<ModelsPolymorphic.MemPack.NumberBase>(orig);
        var copy = MemoryPackSerializer.Deserialize<ModelsPolymorphic.MemPack.NumberBase>(buffer);
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (copy is not ModelsPolymorphic.MemPack.FloatNumber) throw new Exception("Roundtrip entity is not correct type!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffer.Length;
    }

    [Benchmark]
    public async ValueTask<long> NetStrux()
    {
        var orig = new ModelsPolymorphic.NetStrux.FloatNumber
        {
            Value = Math.PI
        };
        await orig.Pack(_dataStore, CancellationToken.None);
        var buffer = orig.SerializeSync();
        var copy = Testing.ModelsPolymorphic.NetStrux.NumberBase.DeserializeFrom(buffer.Span);
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (copy is not ModelsPolymorphic.NetStrux.FloatNumber) throw new Exception("Roundtrip entity is not correct type!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffer.Length;
    }

    [Benchmark]
    public async ValueTask<long> MsgPack2()
    {
        var orig = new ModelsPolymorphic.MsgPack2.FloatNumber
        {
            Value = Math.PI
        };
        orig.Freeze();
        var buffer = orig.SerializeToMessagePack<ModelsPolymorphic.MsgPack2.NumberBase>();
        var copy = buffer.DeserializeFromMessagePack<ModelsPolymorphic.MsgPack2.NumberBase>();
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (copy is not ModelsPolymorphic.MsgPack2.FloatNumber) throw new Exception("Roundtrip entity is not correct type!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffer.Length;
    }

    [Benchmark]
    public async ValueTask<long> MemBlocks()
    {
        var orig = new ModelsPolymorphic.MemBlocks.FloatNumber
        {
            Value = Math.PI
        };
        await orig.Pack(_dataStore, CancellationToken.None);
        var buffer = orig.SerializeSync();
        var copy = Testing.ModelsPolymorphic.MemBlocks.NumberBase.DeserializeFrom(buffer);
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (copy is not ModelsPolymorphic.MemBlocks.FloatNumber) throw new Exception("Roundtrip entity is not correct type!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffer.Length;
    }

    [Benchmark]
    public async ValueTask<long> JsonSystemText()
    {
        var orig = new ModelsPolymorphic.JsonSystemText.FloatNumber
        {
            Value = Math.PI
        };
        orig.Freeze();
        string buffer = DTOMaker.Runtime.JsonSystemText.SerializationHelpers.SerializeToJson<ModelsPolymorphic.JsonSystemText.NumberBase>(orig);
        var copy = DTOMaker.Runtime.JsonSystemText.SerializationHelpers.DeserializeFromJson<ModelsPolymorphic.JsonSystemText.NumberBase>(buffer);
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (copy is not ModelsPolymorphic.JsonSystemText.FloatNumber) throw new Exception("Roundtrip entity is not correct type!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffer.Length;
    }

    [Benchmark]
    public async ValueTask<long> JsonNewtonSoft()
    {
        var orig = new ModelsPolymorphic.JsonNewtonSoft.FloatNumber
        {
            Value = Math.PI
        };
        orig.Freeze();
        string buffer = DTOMaker.Runtime.JsonNewtonSoft.SerializationHelpers.SerializeToJson<ModelsPolymorphic.JsonNewtonSoft.NumberBase>(orig);
        var copy = DTOMaker.Runtime.JsonNewtonSoft.SerializationHelpers.DeserializeFromJson<ModelsPolymorphic.JsonNewtonSoft.NumberBase>(buffer);
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (copy is not ModelsPolymorphic.JsonNewtonSoft.FloatNumber) throw new Exception("Roundtrip entity is not correct type!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffer.Length;
    }
}
