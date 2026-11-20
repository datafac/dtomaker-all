using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using DataFac.Storage.Testing;
using DTOMaker.Runtime.MsgPack2;
using MemoryPack;
using Nino.Core;
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
        var orig = new Models.MemPack.FloatNumber
        {
            Value = Math.PI
        };
        await orig.Pack(_dataStore, CancellationToken.None);
        ReadOnlySpan<byte> buffer = MemoryPackSerializer.Serialize<Models.MemPack.NumberBase>(orig);
        var copy = MemoryPackSerializer.Deserialize<Models.MemPack.NumberBase>(buffer);
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (copy is not Models.MemPack.FloatNumber) throw new Exception("Roundtrip entity is not correct type!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffer.Length;
    }

    [Benchmark]
    public async ValueTask<long> NetStrux()
    {
        var orig = new Models.NetStrux.FloatNumber
        {
            Value = Math.PI
        };
        await orig.Pack(_dataStore, CancellationToken.None);
        var buffer = orig.SerializeSync();
        var copy = Testing.Models.NetStrux.NumberBase.DeserializeFrom(buffer.Span);
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (copy is not Models.NetStrux.FloatNumber) throw new Exception("Roundtrip entity is not correct type!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffer.Length;
    }

    //[Benchmark]
    //public async ValueTask<long> HyperSer()
    //{
    //    var orig = new Models.HyperSer.FloatNumber
    //    {
    //        Value = Math.PI
    //    };
    //    await orig.Pack(_dataStore, CancellationToken.None);
    //    ReadOnlySpan<byte> buffer = Hyper.HyperSerializer.Serialize<Models.HyperSer.NumberBase>(orig);
    //    var copy = Hyper.HyperSerializer.Deserialize<Models.HyperSer.NumberBase>(buffer);
    //    if (_checkValues)
    //    {
    //        if (copy is null) throw new Exception("Roundtrip entity is null!");
    //        if (copy is not Models.HyperSer.FloatNumber) throw new Exception("Roundtrip entity is not correct type!");
    //        if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
    //    }
    //    return buffer.Length;
    //}

    // todo Nino is buggy - try V4 again after release
    //[Benchmark]
    //public async ValueTask<long> NinoSer()
    //{
    //    var orig = new Models.NinoSer.FloatNumber();
    //    orig.Value = Math.PI;
    //    await orig.Pack(_dataStore, CancellationToken.None);
    //    ReadOnlySpan<byte> buffer = NinoSerializer.Serialize<Models.NinoSer.Simple>(orig);
    //    var copy = NinoDeserializer.Deserialize<Models.NinoSer.Simple>(buffer);
    //    if (_checkValues)
    //    {
    //        if (copy is null) throw new Exception("Roundtrip entity is null!");
    //        if (copy is not Models.NinoSer.FloatNumber) throw new Exception("Roundtrip entity is not correct type!");
    //        if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
    //    }
    //    return buffer.Length;
    //}

    [Benchmark]
    public async ValueTask<long> MsgPack2()
    {
        var orig = new Models.MsgPack2.FloatNumber
        {
            Value = Math.PI
        };
        orig.Freeze();
        var buffer = orig.SerializeToMessagePack<Models.MsgPack2.NumberBase>();
        var copy = buffer.DeserializeFromMessagePack<Models.MsgPack2.NumberBase>();
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (copy is not Models.MsgPack2.FloatNumber) throw new Exception("Roundtrip entity is not correct type!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffer.Length;
    }

    [Benchmark]
    public async ValueTask<long> MemBlocks()
    {
        var orig = new Models.MemBlocks.FloatNumber
        {
            Value = Math.PI
        };
        await orig.Pack(_dataStore, CancellationToken.None);
        var buffer = orig.SerializeSync();
        var copy = Testing.Models.MemBlocks.NumberBase.DeserializeFrom(buffer);
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (copy is not Models.MemBlocks.FloatNumber) throw new Exception("Roundtrip entity is not correct type!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffer.Length;
    }

    [Benchmark]
    public async ValueTask<long> JsonSystemText()
    {
        var orig = new Models.JsonSystemText.FloatNumber
        {
            Value = Math.PI
        };
        orig.Freeze();
        string buffer = DTOMaker.Runtime.JsonSystemText.SerializationHelpers.SerializeToJson<Models.JsonSystemText.NumberBase>(orig);
        var copy = DTOMaker.Runtime.JsonSystemText.SerializationHelpers.DeserializeFromJson<Models.JsonSystemText.NumberBase>(buffer);
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (copy is not Models.JsonSystemText.FloatNumber) throw new Exception("Roundtrip entity is not correct type!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffer.Length;
    }

    [Benchmark]
    public async ValueTask<long> JsonNewtonSoft()
    {
        var orig = new Models.JsonNewtonSoft.FloatNumber
        {
            Value = Math.PI
        };
        orig.Freeze();
        string buffer = DTOMaker.Runtime.JsonNewtonSoft.SerializationHelpers.SerializeToJson<Models.JsonNewtonSoft.NumberBase>(orig);
        var copy = DTOMaker.Runtime.JsonNewtonSoft.SerializationHelpers.DeserializeFromJson<Models.JsonNewtonSoft.NumberBase>(buffer);
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (copy is not Models.JsonNewtonSoft.FloatNumber) throw new Exception("Roundtrip entity is not correct type!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffer.Length;
    }
}
