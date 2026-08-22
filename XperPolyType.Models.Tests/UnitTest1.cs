
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
        private ITypeShape<T> TypeShapeOf<T>() where T : IShapeable<T>
        {
            return T.GetTypeShape();
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
        public async Task Test2()
        {
            var student = new Student()
            {
                FirstName = "John",
                LastName = "Doe",
                StudentId = "12345"
            };
            var typeShape = TypeShapeOf<Student>();
            await Verifier.Verify(typeShape);
        }
    }
}
