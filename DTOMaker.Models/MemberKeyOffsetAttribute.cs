using System;

namespace DTOMaker.Models;

[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
public class MemberKeyOffsetAttribute : Attribute
{
    public readonly int MemberKeyOffset;

    public MemberKeyOffsetAttribute(int memberKeyOffset)
    {
        MemberKeyOffset = memberKeyOffset;
    }
}
