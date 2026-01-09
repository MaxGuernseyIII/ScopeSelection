// MIT License
// 
// Copyright (c) 2026-2026 Producore LLC
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace Specifications;

[TestClass]
public class SupplyAndDemandScopeBehaviors
{
  static readonly SupplyAndDemandScope<string>.Space StringTokenSpace = ScopeSpaces.SupplyAndDemand<string>();
  SupplyAndDemandScope<MockToken>.Space ScopeSpace = null!;

  [TestInitialize]
  public void Setup()
  {
    ScopeSpace = ScopeSpaces.SupplyAndDemand<MockToken>();
  }

  [TestMethod]
  public void AnyIncludesItself()
  {
    ScopeSpace.Any.IsSatisfiedBy(ScopeSpace.Any).ShouldBeTrue();
  }

  [TestMethod]
  public void AnyIncludesToken()
  {
    ScopeSpace.Any.IsSatisfiedBy(ScopeSpace.For(new())).ShouldBeTrue();
  }

  [TestMethod]
  public void AnyIncludesUnspecified()
  {
    ScopeSpace.Any.IsSatisfiedBy(ScopeSpace.Unspecified).ShouldBeTrue();
  }

  [TestMethod]
  public void RequireIncludesAny()
  {
    ScopeSpace.Demand(new MockToken()).IsSatisfiedBy(ScopeSpace.Any).ShouldBeTrue();
  }

  [TestMethod]
  public void RequireIncludesToken()
  {
    var Token = new MockToken();
    ScopeSpace.Demand(Token).IsSatisfiedBy(ScopeSpace.For(Token)).ShouldBeTrue();
  }

  [TestMethod]
  public void RequireIncludesSameSupply()
  {
    var Token = new MockToken();
    ScopeSpace.Demand(Token).IsSatisfiedBy(ScopeSpace.Supply(Token)).ShouldBeTrue();
  }

  [TestMethod]
  public void RequireDoesNotIncludeOtherToken()
  {
    ScopeSpace.Demand(new MockToken()).IsSatisfiedBy(ScopeSpace.For(new())).ShouldBeFalse();
  }

  [TestMethod]
  public void RequireDoesNotIncludeOtherSupply()
  {
    ScopeSpace.Demand(new MockToken()).IsSatisfiedBy(ScopeSpace.Supply(new MockToken())).ShouldBeFalse();
  }

  [TestMethod]
  public void RequireDoesNotIncludeUnspecified()
  {
    ScopeSpace.Demand(new MockToken()).IsSatisfiedBy(ScopeSpace.Unspecified).ShouldBeFalse();
  }

  [TestMethod]
  public void UnspecifiedIncludesItself()
  {
    ScopeSpace.Unspecified.IsSatisfiedBy(ScopeSpace.Unspecified).ShouldBeTrue();
  }

  [TestMethod]
  public void UnspecifiedIncludesToken()
  {
    ScopeSpace.Unspecified.IsSatisfiedBy(ScopeSpace.For(new())).ShouldBeTrue();
  }

  [TestMethod]
  public void UnspecifiedIncludesAny()
  {
    ScopeSpace.Unspecified.IsSatisfiedBy(ScopeSpace.Any).ShouldBeTrue();
  }

  [TestMethod]
  public void TokenIncludesAny()
  {
    ScopeSpace.For(new()).IsSatisfiedBy(ScopeSpace.Any).ShouldBeTrue();
  }

  [TestMethod]
  public void TokenIncludesSameToken()
  {
    var Token = new MockToken();
    ScopeSpace.For(Token).IsSatisfiedBy(ScopeSpace.For(Token)).ShouldBeTrue();
  }

  [TestMethod]
  public void TokenDoesNotIncludeOtherToken()
  {
    ScopeSpace.For(new()).IsSatisfiedBy(ScopeSpace.For(new())).ShouldBeFalse();
  }

  [TestMethod]
  public void TokenDoesNotIncludeUnspecified()
  {
    ScopeSpace.For(new()).IsSatisfiedBy(ScopeSpace.Unspecified).ShouldBeFalse();
  }

  [TestMethod]
  public void AndWhenBothDemandsAreSupplied()
  {
    MockToken Token1 = new();
    MockToken Token2 = new();

    ScopeSpace.Intersection(ScopeSpace.Demand(Token1), ScopeSpace.Demand(Token2))
      .IsSatisfiedBy(ScopeSpace.Union(ScopeSpace.Supply(Token1), ScopeSpace.Supply(Token2))).ShouldBeTrue();
  }

  [TestMethod]
  public void AndWhenLeftDemandSupplied()
  {
    MockToken Token1 = new();
    MockToken Token2 = new();

    ScopeSpace.Intersection(ScopeSpace.Demand(Token1), ScopeSpace.Demand(Token2))
      .IsSatisfiedBy(ScopeSpace.Supply(Token1)).ShouldBeFalse();
  }

  [TestMethod]
  public void AndWhenRightDemandSupplied()
  {
    MockToken Token1 = new();
    MockToken Token2 = new();

    ScopeSpace.Intersection(ScopeSpace.Demand(Token1), ScopeSpace.Demand(Token2))
      .IsSatisfiedBy(ScopeSpace.Supply(Token2)).ShouldBeFalse();
  }

  [TestMethod]
  public void AndWhenNoDemandsAreSupplied()
  {
    MockToken Token1 = new();
    MockToken Token2 = new();

    ScopeSpace.Intersection(ScopeSpace.Demand(Token1), ScopeSpace.Demand(Token2))
      .IsSatisfiedBy(ScopeSpace.Unspecified).ShouldBeFalse();
  }

  [TestMethod]
  public void AndWhenSuppliesOverlap()
  {
    var Scope1 = ScopeSpace.For(new());
    var Scope2 = ScopeSpace.For(new());
    var Scope3 = ScopeSpace.For(new());

    var Intersection = ScopeSpace.Intersection(ScopeSpace.Union(Scope1, Scope2), ScopeSpace.Union(Scope2, Scope3));

    Scope1.IsSatisfiedBy(Intersection).ShouldBeFalse();
    Intersection.IsSatisfiedBy(Scope1).ShouldBeFalse();
    Scope2.IsSatisfiedBy(Intersection).ShouldBeTrue();
    Intersection.IsSatisfiedBy(Scope2).ShouldBeTrue();
    Scope3.IsSatisfiedBy(Intersection).ShouldBeFalse();
    Intersection.IsSatisfiedBy(Scope3).ShouldBeFalse();
  }

  [TestMethod]
  public void OrWhenBothDemandsAreSupplied()
  {
    MockToken Token1 = new();
    MockToken Token2 = new();

    ScopeSpace.Union(ScopeSpace.Demand(Token1), ScopeSpace.Demand(Token2))
      .IsSatisfiedBy(ScopeSpace.Union(ScopeSpace.Supply(Token1), ScopeSpace.Supply(Token2))).ShouldBeTrue();
  }

  [TestMethod]
  public void OrWhenLeftDemandSupplied()
  {
    MockToken Token1 = new();
    MockToken Token2 = new();

    ScopeSpace.Union(ScopeSpace.Demand(Token1), ScopeSpace.Demand(Token2))
      .IsSatisfiedBy(ScopeSpace.Supply(Token1)).ShouldBeTrue();
  }

  [TestMethod]
  public void OrWhenRightDemandSupplied()
  {
    MockToken Token1 = new();
    MockToken Token2 = new();

    ScopeSpace.Union(ScopeSpace.Demand(Token1), ScopeSpace.Demand(Token2))
      .IsSatisfiedBy(ScopeSpace.Supply(Token2)).ShouldBeTrue();
  }

  [TestMethod]
  public void OrWhenNoDemandsAreSupplied()
  {
    MockToken Token1 = new();
    MockToken Token2 = new();

    ScopeSpace.Union(ScopeSpace.Demand(Token1), ScopeSpace.Demand(Token2))
      .IsSatisfiedBy(ScopeSpace.Unspecified).ShouldBeFalse();
  }

  [TestMethod]
  public void IsDistinct()
  {
    var Space = ScopeSpaces.SupplyAndDemand<MockToken>();

    Space.ShouldBeAssignableTo<DistinctSpaceScope<SupplyAndDemandScope<MockToken>.Space,
      SupplyAndDemandScope<MockToken>>.DistinctSpace>();
  }

  [TestMethod]
  public void DemandProducesSameMementoAsDemand()
  {
    RoundTripEquivalentScopesScenario(Space =>
    {
      var Token = Any.String();

      return (Space.Demand(Token), Space.Demand(Token));
    });
  }

  [TestMethod]
  public void SupplyProducesSameMementoAsSupply()
  {
    RoundTripEquivalentScopesScenario(Space =>
    {
      var Token = Any.String();
      return (Space.Supply(Token), Space.Supply(Token));
    });
  }

  [TestMethod]
  public void ForProducesSameMementoAsFor()
  {
    RoundTripEquivalentScopesScenario(Space =>
    {
      var Token = Any.String();
      return (Space.For(Token), Space.For(Token));
    });
  }

  [TestMethod]
  public void DemandProducesDifferentMementoFromSupply()
  {
    RoundTripNonEquivalentScopesScenario(Space =>
    {
      var Token = Any.String();
      return (Space.Demand(Token), Space.Supply(Token));
    });
  }

  [TestMethod]
  public void DemandProducesDifferentMementoFromFor()
  {
    RoundTripNonEquivalentScopesScenario(Space =>
    {
      var Token = Any.String();
      return (Space.Demand(Token), Space.For(Token));
    });
  }

  [TestMethod]
  public void DemandDifferentForDifferentTokens()
  {
    RoundTripNonEquivalentScopesScenario(Space =>
    {
      var Token = Any.String();
      return (Space.Demand(Token), Space.Demand(Any.String()));
    });
  }

  [TestMethod]
  public void SupplyProducesDifferentMementoFromFor()
  {
    RoundTripNonEquivalentScopesScenario(Space =>
    {
      var Token = Any.String();
      return (Space.Supply(Token), Space.For(Token));
    });
  }

  [TestMethod]
  public void SupplyDifferentForDifferentTokens()
  {
    RoundTripNonEquivalentScopesScenario(Space =>
    {
      var Token = Any.String();
      return (Space.Supply(Token), Space.Supply(Any.String()));
    });
  }

  [TestMethod]
  public void ForDifferentForDifferentTokens()
  {
    RoundTripNonEquivalentScopesScenario(Space =>
    {
      var Token = Any.String();
      return (Space.For(Token), Space.For(Any.String()));
    });
  }

  [TestMethod]
  public void AnyRoundTrip()
  {
    var ScopeSpace = ScopeSpaces.SupplyAndDemand<string>();

    var Memento = ScopeSpace.Any.GetMemento();

    ScopeSpace.FromMemento(Memento).ShouldBeSameAs(ScopeSpace.Any);
  }

  [TestMethod]
  public void UnspecifiedRoundTrip()
  {
    var ScopeSpace = ScopeSpaces.SupplyAndDemand<string>();

    var Memento = ScopeSpace.Unspecified.GetMemento();

    ScopeSpace.FromMemento(Memento).ShouldBeSameAs(ScopeSpace.Unspecified);
  }

  [DynamicData(nameof(DistinctScopes))]
  [TestMethod]
  public void RoundTripEquivalence(SupplyAndDemandScope<string> Scope)
  {
    var OriginalMemento = Scope.GetMemento();

    var RoundTrippedMemento = StringTokenSpace.FromMemento(OriginalMemento).GetMemento();

    RoundTrippedMemento.GetRawText().ShouldBe(OriginalMemento.GetRawText());
  }

  [DynamicData(nameof(DistinctScopes))]
  [TestMethod]
  public void SelfEquivalence(SupplyAndDemandScope<string> Scope)
  {
    Scope.GetMemento().GetRawText().ShouldBe(Scope.GetMemento().GetRawText());
  }

  [DynamicData(nameof(NonEquivalentPairs))]
  [TestMethod]
  public void NonEquivalence(SupplyAndDemandScope<string> L, SupplyAndDemandScope<string> R)
  {
    L.GetMemento().GetRawText().ShouldNotBe(R.GetMemento().GetRawText());
  }

  public static IReadOnlyList<SupplyAndDemandScope<string>> DistinctScopes
  {
    get
    {
      var S = StringTokenSpace;
      var T1 = Any.String();
      var T2 = Any.String();

      return
      [
        S.Any,
        S.Unspecified,
        S.Demand(T1),
        S.Demand(T2),
        S.Supply(T1),
        S.Supply(T2),
        S.For(T1),
        S.For(T2),
        S.Union(S.For(T1), S.For(T2)),
        S.Union(S.Supply(T2), S.For(T1)),
        S.Union(S.For(T2), S.Demand(T1)),
        S.Intersection(S.For(T1), S.For(T2)),
        S.Intersection(S.Supply(T2), S.For(T1)),
        S.Intersection(S.For(T2), S.Demand(T1))
      ];
    }
  }

  public static IReadOnlyList<(SupplyAndDemandScope<string> L, SupplyAndDemandScope<string> R)> NonEquivalentPairs
  {
    get
    {
      return [..DistinctScopes.SelectMany(L => DistinctScopes.Where(R => R != L).Select(R => (L, R)))];
    }
  }

  static void RoundTripNonEquivalentScopesScenario(
    Func<SupplyAndDemandScope<string>.Space, (SupplyAndDemandScope<string> L, SupplyAndDemandScope<string> R)>
      Operation)
  {

    var ScopeSpace = ScopeSpaces.SupplyAndDemand<string>();
    var Token = Any.String();
    var (L, R) = Operation(ScopeSpace);

    L.GetMemento().GetRawText().ShouldNotBe(R.GetMemento().GetRawText());
  }

  static void RoundTripEquivalentScopesScenario(
    Func<SupplyAndDemandScope<string>.Space, (SupplyAndDemandScope<string> L, SupplyAndDemandScope<string> R)>
      Operation)
  {

    var ScopeSpace = ScopeSpaces.SupplyAndDemand<string>();
    var (L, R) = Operation(ScopeSpace);

    L.GetMemento().GetRawText().ShouldBe(R.GetMemento().GetRawText());
  }
}