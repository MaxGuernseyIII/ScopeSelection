# Scope Selection

A simple, portable library to make selection of subsets easy and abstract.

## The Problem

When accessing a semi-hierarchical collection of items, you often want an easy way to tell if an element is in a particular subset.

Consider the following data class to support execution of Cucumber style clauses:

```csharp
public class Clause
{
	public string Feature { get; init; } = "";
	public string Scenario { get; init; } = "";
	public string Keyword { get; init; } = "";
	public IReadOnlyList<string> Tags { get; init; } = [];

	public string Payload { get; set; } = "";
}
```

Compare that with different ways it may be bound:

```csharp
public class StepBinding
{
	public string Feature { get; init; } = "";
	public string Scenario { get; init; } = "";
	public string Keyword { get; init; } = "";
	public IReadOnlyList<string> Tags { get; init; } = [];

	public required IBinding { get; set; }
}

public class StepArgumentTransformationBinding
{
	public string Feature { get; init; } = "";
	public string Scenario { get; init; } = "";
	public string Keyword { get; init; } = "";
	public IReadOnlyList<string> Tags { get; init; } = [];

	public required ITransformation { get; set; }
}
```

Such structuring proliferates and promotes redundant query logic.

## The Solution

This can be solved by introducing an abstraction that encapsulates redundant structure.

```csharp
public class Clause
{
	public required Scope { get; init }

	public string Payload { get; set; } = "";
}

public class StepBinding
{
	public required Scope { get; init }

	public required IBinding { get; set; }
}

public class StepArgumentTransformationBinding
{
	public required Scope { get; init }

	public required ITransformation { get; set; }
}
```

Encapsulating redundant structure provides an opportunity to encapsulate redundant logic, too. Instead of sophisticated logic being repeated, you can hide them behind a single method you use in a `where` LINQ operation.

```csharp
var Bindings = AllBindings.Where(B => Operation.Scope.IsSatisfiedBy(B.Scope));
```

At its core, this package provides the abstraction to solve this problem, but it goes a little further.

## Built-In Scope Types

While the fundamental purpose of the library is to provide the contract, this package also provides two built-in types of scope:

1. A scope that operates by supply and demand of tokens
1. A scope that combines two other scopes into a multidimensional scope

Later, an hierarchical built-in may be added.

## Supply and Demand

A supply and demand scope space involves explicit declarations involving tokens. These tokens can be of any type.

Following is an example of use.

```csharp
  [TestMethod]
  public void SelectItem()
  {
    var ScopeSpace = ScopeSpaces.SupplyAndDemand<ClauseType>();
    var BindingScope = ScopeSpace.Supply(Given);

    var Included = ScopeSpace.Demand(Given).IsSatisfiedBy(BindingScope);

    Included.ShouldBeTrue();
  }

  [TestMethod]
  public void FailToSelectItem()
  {
    var ScopeSpace = ScopeSpaces.SupplyAndDemand<ClauseType>();
    var BindingScope = ScopeSpace.Supply([Given, Then]);

    var Included = ScopeSpace.Demand(When).IsSatisfiedBy(BindingScope);

    Included.ShouldBeFalse();
  }
```

## Composite

A composite scope space is multi-dimensional.

```csharp
  [TestMethod]
  public void SelectItemInCompositeSpace()
  {
    var ClauseTypes = ScopeSpaces.SupplyAndDemand<ClauseType>();
    var Features = ScopeSpaces.SupplyAndDemand<string>();
    var CompositeSpace = ScopeSpaces.Composite(ClauseTypes, Features);
    var BindingScope = CompositeSpace.Combine(ClauseTypes.Any, Features.Supply("Scope Resolution"));

    var Included = CompositeSpace.Combine(ClauseTypes.Demand(Given), Features.Any).IsSatisfiedBy(BindingScope);

    Included.ShouldBeTrue();
  }

  [TestMethod]
  public void FailCompositeSelectionDueToOneDimension()
  {
    var ClauseTypes = ScopeSpaces.SupplyAndDemand<ClauseType>();
    var Features = ScopeSpaces.SupplyAndDemand<string>();
    var CompositeSpace = ScopeSpaces.Composite(ClauseTypes, Features);
    var BindingScope = CompositeSpace.Combine(ClauseTypes.Unspecified, Features.Supply("Scope Resolution"));

    var Included = CompositeSpace.Combine(ClauseTypes.Demand(Given), Features.Any).IsSatisfiedBy(BindingScope);

    Included.ShouldBeFalse();
  }
```

It does not matter the two underlying dimensions are. You can even have two dimensions from the exact same type of space.