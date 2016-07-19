# TOC

- [Pairs Generator](#pairsgenerator)
  - [Logging](#logging)
  - [Cancel process](#cancel-process)
  - [Options](#options)
- [League Fixtures Generator](#leaguefixturesgenerator)

## PairsGenerator

This is a simple generic class and you need to provide the type of elements it will combine (i.e.: strings, guids, integers, etc.).

The main method `GetFixtures()` combines input elements into pairs and **returns sets of combined pairs**.

It uses a *backtracking* algorithm located in the base abstract class.

Because it can take a long time processing a big number of elements, **it's advised to run it asynchronously in background**. 
You'll benefit from being able to do some other work in your code while waiting for the result.

It always takes a cancellation token to allow canceling it prematurely while running if needed.

Example:

```csharp
using FixturesGenerator.Base;

public async Task<FixtureSets<string>> GetFixtures()
{
    // some work...

    return await Task.Run(() => 
        generator.GetFixtures(elements, cancellationToken));
}
```

### Logging

The process logs some debug info, you just need to provide a logger so you can easily follow the process' steps out in the console or wherever you set.

```csharp
using FixturesGenerator.Pairs;
using Microsoft.Extensions.Logging;

var loggerFactory = new LoggerFactory();
loggerFactory.AddConsole();
var logger = loggerFactory.CreateLogger<PairsGenerator<string>>();

var generator = new PairsGenerator<string>(logger);
```

[top](#toc)

### Cancel Process

Use a `CancellationToken` to be able to cancel like any other long-time consuming process.

```csharp
using FixturesGenerator.Base;
using System.Threading; 

var cancellationTokenSource = new CancellationTokenSource();
var result = generator.GetFixtures(elements, cancellationTokenSource.Token);

// Finish work prematurely
cancellationTokenSource.Cancel();
```

**Note**: When canceling the process, the result will always be an empty collection. 

[top](#toc)

### Options

You can call `Configure()` with some options like this:

```csharp
using FixturesGenerator.Pairs;

generator.Configure(new PairsGeneratorSettings(shuffleItems: false, includeInvertedPairs: true));
```

[top](#toc)

#### Shuffle Items

Set the option `ShuffleItems = true` if you want to shuffle before combining them. Sometimes you might need the results ordered and some other times you might need them in random order.

#### Include Inverted Pairs

Set the option `IncludeInvertedPairs = true` to include the inverted (*mirrors*) pairs along with the result.

This is useful when asking for the fixtures for the second leg of a competition, when you need to include the fixtures where the *home* teams play later as *away* teams.

```csharp
using FixturesGenerator.Base;
using FixturesGenerator.Pairs;

char[] elements = new char[4] { 'A', 'B', 'C', 'D' };

var generator = new PairsGenerator<char>();
generator.Configure(new PairsGeneratorSettings(false, true));

var roundFixtures = generator.GetFixtures(elements, CancellationToken.None);

// output
// [
//     [ {'A', 'B'}, {'C', 'D'} ],
//     [ {'A', 'C'}, {'B', 'D'} ],
//     [ {'A', 'D'}, {'B', 'C'} ],
//     mirrored pairs also included in the result
//     [ {'B', 'A'}, {'D', 'C'} ],
//     [ {'C', 'A'}, {'D', 'B'} ],
//     [ {'D', 'A'}, {'C', 'B'} ]
// ]
```

[top](#toc)

## LeagueFixturesGenerator

This generator is more close to what sports leagues would generate.
It shuffles the elements to give random fixtures each time and includes first leg and second leg fixtures in the result.

Also, tries to switch elements position between sets, which gives as result an element as primary in one set, secondary in the following set, primary again in the following set, secondary again in the following set, etc.

With this, it solves the problem of having a team playing home one round, away in the following round, home in the following, away in the following, etc.

The use is the same shown in the `PairsGenerator` class.

Example:

```csharp
using FixturesGenerator.Base;
using FixturesGenerator.League;
using System.Threading;
using System.Threading.Tasks;

public void async Task<FixtureSets<string>> GenerateLeagueFixturesAsync(IEnumerable<string> teams)
{
    var generator = new LeagueFixturesGenerator<string>();
    return await Task.Run(() => generator.GetFixtures(teams, CancellationToken.None));
}
```

**Advise:** When the number of elements to combine is high, it is recommended to use an asynchronous pattern, like shown above, to run the process in background.
Your application can terminate the background thread if needed by using the [cancellation token](#cancel-process).  

[top](#toc)
