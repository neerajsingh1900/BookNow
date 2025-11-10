using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.Interfaces
{
    public interface IMessageBus
    {
        Task PublishAsync<TEvent>(TEvent @event);
        void Subscribe<TEvent>(Func<TEvent, Task> handler);
    }
}
