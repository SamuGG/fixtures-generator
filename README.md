# Fixtures Generator

This library project made in C# is mainly used for **generation of league fixtures** providing **sports applications** an easier way to generate them.

Although it can be used to get combinations of elements, returned in a collection or pair sets.

## How To Use It

Given the following elements: Team1, Team2, Team3, Team4; return a collection of pair-sets:

```csharp
string[] elements = new string[4] { "Team1", "Team2", "Team3", "Team4" };
var generator = new LeagueFixturesGenerator<string>();
var output = generator.GetFixtures(elements, CancellationToken.None);

// output
// [
//   [ {"Team4", "Team3"}, {"Team1", "Team2"} ],
//   [ {"Team1", "Team4"}, {"Team2", "Team3"} ],
//   [ {"Team4", "Team2"}, {"Team3", "Team1"} ],
//   [ {"Team3", "Team4"}, {"Team2", "Team1"} ],
//   [ {"Team4", "Team1"}, {"Team3", "Team2"} ],
//   [ {"Team1", "Team3"}, {"Team2", "Team4"} ]
// ]
```

See the [documentation](docs/Documentation.md) to know how to use it.

## Contributing

Please read the [contribution guidelines](docs/Contributing.md)
