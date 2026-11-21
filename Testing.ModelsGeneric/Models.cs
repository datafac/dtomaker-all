using DTOMaker.Models;

namespace Testing.ModelsGeneric;

[Entity(9)]
public interface ICoupleBase : IEntityBase
{
}

[Entity(10)]
public interface ICouple<T1, T2> : ICoupleBase
{
    [Member(1)] T1 Item1 { get; set; }
    [Member(2)] T2 Item2 { get; set; }
}

[Entity(11)]
public interface IMyCoupleA : ICouple<int, string>
{
}

[Entity(12)]
public interface IMyCoupleB : IEntityBase
{
    [Member(1)] ICouple<int, string> Pair { get; set; }
}

//[Entity(12)]
//public interface IVector8<T> : IEntityBase
//{
//    [Member(1)] T Item0 { get; set; }
//    [Member(2)] T Item1 { get; set; }
//    [Member(3)] T Item2 { get; set; }
//    [Member(4)] T Item3 { get; set; }
//    [Member(5)] T Item4 { get; set; }
//    [Member(6)] T Item5 { get; set; }
//    [Member(7)] T Item6 { get; set; }
//    [Member(8)] T Item7 { get; set; }
//    //[Member(9)] T Item8 { get; set; }
//    //[Member(10)] T Item9 { get; set; }
//    //[Member(11)] T ItemA { get; set; }
//    //[Member(12)] T ItemB { get; set; }
//    //[Member(13)] T ItemC { get; set; }
//    //[Member(14)] T ItemD { get; set; }
//    //[Member(15)] T ItemE { get; set; }
//    //[Member(16)] T ItemF { get; set; }
//}

////[Struct]
//public readonly struct Pair<T1,T2>
//{
//    [Member(1)] T1 Item1 { get; }
//    [Member(2)] T2 Item2 { get; }
//    public Pair(T1 item1, T2 item2)
//    {
//        Item1 = item1;
//        Item2 = item2;
//    }
//}

//[Entity(13)]
//public interface IMyVector : IVector8<Pair<int, string>>
//{
//}
