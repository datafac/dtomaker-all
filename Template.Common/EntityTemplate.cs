using DataFac.Memory;
using DTOMaker.Models;
using T_CustomMemberType_ = System.DayOfWeek;
using T_NativeMemberType_ = System.Int32;
namespace T_MemberTypeIntfSpace_
{
    public interface T_MemberTypeIntfName_ : IEntityBase
    {
        long Field1 { get; set; }
    }
}
namespace T_ConverterSpace_
{
    public class T_ConverterName_ : DTOMaker.Models.IStructConverter<T_CustomMemberType_, T_NativeMemberType_>
    {
        public NativeType NativeType => NativeType.Int32;
        public static T_NativeMemberType_ ToNative(T_CustomMemberType_ custom) => (T_NativeMemberType_)custom;
        public static T_NativeMemberType_? ToNative(T_CustomMemberType_? custom) => custom.HasValue ? (T_NativeMemberType_)custom.Value : null;
        public static T_CustomMemberType_ ToCustom(T_NativeMemberType_ native) => (T_CustomMemberType_)native;
        public static T_CustomMemberType_? ToCustom(T_NativeMemberType_? native) => native.HasValue ? (T_CustomMemberType_)native : null;
    }
}
namespace T_BaseIntfNameSpace_
{
    public interface T_BaseIntfName_ : IEntityBase
    {
        T_NativeMemberType_ BaseField1 { get; set; }
    }
}
namespace T_IntfNameSpace_
{
    public interface T_EntityIntfName_ : T_BaseIntfNameSpace_.T_BaseIntfName_
    {
        T_NativeMemberType_? T_NullableNativeStructMemberName_ { get; set; }
        T_NativeMemberType_ T_RequiredNativeStructMemberName_ { get; set; }
        T_CustomMemberType_? T_NullableCustomStructMemberName_ { get; set; }
        T_CustomMemberType_ T_RequiredCustomStructMemberName_ { get; set; }
        T_MemberTypeIntfSpace_.T_MemberTypeIntfName_? T_NullableEntityMemberName_ { get; set; }
        T_MemberTypeIntfSpace_.T_MemberTypeIntfName_ T_RequiredEntityMemberName_ { get; set; }
        Octets? T_NullableBinaryMemberName_ { get; set; }
        Octets T_RequiredBinaryMemberName_ { get; set; }
        string? T_NullableStringMemberName_ { get; set; }
        string T_RequiredStringMemberName_ { get; set; }
    }
}

