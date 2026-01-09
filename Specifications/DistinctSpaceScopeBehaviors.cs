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