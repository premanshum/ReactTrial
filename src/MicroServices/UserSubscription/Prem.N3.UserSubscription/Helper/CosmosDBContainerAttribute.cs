using System;

namespace Prem.N3.UserSubscription.Helper
{
    public class CosmoDBContainerAttribute : Attribute
    {
        public string Name { get; set; }

        public string PartitionKey { get; set; }
    }
}
