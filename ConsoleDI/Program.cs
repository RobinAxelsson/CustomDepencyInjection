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
            var container = new DependencyContainer();
            container.AddTransient<HelloService>();
            container.AddTransient<ServiceConsumer>();
            container.AddSingleton<MessageService>();

            var resolver = new DependencyResolver(container);
            var service1 = resolver.GetService<ServiceConsumer>();
            var service2 = resolver.GetService<ServiceConsumer>();
            var service3 = resolver.GetService<ServiceConsumer>();

            service1.Print();
            service2.Print();
            service3.Print();
            Console.ReadLine();
        }
    }
    public class Dependency
    {
        public Dependency(Type t, DependencyLifetime l)
        {
            Type = t;
            Lifetime = l;
        }
        public Type Type { get; set; }

        public void AddImplementation(object obj)
        {
            Implementation = obj;
            IsImplemented = true;
        }
        public DependencyLifetime Lifetime { get; set; }
        public object Implementation { get; private set; }
        public bool IsImplemented { get; set; }
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

            var constructor = dependency.Type.GetConstructors().Single();
            var parameters = constructor.GetParameters();
            var parameterImplemenatations = new object[parameters.Length];

            if (parameters.Length > 0)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    parameterImplemenatations[i] = GetService(parameters[i].ParameterType);
                }
                return CreateImplementation(dependency, t => Activator.CreateInstance(t, parameterImplemenatations));
            }

            return CreateImplementation(dependency, t => Activator.CreateInstance(t));
        }

        public object CreateImplementation(Dependency dependency, Func<Type, object> factory)
        {
            if (dependency.IsImplemented)
            {
                return dependency.Implementation;
            }

            var implementation = factory(dependency.Type);

            if (dependency.Lifetime == DependencyLifetime.Singleton)
            {
                dependency.AddImplementation(implementation);
            }

            return implementation;
        }

        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }
    }
    public class DependencyContainer
    {
        private readonly List<Dependency> _dependencies;
        public DependencyContainer()
        {
            _dependencies = new();
        }
        public void AddSingleton<T>()
        {
            _dependencies.Add(new Dependency(typeof(T), DependencyLifetime.Singleton));
        }

        public void AddTransient<T>()
        {
            _dependencies.Add(new Dependency(typeof(T), DependencyLifetime.Transient));
        }

        public Dependency GetDependency(Type type)
        {
            var dep = _dependencies;
            return _dependencies.First(x => x.Type.Name == type.Name);
        }

    }

}
