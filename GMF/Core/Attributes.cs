using System;
using Microsoft.Extensions.DependencyInjection;

namespace GMF
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ServiceDescriptorAttribute : Attribute
    {
        public ServiceLifetime Lifetime { get; }
        
        public ServiceDescriptorAttribute(ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            Lifetime = lifetime;
        }
    }
}