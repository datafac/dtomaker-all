using System;

namespace DTOMaker.Models
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MemberAttribute : Attribute
    {
        public readonly int Sequence;

        public MemberAttribute(int sequence)
        {
            Sequence = sequence;
        }

    }
}
