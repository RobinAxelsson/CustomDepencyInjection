using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.Intrinsics.Arm;

namespace ConsoleDI
{
    //https://youtu.be/NkTF_6IQPiY?t=599
    public class Program
    {
        public static void Main(string[] args)
        {
            var ctorParams = typeof(ServiceConsumer).GetConstructors()
                .Select(x => x.GetParameters());
            var container = new DependencyContainer();
            container.AddDependency<HelloService>();
            container.AddDependency<ServiceConsumer>();
            container.AddDependency<MessageService>();

            var resolver = new DependencyResolver(container);
            var service1 = resolver.GetService<ServiceConsumer>();
            var service2 = resolver.GetService<ServiceConsumer>();
            var service3 = resolver.GetService<ServiceConsumer>();

            service1.Print();
            service2.Print();
            service3.Print();

        }
    }
    public class Dependency
    {
        public Type Type { get; set; }
        public DependencyLifetime Lifetime { get; set; }
    }
    public class DependencyResolver
    {
        private DependencyContainer _container;
        public DependencyResolver(DependencyContainer container)
        {
            _container = container;
        }

        private object GetService(Type type)
        {
            var dependency = _container.GetDependency(type);
            var constructor = dependency.GetConstructors().Single();
            var parameters = constructor.GetParameters();
            var parameterImplemenatations = new object[parameters.Length];

            if (parameters.Length > 0)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    parameterImplemenatations[i] = GetService(parameters[i].ParameterType);
                }

                return Activator.CreateInstance(dependency, parameterImplemenatations);
            }

            return Activator.CreateInstance(dependency);
        }

        public T GetService<T>()
        {
            return (T) GetService(typeof(T));
        }
    }
    public class DependencyContainer
    {
        private readonly List<Type> _dependencies = new();
        public void AddDependency(Type type) => _dependencies.Add(type);
        public void AddDependency<T>() => _dependencies.Add(typeof(T));
        public Type GetDependency(Type type) => _dependencies.First(x => x.Name == type.Name);
    } 
    
}
