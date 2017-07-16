using System;
using Swagger_Slayer;

namespace Sample
{
    public class RequestModel
    {
        [PropertyRequired]
        public Guid ResourceId { get; set; }
    }
}
