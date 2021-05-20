using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDI
{
    public enum DependencyLifetime
    {
        Singleton = 0,
        Transient = 1
    }
    public class ServiceConsumer
    {
        private HelloService _hello;

        public ServiceConsumer(HelloService hello)
        {
            _hello = hello;
        }
        public void Print() => _hello.Print();
    }

    public class HelloService
    {
        private MessageService _message;
        public HelloService(MessageService message)
        {
            _message = message;
        }
        public void Print()
        {
            Console.WriteLine("Hello World : " + _message.Message());
        }
    }

    public class MessageService
    {
        private int _random;

        public MessageService()
        {
            _random = new Random().Next(1000);
        }
        public string Message()
        {
            return $"Yo #{_random}";
        }
    }
    public interface ITest
    {
        public void Print();
    }

    public class Test : ITest
    {
        public Test()
        {
            Print();
        }
        public void Print()
        {
            System.Diagnostics.Debug.WriteLine("Hello from test");
        }
    }
}
