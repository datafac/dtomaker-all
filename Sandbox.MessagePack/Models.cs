using System;
using DTOMaker.Models;
using DTOMaker.Runtime;
namespace MyOrg.Models
{
    [Entity]
    [Id(1)]
    public interface IMyDTO : IEntityBase
    {
        [Member(2)] IMyDTO? Field1 { get; set; }
    }
}
