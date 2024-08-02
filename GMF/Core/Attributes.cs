using System;
using Microsoft.Extensions.DependencyInjection;

namespace GMF
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class ServiceDescriptorAttribute : Attribute
	{
		public ServiceLifetime Lifetime { get; }
		public Type BindType { get; }
		public ServiceDescriptorAttribute(Type bindType, ServiceLifetime lifetime = ServiceLifetime.Transient)
		{
			Lifetime = lifetime;
			BindType = bindType;
		}
	}
}