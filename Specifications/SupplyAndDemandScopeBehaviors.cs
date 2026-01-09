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

using Shouldly;

namespace Specifications;

[TestClass]
public class SupplyAndDemandScopeBehaviors
{
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
}