
using PolyType.Examples.JsonSerializer;
using PolyType.Examples.XmlSerializer;
using System.Text.Json.Serialization;
using Xunit;
using XperPolyType.Models;

namespace XperPolyType.Models.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            // todo try serializing Student
            var student = new Student()
            {
                FirstName = "John",
                LastName = "Doe",
                StudentId = "12345"
            };

            //string json = JsonSerializerTS.Serialize(student);
            //string xml = XmlSerializer.Serialize(student);
        }
    }
}
