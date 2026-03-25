using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using DataFac.Storage.Testing;
using DTOMaker.Runtime.MsgPack2;
using MemoryPack;
using System;
using System.Threading.Tasks;

namespace Testing.PerfBench;

public enum ValueKind
{
    StrNull,
    StrZero,
    StrB064,
    StrK002,
}

//[SimpleJob(RuntimeMoniker.Net80)]
//[SimpleJob(RuntimeMoniker.Net90)]
[SimpleJob(RuntimeMoniker.Net10_0)]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class Roundtrip_String
{
    //[Params(ValueKind.StrZero, ValueKind.StrB064, ValueKind.StrK002)]
    [Params(ValueKind.StrB064)]
    public ValueKind Kind { get; set; }

    public Roundtrip_String() => _checkValues = false;
    public Roundtrip_String(bool checkValues, ValueKind kind)
    {
        _checkValues = checkValues;
        Kind = kind;
    }

    /// <summary>
    /// Unit tests should set this to true to validate that the values are correctly roundtripped.
    /// </summary>
    private readonly bool _checkValues;

    private readonly TestDataStore _dataStore = new TestDataStore();

    private static string GetTestValue(ValueKind kind)
    {
        return kind switch
        {
            ValueKind.StrZero => string.Empty,
            ValueKind.StrB064 => "abcdefghijklmnopqrstuvwxyz",
            ValueKind.StrK002 =>
                """
                You came like a motherless child
                Empty handed and starry eyed
                I took you in under my wing
                I tried to fix your, fix your broken dreams
                Like the fall of ancient Rome
                Bring by brick and stone by stone
                Doesn't matter how far you've come
                You've always got further, further, to go
                The stories that you told
                Somehow brought a sense of hope
                You knew how hard I'd fall
                That's the cruellest thing of all
                Hey, hey, babe
                It was always you and I
                Sometimes we almost touched blue skies
                But your dead weight pulled me down
                Hey, hey, babe
                Like the lovers by the wall
                Somehow, I knew we'd lose it all
                So long, hey, hey, bye, bye
                Bye, bye
                You said you'd love me all with you've got
                But you don't know a thing about love
                You built me an empty shrine
                Of stardust and rusty knives
                You knew how hard I'd fall
                That's the cruellest thing of all
                Hey, hey, babe
                It was always you and I
                Sometimes we almost touched blue skies
                But your dead weight pulled me down
                Hey, hey, babe
                Like the lovers by the wall
                A final kiss when darkness falls
                So long, hey, hey, bye, bye
                Bye, bye
                We were always a lover's lament
                Such beautiful endless decent
                Yeah, we were always a lover's lament
                It was always
                It was always you and I
                Such a beautiful endless decent
                It was always
                It was always you and I
                You and I
                """,
            _ => string.Empty
        };
    }

    [Benchmark(Baseline = true)]
    public async ValueTask<long> MemoryPack()
    {
        var orig = new Testing.Models.MemPack.Required_String();
        orig.Field = GetTestValue(Kind);
        orig.Freeze();
        var buffer = MemoryPackSerializer.Serialize<Testing.Models.MemPack.Required_String>(orig);
        var copy = MemoryPackSerializer.Deserialize<Testing.Models.MemPack.Required_String>(buffer);
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
        var orig = new Testing.Models.MsgPack2.Required_String();
        orig.Field = GetTestValue(Kind);
        orig.Freeze();
        var buffer = orig.SerializeToMessagePack<Testing.Models.MsgPack2.Required_String>();
        var copy = buffer.DeserializeFromMessagePack<Testing.Models.MsgPack2.Required_String>();
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
        var orig = new Testing.Models.NetStrux.Required_String();
        orig.Field = GetTestValue(Kind);
        await orig.Pack(_dataStore);
        var buffers = orig.GetBuffers();
        var copy = new Testing.Models.NetStrux.Required_String(buffers);
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            await copy.UnpackAll(_dataStore);
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffers.Buffers.Length;
    }

    [Benchmark]
    public async ValueTask<long> MemBlox2()
    {
        var orig = new Testing.Models.MemBlox2.Required_String();
        orig.Field = GetTestValue(Kind);
        await orig.Pack(_dataStore);
        var buffers = orig.GetBuffers();
        var copy = new Testing.Models.MemBlox2.Required_String(buffers);
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            await copy.UnpackAll(_dataStore);
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffers.Buffers.Length;
    }

    [Benchmark]
    public async ValueTask<long> MemBlocks()
    {
        var orig = new Testing.Models.MemBlocks.Required_String();
        orig.Field = GetTestValue(Kind);
        await orig.Pack(_dataStore);
        var buffers = orig.GetBuffers();
        var copy = new Testing.Models.MemBlocks.Required_String(buffers);
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffers.Length;
    }

    [Benchmark]
    public async ValueTask<long> JsonSystemText()
    {
        var orig = new Testing.Models.JsonSystemText.Required_String();
        orig.Field = GetTestValue(Kind);
        orig.Freeze();
        string buffer = DTOMaker.Runtime.JsonSystemText.SerializationHelpers.SerializeToJson(orig);
        var copy = DTOMaker.Runtime.JsonSystemText.SerializationHelpers.DeserializeFromJson<Testing.Models.JsonSystemText.Required_String>(buffer);
        if (_checkValues)
        {
            if (copy is null) throw new Exception("Roundtrip entity is null!");
            if (!copy.Equals(orig)) throw new Exception("Roundtrip entity != original");
        }
        return buffer.Length;
    }
}
