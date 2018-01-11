using System;
using System.Collections.Generic;
using System.Threading;

namespace Lykke.Service.EthereumClassic.Api.Common.Utils
{
    internal class UtcNowContext
    {
        static UtcNowContext()
        {
            ThreadScopeStack = new ThreadLocal<Stack<UtcNowContext>>(() => new Stack<UtcNowContext>());
        }

        public static UtcNowContext Current =>
            ThreadScopeStack.Value.Count == 0 ? null : ThreadScopeStack.Value.Peek();

        private static ThreadLocal<Stack<UtcNowContext>> ThreadScopeStack { get; }



        public UtcNowContext(DateTime utcNow)
        {
            ContextUtcNow = utcNow;

            ThreadScopeStack.Value.Push(this);
        }

        public DateTime ContextUtcNow { get; }

        public void Dispose()
        {
            ThreadScopeStack.Value.Pop();
        }
    }
}