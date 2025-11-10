using BookNow.Application.Interfaces;
using System.Collections.Concurrent;

public class InMemoryMessageBus : IMessageBus
{
    private readonly ConcurrentDictionary<Type, List<Func<object, Task>>> _handlers = new();

    public Task PublishAsync<TEvent>(TEvent @event)
    {
        if (_handlers.TryGetValue(typeof(TEvent), out var handlers))
        {
            var tasks = handlers.Select(h => h(@event));
            return Task.WhenAll(tasks);
        }
        return Task.CompletedTask;
    }

    public void Subscribe<TEvent>(Func<TEvent, Task> handler)
    {
        var handlers = _handlers.GetOrAdd(typeof(TEvent), _ => new List<Func<object, Task>>());
        handlers.Add(evt => handler((TEvent)evt));
    }
}
