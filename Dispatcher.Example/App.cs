using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Dispatcher.Example
{
    class App
    {
        Dispatcher<Person> _dispatcher = new Dispatcher<Person>(ItemProcessed);

        public App()
        {
            StartNewThread();
            StartNewThread();
        }

        private  void StartNewThread()
        {
            Console.WriteLine("Starting new thread...");

            Thread t = new Thread(RunThread);
            t.IsBackground = true;
            t.Start();
        }

        private void RunThread()
        {
            Console.WriteLine(string.Format("New thread started. ThreadId={0}", Thread.CurrentThread.ManagedThreadId));

            AddItemToDispatcher("John", 4);
            AddItemToDispatcher("Joe", 5);
            AddItemToDispatcher("Jimmy", 6);
            AddItemToDispatcher("Jack", 7);
        }

        private void AddItemToDispatcher(string name, int age)
        {
            Console.WriteLine(string.Format("Adding {0}, Age: {1} to dispatcher, from thread={2}", name, age, Thread.CurrentThread.ManagedThreadId));
            _dispatcher.Add(new Person() {Name = name, Age = age});
        }

        private static void ItemProcessed(Person obj)
        {
            Console.WriteLine(string.Format("Processing person {0}, age: {1} on thread={2}", obj.Name, obj.Age, Thread.CurrentThread.ManagedThreadId));            
        }
    }
}
