using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using DataFac.Storage.Testing;
using DTOMaker.Runtime.MsgPack2;
using MemoryPack;
using System;
using System.Threading.Tasks;

namespace Testing.PerfBench;

//[SimpleJob(RuntimeMoniker.Net80)]
//[SimpleJob(RuntimeMoniker.Net90)]
[SimpleJob(RuntimeMoniker.Net10_0)]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class Roundtrip_Int64
{
    public Roundtrip_Int64() => _checkValues = false;
    public Roundtrip_Int64(bool checkValues) => _checkValues = checkValues;

    /// <summary>
    /// Unit tests should set this to true to validate that the values are correctly roundtripped.
    /// </summary>
    private readonly bool _checkValues;

    private readonly TestDataStore _dataStore = new TestDataStore();

    [Benchmark(Baseline = true)]
    public async ValueTask<long> MemoryPack()
    {
        var orig = new Testing.Models.MemPack.Required_Int64();
        orig.Field = 123456L;
        orig.Freeze();
        var buffer = MemoryPackSerializer.Serialize<Testing.Models.MemPack.Required_Int64>(orig);
        var copy = MemoryPackSerializer.Deserialize<Testing.Models.MemPack.Required_Int64>(buffer);
        copy!.Freeze();
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
        var orig = new Testing.Models.MsgPack2.Required_Int64();
        orig.Field = 123456L;
        orig.Freeze();
        var buffer = orig.SerializeToMessagePack<Testing.Models.MsgPack2.Required_Int64>();
        var copy = buffer.DeserializeFromMessagePack<Testing.Models.MsgPack2.Required_Int64>();
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
        var orig = new Testing.Models.MemBlocks.Required_Int64();
        orig.Field = 123456L;
        await orig.Pack(_dataStore);
        var content = orig.GetContent();
        var copy = new Testing.Models.MemBlocks.Required_Int64(content);
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return content.Length;
    }

    [Benchmark]
    public async ValueTask<long> JsonSystemText()
    {
        var orig = new Testing.Models.JsonSystemText.Required_Int64();
        orig.Field = 123456L;
        orig.Freeze();
        string buffer = DTOMaker.Runtime.JsonSystemText.SerializationHelpers.SerializeToJson(orig);
        var copy = DTOMaker.Runtime.JsonSystemText.SerializationHelpers.DeserializeFromJson<Testing.Models.JsonSystemText.Required_Int64>(buffer);
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffer.Length;
    }
}
