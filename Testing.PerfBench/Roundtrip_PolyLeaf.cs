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
public class Roundtrip_PolyLeaf
{
    public Roundtrip_PolyLeaf() => _checkValues = false;
    public Roundtrip_PolyLeaf(bool checkValues)
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
        var orig = new Testing.Models.MemPack.FloatNumber();
        orig.Value = Math.PI;
        await orig.Pack(_dataStore, CancellationToken.None);
        ReadOnlySpan<byte> buffer = MemoryPackSerializer.Serialize<Testing.Models.MemPack.FloatNumber>(orig);
        var copy = MemoryPackSerializer.Deserialize<Testing.Models.MemPack.FloatNumber>(buffer);
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
        var orig = new Testing.Models.NetStrux.FloatNumber();
        orig.Value = Math.PI;
        await orig.Pack(_dataStore, CancellationToken.None);
        var buffer = orig.SerializeSync();
        var copy = Testing.Models.NetStrux.FloatNumber.DeserializeFrom(buffer.Span);
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffer.Length;
    }

    //[Benchmark]
    //public async ValueTask<long> HyperSer()
    //{
    //    var orig = new Testing.Models.HyperSer.FloatNumber();
    //    orig.Value = Math.PI;
    //    await orig.Pack(_dataStore, CancellationToken.None);
    //    ReadOnlySpan<byte> buffer = Hyper.HyperSerializer.Serialize(orig);
    //    var copy = Hyper.HyperSerializer.Deserialize<Testing.Models.HyperSer.FloatNumber>(buffer);
    //    if (_checkValues)
    //    {
    //        if (copy is null) throw new Exception("Roundtrip entity is null!");
    //        if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
    //    }
    //    return buffer.Length;
    //}

    // todo Nino is buggy - try V4 again after release
    //[Benchmark]
    //public async ValueTask<long> NinoSer()
    //{
    //    var orig = new Testing.Models.NinoSer.FloatNumber();
    //    orig.Value = Math.PI;
    //    await orig.Pack(_dataStore, CancellationToken.None);
    //    ReadOnlySpan<byte> buffer = NinoSerializer.Serialize(orig);
    //    var copy = NinoDeserializer.Deserialize<Testing.Models.NinoSer.Simple>(buffer);
    //    if (_checkValues)
    //    {
    //        if (copy is null) throw new Exception("Roundtrip entity is null!");
    //        if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
    //    }
    //    return buffer.Length;
    //}

    [Benchmark]
    public async ValueTask<long> MsgPack2()
    {
        var orig = new Testing.Models.MsgPack2.FloatNumber();
        orig.Value = Math.PI;
        orig.Freeze();
        var buffer = orig.SerializeToMessagePack<Testing.Models.MsgPack2.FloatNumber>();
        var copy = buffer.DeserializeFromMessagePack<Testing.Models.MsgPack2.FloatNumber>();
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
        var orig = new Testing.Models.MemBlocks.FloatNumber();
        orig.Value = Math.PI;
        await orig.Pack(_dataStore, CancellationToken.None);
        var buffer = orig.SerializeSync();
        var copy = new Testing.Models.MemBlocks.FloatNumber(buffer);
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
        var orig = new Testing.Models.JsonSystemText.FloatNumber();
        orig.Value = Math.PI;
        orig.Freeze();
        string buffer = DTOMaker.Runtime.JsonSystemText.SerializationHelpers.SerializeToJson(orig);
        var copy = DTOMaker.Runtime.JsonSystemText.SerializationHelpers.DeserializeFromJson<Testing.Models.JsonSystemText.FloatNumber>(buffer);
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
        var orig = new Testing.Models.JsonNewtonSoft.FloatNumber();
        orig.Value = Math.PI;
        orig.Freeze();
        string buffer = DTOMaker.Runtime.JsonNewtonSoft.SerializationHelpers.SerializeToJson(orig);
        var copy = DTOMaker.Runtime.JsonNewtonSoft.SerializationHelpers.DeserializeFromJson<Testing.Models.JsonNewtonSoft.FloatNumber>(buffer);
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffer.Length;
    }
}
