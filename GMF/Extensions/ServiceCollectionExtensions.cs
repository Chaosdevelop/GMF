using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GMF.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddServicesWithAttribute(this IServiceCollection services)
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies)
			{
				var typesWithAttributes = assembly.GetTypes()
					.Where(type => CustomAttributeExtensions.GetCustomAttribute<ServiceDescriptorAttribute>((MemberInfo)type) != null);

				foreach (var type in typesWithAttributes)
				{
					var attribute = type.GetCustomAttribute<ServiceDescriptorAttribute>();
					if (attribute != null)
					{

						var lifetime = attribute?.Lifetime ?? ServiceLifetime.Transient;
						var bindtype = attribute.BindType;
						services.Add(new ServiceDescriptor(bindtype, type, lifetime));
					}

				}
			}

			return services;
		}
	}
}