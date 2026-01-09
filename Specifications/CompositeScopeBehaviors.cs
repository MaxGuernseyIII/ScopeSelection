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
  ScopeSpace<CompositeScope<MockScope1, MockScope2>> Space = null!;
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
      new CompositeScope<MockScope1, MockScope2>(Space1.Any, Space2.Any));
  }

  [TestMethod]
  public void UnspecifiedIsComposed()
  {
    Space.Unspecified.ShouldBeEquivalentTo(
      new CompositeScope<MockScope1, MockScope2>(Space1.Unspecified, Space2.Unspecified));
  }

  [TestMethod]
  public void IncludesWhenBothLeftAndRightInclude()
  {
    var Left = new MockScope1([], [], []);
    var Right = new MockScope2([], [], []);
    var Composed = new CompositeScope<MockScope1, MockScope2>(
      new([Left], [], []),
      new([Right], [], [])
    );

    Composed.IsSatisfiedBy(new(Left, Right)).ShouldBeTrue();
  }

  [TestMethod]
  public void IncludesWhenLeftAIncludes()
  {
    var Left = new MockScope1([], [], []);
    var Right = new MockScope2([], [], []);
    var Composed = new CompositeScope<MockScope1, MockScope2>(
      new([Left], [], []),
      new([], [], [])
    );

    Composed.IsSatisfiedBy(new(Left, Right)).ShouldBeFalse();
  }

  [TestMethod]
  public void IncludesWhenRightIncludes()
  {
    var Left = new MockScope1([], [], []);
    var Right = new MockScope2([], [], []);
    var Composed = new CompositeScope<MockScope1, MockScope2>(
      new([], [], []),
      new([Right], [], [])
    );

    Composed.IsSatisfiedBy(new(Left, Right)).ShouldBeFalse();
  }

  [TestMethod]
  public void IncludesWhenNeitherIncludes()
  {
    var Left = new MockScope1([], [], []);
    var Right = new MockScope2([], [], []);
    var Composed = new CompositeScope<MockScope1, MockScope2>(
      new([], [], []),
      new([], [], [])
    );

    Composed.IsSatisfiedBy(new(Left, Right)).ShouldBeFalse();
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

    var Actual = Space.Union(new(LeftLeft, LeftRight),
      new(RightLeft, RightRight));

    Actual.ShouldBeEquivalentTo(new CompositeScope<MockScope1, MockScope2>(NewLeft, NewRight));
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

    var Actual = Space.Intersection(new(LeftLeft, LeftRight),
      new(RightLeft, RightRight));

    Actual.ShouldBeEquivalentTo(new CompositeScope<MockScope1, MockScope2>(NewLeft, NewRight));
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

  class Statement
  {
  }

  class Parameter
  {
  }
}