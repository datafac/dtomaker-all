
using PolyType.Examples.JsonSerializer;
using PolyType.Examples.XmlSerializer;
using PolyType.Examples.Cloner;
using System.Text.Json.Serialization;
using Xunit;
using XperPolyType.Models;
using PolyType;
using System.Text;

using VerifyXunit;
using System.Threading.Tasks;
using VerifyTests;

namespace XperPolyType.Models.Tests
{
    public class UnitTest1
    {
        private ITypeShape<T> TypeShapeOf<T>(T shape) where T : IShapeable<T>
        {
            return T.GetTypeShape();
        }

        private string DumpShape<T>(T shape) where T : IShapeable<T>
        {
            // do something with shape
            ITypeShape<T> typeShape = T.GetTypeShape();
            StringBuilder result = new StringBuilder();
            result.AppendLineN($"Type   : {typeShape.Type.FullName}");
            result.AppendLineN($"Kind   : {typeShape.Kind}");
            result.AppendLineN($"Methods: {typeShape.Methods.Count}");
            foreach (var method in typeShape.Methods)
            {
                result.AppendLineN($"       - {method.Name}");
            }
            result.AppendLineN($"Events : {typeShape.Events.Count}");
            foreach (var evt in typeShape.Events)
            {
                result.AppendLineN($"       - {evt.Name}");
            }
            return result.ToString();
        }

        [Fact]
        public async Task CheckVerifySetup()
        {
            await VerifyChecks.Run();
        }

        [Fact]
        public async Task Test0()
        {
            var student = new Student()
            {
                FirstName = "John",
                LastName = "Doe",
                StudentId = "12345"
            };
            await Verifier.Verify(student);
        }

        [Fact]
        public async Task Test1()
        {
            var student = new Student()
            {
                FirstName = "John",
                LastName = "Doe",
                StudentId = "12345"
            };
            string shape = DumpShape(student);
            await Verifier.Verify(shape);
        }

        [Fact]
        public async Task Test2()
        {
            var student = new Student()
            {
                FirstName = "John",
                LastName = "Doe",
                StudentId = "12345"
            };
            var typeShape = TypeShapeOf(student);
            await Verifier.Verify(typeShape);
        }
    }
}
