# Fixtures Generator

This library project made in C# is mainly used for **generation of league fixtures** providing **sports applications** an automated way of generating them.

## Installation

Install the [nuget package](https://www.nuget.org/packages/FixturesGenerator/) in your project.

    Install-Package FixturesGenerator

## Basic Usage

```csharp
    using System;
    using FixturesGenerator.Generators;

    public class MyClass
    {
        public void Run()
        {
            const byte numElements = 4;
            string[] elements = new string[numElements] { "Team1", "Team2", "Team3", "Team4" };
            
            LeagueFixturesGenerator fixturesGenerator = new LeagueFixturesGenerator();
            int[,,] fixturesIndexes = fixturesGenerator.GenerateFixtures(numElements);
            // 3D array accessed: [round, match, local/visitor]

            // Print first match
            int localTeamIndex = fixturesIndexes[0, 0, 0];   
            int visitorTeamIndex = fixturesIndexes[0, 0, 1];
            Console.WriteLine($"{elements[localTeamIndex]} vs {elements[visitorTeamIndex]}");

            // Print second match
            localTeamIndex = fixturesIndexes[0, 1, 0];
            visitorTeamIndex = fixturesIndexes[0, 1, 1];
            Console.WriteLine($"{elements[localTeamIndex]} vs {elements[visitorTeamIndex]}");

            // Print 3rd round, 2nd match
            localTeamIndex = fixturesIndexes[2, 1, 0];
            visitorTeamIndex = fixturesIndexes[2, 1, 1];
            Console.WriteLine($"{elements[localTeamIndex]} vs {elements[visitorTeamIndex]}");
        }
    } 
````
## Documentation

Read more documentation [here](docs/Documentation.md)

## Contributing

Please read the [contribution guidelines](docs/Contributing.md)

## License

[MIT license](LICENSE.md)