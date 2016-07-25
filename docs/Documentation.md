## Long-Running Tasks

Beware that for some high number of elements, calling the method `GenerateFixtures()` can become a long-running task. 
It's important to know how your application can deal with it accordingly.

### Async Tasks

If your application needs to do some work while a process takes a long time then, you should consider running the time-consuming process in another thread, like this:

```csharp
async Task<int[,,]> GenerateFixturesAsync(byte numElements, IFixturesGenerator fixturesGenerator)
{
    return await Task.Run<int[,,]>(() => fixturesGenerator.GenerateFixtures(numElements));
}
```

### Cancelling Tasks

The proper way of cancelling a long-running process is using a `CancellationToken`. 
If your application needs to finish the process prematurely then, you should call the constructor using a cancellation token, like this:

```csharp
CancellationTokenSource source = new CancellationTokenSource();
CancellationToken token = source.Token;
LeagueFixturesGenerator fixturesGenerator = new LeagueFixturesGenerator(token);

// start long-running task
fixturesGenerator.GenerateFixtures(LeagueFixturesGenerator.MaxNumElements);

// ...time out
token.Cancel();
```

Calling `token.Cancel()` will finish the process and your application can continue.