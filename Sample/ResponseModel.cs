using System;

namespace Sample
{
    public class ResponseModel
    {
        public string ResourceName { get; set; }
        public Guid ResourceId { get; set; }
        public NetstedClass SomeNetstedClass { get; set; }
    }

    public class NetstedClass
    {
        public string Description { get; set; }
    }
}
