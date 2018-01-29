using System;
using System.Collections.Generic;
using System.Threading;

namespace Lykke.Service.EthereumClassicApi.Common.Utils
{
    internal class NewGuidContext : IDisposable
    {
        static NewGuidContext()
        {
            ThreadScopeStack = new ThreadLocal<Stack<NewGuidContext>>(() => new Stack<NewGuidContext>());
        }


        public NewGuidContext(Guid newGuid)
        {
            ContextNewGuid = newGuid;

            ThreadScopeStack.Value.Push(this);
        }

        public Guid ContextNewGuid { get; }

        public static NewGuidContext Current =>
            ThreadScopeStack.Value.Count == 0 ? null : ThreadScopeStack.Value.Peek();

        private static ThreadLocal<Stack<NewGuidContext>> ThreadScopeStack { get; }

        public void Dispose()
        {
            ThreadScopeStack.Value.Pop();
        }
    }
}
