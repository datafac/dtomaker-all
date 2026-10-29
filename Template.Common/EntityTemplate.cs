using DataFac.Memory;
using DTOMaker.Models;
// old todo keep until feature done
using T_CustomMemberType_ = System.DayOfWeek;
using T_NativeMemberType_ = System.Int32;
// endold
using T_CustomStructType_ = System.DayOfWeek;
using T_NativeStructType_ = System.Int32;
using T_CustomStringType_ = System.Version;
using T_NativeStringType_ = System.String;
namespace T_MemberTypeIntfSpace_
{
    public interface T_MemberTypeIntfName_ : IEntityBase
    {
        long Field1 { get; set; }
    }
}
namespace T_ConverterSpace_
{
    // old todo keep until feature done
    public class T_ConverterName_ : DTOMaker.Models.IStructConverter<T_CustomMemberType_, T_NativeMemberType_>
    {
        public NativeType NativeType => NativeType.Int32;
        public static T_NativeMemberType_ ToNative(T_CustomMemberType_ custom) => (T_NativeMemberType_)custom;
        public static T_NativeMemberType_? ToNative(T_CustomMemberType_? custom) => custom.HasValue ? (T_NativeMemberType_)custom.Value : null;
        public static T_CustomMemberType_ ToCustom(T_NativeMemberType_ native) => (T_CustomMemberType_)native;
        public static T_CustomMemberType_? ToCustom(T_NativeMemberType_? native) => native.HasValue ? (T_CustomMemberType_)native : null;
    }
    // endold
    public class T_StructConverterName_ : DTOMaker.Models.IStructConverter<T_CustomStructType_, T_NativeStructType_>
    {
        public NativeType NativeType => NativeType.Int32;
        public static T_NativeStructType_ ToNative(T_CustomStructType_ custom) => (T_NativeStructType_)custom;
        public static T_NativeStructType_? ToNative(T_CustomStructType_? custom) => custom.HasValue ? (T_NativeStructType_)custom.Value : null;
        public static T_CustomStructType_ ToCustom(T_NativeStructType_ native) => (T_CustomStructType_)native;
        public static T_CustomStructType_? ToCustom(T_NativeStructType_? native) => native.HasValue ? (T_CustomStructType_)native : null;
    }
    public class T_StringConverterName_ : DTOMaker.Models.IObjectConverter<T_CustomStringType_, T_NativeStringType_>
    {
        public NativeType NativeType => NativeType.String;
        public static T_NativeStringType_ ToNative(T_CustomStringType_ custom) => custom.ToString();
        public static T_CustomStringType_ ToCustom(T_NativeStringType_ native) => new T_CustomStringType_(native);
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
        T_NativeStructType_? T_NullableNativeStructMemberName_ { get; set; }
        T_NativeStructType_ T_RequiredNativeStructMemberName_ { get; set; }
        T_CustomStructType_? T_NullableCustomStructMemberName_ { get; set; }
        T_CustomStructType_ T_RequiredCustomStructMemberName_ { get; set; }
        T_MemberTypeIntfSpace_.T_MemberTypeIntfName_? T_NullableEntityMemberName_ { get; set; }
        T_MemberTypeIntfSpace_.T_MemberTypeIntfName_ T_RequiredEntityMemberName_ { get; set; }
        Octets? T_NullableBinaryMemberName_ { get; set; }
        Octets T_RequiredBinaryMemberName_ { get; set; }
        // old todo keep until feature done
        //string? T_NullableStringMemberName_ { get; set; }
        //string T_RequiredStringMemberName_ { get; set; }
        // endold
        T_NativeStringType_? T_NullableNativeStringMemberName_ { get; set; }
        T_NativeStringType_ T_RequiredNativeStringMemberName_ { get; set; }
        T_CustomStringType_? T_NullableCustomStringMemberName_ { get; set; }
        T_CustomStringType_ T_RequiredCustomStringMemberName_ { get; set; }
    }
}

