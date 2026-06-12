# BusinessDays.Net

[![CI](https://github.com/shawnfeds/BusinessDays.Net/actions/workflows/ci.yml/badge.svg)](https://github.com/shawnfeds/BusinessDays.Net/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/BusinessDays.Net.svg)](https://www.nuget.org/packages/BusinessDays.Net)
[![NuGet Downloads](https://img.shields.io/nuget/dt/BusinessDays.Net.svg)](https://www.nuget.org/packages/BusinessDays.Net)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Lightweight, zero-dependency business day calculator for .NET.  
Supports configurable working weeks, pluggable holiday providers, and payroll/SLA use cases.

---

## Features

- **Configurable working week** — Mon–Fri by default, but supports any combination (e.g. Sun–Thu for Middle Eastern markets)
- **Pluggable holiday providers** — bring your own: static lists, Nager.Date, a database, anything that implements `IHolidayProvider`
- **Zero allocations** on all core operations — see benchmarks below
- **Zero external dependencies**
- **Targets `netstandard2.0`** — compatible with .NET Framework 4.6.1+, .NET Core, and .NET 5+

---

## Installation

```shell
dotnet add package BusinessDays.Net
```

---

## Quick Start

```csharp
// Default Mon–Fri calendar, no holidays
var calendar = new BusinessCalendar();

// Check if a date is a business day
bool isBusinessDay = calendar.IsBusinessDay(DateTime.Today);

// Add business days
DateTime deadline = calendar.AddBusinessDays(DateTime.Today, 5);

// Count business days between two dates
int days = calendar.BusinessDaysBetween(startDate, endDate);

// Next / previous business day
DateTime next = calendar.NextBusinessDay(DateTime.Today);
DateTime prev = calendar.PreviousBusinessDay(DateTime.Today);
```

---

## Custom Working Week

```csharp
// Sun–Thu working week (e.g. Middle East markets)
var calendar = new BusinessCalendar(new[]
{
    DayOfWeek.Sunday,
    DayOfWeek.Monday,
    DayOfWeek.Tuesday,
    DayOfWeek.Wednesday,
    DayOfWeek.Thursday
});
```

---

## Pluggable Holiday Providers

Implement `IHolidayProvider` to plug in any holiday source:

```csharp
public class MyHolidayProvider : IHolidayProvider
{
    public IEnumerable<DateTime> GetHolidays(int year)
    {
        // Return from a database, static list, Nager.Date, etc.
        return new[]
        {
            new DateTime(year, 1, 1),   // New Year's Day
            new DateTime(year, 12, 25)  // Christmas
        };
    }
}

var calendar = new BusinessCalendar(new MyHolidayProvider());
```

Holidays are cached per year internally — `GetHolidays` is called at most once per year regardless of how many operations you perform.

---

## Payroll and Billing

```csharp
// First business day of the month
DateTime firstBusinessDay = calendar.NthBusinessDayOfMonth(2024, 6, 1);

// Third Friday of the month that is also a business day
DateTime payDay = calendar.NthBusinessWeekdayOfMonth(2024, 6, 3, DayOfWeek.Friday);
```

---

## Benchmarks

Benchmarked with [BenchmarkDotNet](https://benchmarkdotnet.org) on:
- Intel Core 5 210H 2.20GHz, .NET 10.0.8, Windows 11

| Operation                        | Mean      | Allocated |
|--------------------------------- |----------:|----------:|
| IsBusinessDay                    |   16 ns   |         - |
| NextBusinessDay                  |   18 ns   |         - |
| PreviousBusinessDay              |   23 ns   |         - |
| AddBusinessDays (5 days)         |   89 ns   |         - |
| AddBusinessDays (260 days)       | 4,668 ns  |         - |
| BusinessDaysBetween (1 year)     | 4,736 ns  |         - |
| BusinessDaysBetween (10 years)   | 56,566 ns |         - |

**Zero heap allocations on every operation.**

---

## Design Decisions

**Why `netstandard2.0`?**  
Enterprise codebases running .NET Framework 4.6.1+ can consume this package without upgrading their runtime. Modern .NET projects work equally well.

**Why no built-in holiday data?**  
Holiday rules differ by country, region, and organisation. Baking in data means baking in assumptions. Inject what you need — wire in [Nager.Date](https://github.com/nager/Nager.Date) if you want a full country holiday database.

**Why `IHolidayProvider` instead of `IEnumerable<DateTime>`?**  
A raw list forces the caller to know all years upfront. A provider is called lazily per year, which supports dynamic sources like databases without loading decades of data eagerly.

---

## Compared to Alternatives

| Feature | BusinessDays.Net | PublicHoliday | Nager.Date |
|---|---|---|---|
| Configurable working week | ✅ | ❌ | ❌ |
| Pluggable holiday provider | ✅ | ❌ | N/A |
| Zero dependencies | ✅ | ✅ | ❌ |
| Built-in country holidays | ❌ (by design) | ✅ | ✅ |
| `netstandard2.0` | ✅ | ❌ | ✅ |
| Zero allocations | ✅ | ❌ | ❌ |

---

## License

MIT — see [LICENSE](LICENSE)
