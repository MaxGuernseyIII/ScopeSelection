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
public class CompositeScopeBehaviors
{
  CompositeScope<MockScope1, MockScope2>.Space Space = null!;
  MockScope1.Space Space1 = null!;
  MockScope2.Space Space2 = null!;

  [TestInitialize]
  public void Setup()
  {
    Space1 = new();
    Space2 = new();
    Space = ScopeSpaces.Composite(Space1, Space2);
  }

  [TestMethod]
  public void AnyIsComposed()
  {
    Space.Any.ShouldBeEquivalentTo(
      Space.Combine(Space1.Any, Space2.Any));
  }

  [TestMethod]
  public void UnspecifiedIsComposed()
  {
    Space.Unspecified.ShouldBeEquivalentTo(
      Space.Combine(Space1.Unspecified, Space2.Unspecified));
  }

  [TestMethod]
  public void IncludesWhenBothLeftAndRightInclude()
  {
    var Left = new MockScope1([], [], []);
    var Right = new MockScope2([], [], []);
    var Composed = Space.Combine(
      new([Left], [], []),
      new([Right], [], [])
    );

    Composed.IsSatisfiedBy(Space.Combine(Left, Right)).ShouldBeTrue();
  }

  [TestMethod]
  public void IncludesWhenLeftAIncludes()
  {
    var Left = new MockScope1([], [], []);
    var Right = new MockScope2([], [], []);
    var Composed = Space.Combine(
      new([Left], [], []),
      new([], [], [])
    );

    Composed.IsSatisfiedBy(Space.Combine(Left, Right)).ShouldBeFalse();
  }

  [TestMethod]
  public void IncludesWhenRightIncludes()
  {
    var Left = new MockScope1([], [], []);
    var Right = new MockScope2([], [], []);
    var Composed = Space.Combine(
      new([], [], []),
      new([Right], [], [])
    );

    Composed.IsSatisfiedBy(Space.Combine(Left, Right)).ShouldBeFalse();
  }

  [TestMethod]
  public void IncludesWhenNeitherIncludes()
  {
    var Left = new MockScope1([], [], []);
    var Right = new MockScope2([], [], []);
    var Composed = Space.Combine(
      new([], [], []),
      new([], [], [])
    );

    Composed.IsSatisfiedBy(Space.Combine(Left, Right)).ShouldBeFalse();
  }

  [TestMethod]
  public void Or()
  {
    var NewLeft = new MockScope1([], [], []);
    var NewRight = new MockScope2([], [], []);
    var RightLeft = new MockScope1([], [], []);
    var RightRight = new MockScope2([], [], []);
    var LeftLeft = new MockScope1([], [(RightLeft, NewLeft)], []);
    var LeftRight = new MockScope2([], [(RightRight, NewRight)], []);

    var Actual = Space.Union(Space.Combine(LeftLeft, LeftRight),
      Space.Combine(RightLeft, RightRight));

    Actual.ShouldBeEquivalentTo(Space.Combine(NewLeft, NewRight));
  }

  [TestMethod]
  public void And()
  {
    var NewLeft = new MockScope1([], [], []);
    var NewRight = new MockScope2([], [], []);
    var RightLeft = new MockScope1([], [], []);
    var RightRight = new MockScope2([], [], []);
    var LeftLeft = new MockScope1([], [], [(RightLeft, NewLeft)]);
    var LeftRight = new MockScope2([], [], [(RightRight, NewRight)]);

    var Actual = Space.Intersection(Space.Combine(LeftLeft, LeftRight),
      Space.Combine(RightLeft, RightRight));

    Actual.ShouldBeEquivalentTo(Space.Combine(NewLeft, NewRight));
  }

  [TestMethod]
  public void Example()
  {
    var TypeDimension = ScopeSpaces.SupplyAndDemand<Type>();
    var TagsDimension = ScopeSpaces.SupplyAndDemand<string>();
    var CombinedScopeSpace = ScopeSpaces.Composite(TypeDimension, TagsDimension);
    var Supplied = CombinedScopeSpace.Combine(
      TypeDimension.Supply(typeof(Statement)),
      TagsDimension.Supply(["@ui", "@account-management", "@login"]));
    var Demanded = CombinedScopeSpace.Combine(
      TypeDimension.Any,
      TagsDimension.Demand(["@ui", "@login"])
    );

    Demanded.IsSatisfiedBy(Supplied).ShouldBe(true);
  }

  [TestMethod]
  public void ScanningTokensFromDifferentSpacesNotSupported()
  {
    var Token = new MockToken();
    var Dimension1 = ScopeSpaces.SupplyAndDemand<MockToken>();
    var Dimension2 = ScopeSpaces.SupplyAndDemand<MockToken>();
    var CombinedScopeSpace1 = ScopeSpaces.Composite(Dimension1, Dimension2);
    var CombinedScopeSpace2 = ScopeSpaces.Composite(Dimension1, Dimension2);

    Assert.Throws<InvalidOperationException>(() =>
    {
      CombinedScopeSpace1.Combine(Dimension1.For(Token), Dimension2.For(Token))
        .IsSatisfiedBy(
          CombinedScopeSpace2.Combine(Dimension1.For(Token), Dimension2.For(Token)));
    }).Message.ShouldBe("Cannot compare scopes from different spaces.");
  }

  class Statement
  {
  }
}

[TestClass]
public class DistinctSpaceScopeBehaviors
{
  [TestMethod]
  public void CannotCompareFromDifferentSpaces()
  {
    var Space1 = new MockDistinctSpaceScope.Space();
    var Space2 = new MockDistinctSpaceScope.Space();

    Assert.Throws<InvalidOperationException>(() =>
    {
      new MockDistinctSpaceScope(Space1)
        .IsSatisfiedBy(
          new(Space2));
    }).Message.ShouldBe("Cannot compare scopes from different spaces.");
  }

  [TestMethod]
  public void CannotUnionFromDifferentSpaces()
  {
    var Space1 = new MockDistinctSpaceScope.Space();
    var Space2 = new MockDistinctSpaceScope.Space();

    Assert.Throws<InvalidOperationException>(() =>
    {
      Space2.Union(new(Space1), new(Space1));
    }).Message.ShouldBe("Cannot union scopes from different spaces.");

    Assert.Throws<InvalidOperationException>(() =>
    {
      Space2.Union(new(Space2), new(Space1));
    }).Message.ShouldBe("Cannot union scopes from different spaces.");

    Assert.Throws<InvalidOperationException>(() =>
    {
      Space2.Union(new(Space1), new(Space2));
    }).Message.ShouldBe("Cannot union scopes from different spaces.");
  }

  [TestMethod]
  public void CannotIntersectFromDifferentSpaces()
  {
    var Space1 = new MockDistinctSpaceScope.Space();
    var Space2 = new MockDistinctSpaceScope.Space();

    Assert.Throws<InvalidOperationException>(() =>
    {
      Space2.Intersection(new(Space1), new(Space1));
    }).Message.ShouldBe("Cannot intersect scopes from different spaces.");

    Assert.Throws<InvalidOperationException>(() =>
    {
      Space2.Intersection(new(Space2), new(Space1));
    }).Message.ShouldBe("Cannot intersect scopes from different spaces.");

    Assert.Throws<InvalidOperationException>(() =>
    {
      Space2.Intersection(new(Space1), new(Space2));
    }).Message.ShouldBe("Cannot intersect scopes from different spaces.");
  }

  [TestMethod]
  public void SatisfiedInSameSpace()
  {
    var Space = new MockDistinctSpaceScope.Space();
    var Solution = new MockDistinctSpaceScope(Space);
    var Requirement = new MockDistinctSpaceScope(Space)
    {
      SatisfiedBy = { Solution }
    };

    var Satisfied = Requirement.IsSatisfiedBy(Solution);

    Satisfied.ShouldBeTrue();
  }

  [TestMethod]
  public void UnsatisfiedInSameSpace()
  {
    var Space = new MockDistinctSpaceScope.Space();
    var Solution = new MockDistinctSpaceScope(Space);
    var Requirement = new MockDistinctSpaceScope(Space);

    var Satisfied = Requirement.IsSatisfiedBy(Solution);

    Satisfied.ShouldBeFalse();
  }

  [TestMethod]
  public void Union()
  {
    var Space = new MockDistinctSpaceScope.Space();
    var L = new MockDistinctSpaceScope(Space);
    var R = new MockDistinctSpaceScope(Space);
    var Expected = new MockDistinctSpaceScope(Space);
    Space.Unions.Add((L, R, Expected));

    var Actual = Space.Union(L, R);

    Actual.ShouldBeSameAs(Expected);
  }

  [TestMethod]
  public void Intersection()
  {
    var Space = new MockDistinctSpaceScope.Space();
    var L = new MockDistinctSpaceScope(Space);
    var R = new MockDistinctSpaceScope(Space);
    var Expected = new MockDistinctSpaceScope(Space);
    Space.Intersections.Add((L, R, Expected));

    var Actual = Space.Intersection(L, R);

    Actual.ShouldBeSameAs(Expected);
  }

  class MockDistinctSpaceScope(MockDistinctSpaceScope.Space Origin) : DistinctSpaceScope<MockDistinctSpaceScope.Space, MockDistinctSpaceScope>(Origin)
  {
    public List<MockDistinctSpaceScope> SatisfiedBy { get; } = [];

    public class Space : DistinctSpace
    {
      public List<(MockDistinctSpaceScope L, MockDistinctSpaceScope R, MockDistinctSpaceScope P)> Unions { get; } = [];

      public List<(MockDistinctSpaceScope L, MockDistinctSpaceScope R, MockDistinctSpaceScope P)> Intersections { get; } =
        [];

      public override MockDistinctSpaceScope Any => throw new NotImplementedException();

      public override MockDistinctSpaceScope Unspecified => throw new NotImplementedException();

      protected override MockDistinctSpaceScope UnionWithinSpace(
        MockDistinctSpaceScope L,
        MockDistinctSpaceScope R)
      {
        return Unions.Single(U => U.L == L && U.R == R).P;
      }

      protected override MockDistinctSpaceScope IntersectionWithinSpace(
        MockDistinctSpaceScope L,
        MockDistinctSpaceScope R)
      {
        return Intersections.Single(U => U.L == L && U.R == R).P;
      }
    }

    protected override bool IsSatisfiedByWithinSpace(MockDistinctSpaceScope Other)
    {
      return SatisfiedBy.Contains(Other);
    }
  }
}