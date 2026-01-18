using DTOMaker.Models;
using T_CustomMemberType_ = System.DayOfWeek;
using T_NativeMemberType_ = System.Int32;
namespace T_MemberTypeIntfSpace_
{
    public interface T_MemberTypeIntfName_
    {
        long Field1 { get; set; }
    }
}
namespace T_ConverterSpace_
{
    public class T_ConverterName_ : DTOMaker.Models.IStructConverter<T_CustomMemberType_, T_NativeMemberType_>
    {
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
