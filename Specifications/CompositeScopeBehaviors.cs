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

using SerializationTestScope = CompositeScope<SupplyAndDemandScope<string>,
  CompositeScope<SupplyAndDemandScope<string>, SupplyAndDemandScope<string>>>;

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
  public void IsDistinct()
  {
    var Space = ScopeSpaces.Composite(new MockScope1.Space(), new MockScope2.Space());

    Space.ShouldBeAssignableTo<DistinctSpaceScope<
      CompositeScope<MockScope1, MockScope2>.Space,
      CompositeScope<MockScope1, MockScope2>>.DistinctSpace>();
  }

  [DynamicData(nameof(DistinctScopes))]
  [TestMethod]
  public void RoundTripEquivalence(SerializationTestScope Scope)
  {
    var OriginalMemento = Scope.GetMemento();

    var RoundTrippedMemento = SerializationTestSpace.FromMemento(OriginalMemento).GetMemento();

    RoundTrippedMemento.GetRawText().ShouldBe(OriginalMemento.GetRawText());
  }

  [DynamicData(nameof(EquivalentPairs))]
  [TestMethod]
  public void SelfEquivalence(SerializationTestScope L, SerializationTestScope R)
  {
    L.GetMemento().GetRawText().ShouldBe(R.GetMemento().GetRawText());
  }

  [DynamicData(nameof(NonEquivalentPairs))]
  [TestMethod]
  public void NonEquivalence(SerializationTestScope L, SerializationTestScope R)
  {
    L.GetMemento().GetRawText().ShouldNotBe(R.GetMemento().GetRawText());
  }

  static readonly SupplyAndDemandScope<string>.Space SerializationTestDimensionSpace = ScopeSpaces.SupplyAndDemand<string>();

  static readonly CompositeScope<SupplyAndDemandScope<string>, SupplyAndDemandScope<string>>.Space SerializationTestCompositeDimension 
    = ScopeSpaces.Composite(SerializationTestDimensionSpace, SerializationTestDimensionSpace);

  static readonly SerializationTestScope.Space SerializationTestSpace =
    ScopeSpaces.Composite(SerializationTestDimensionSpace,
      SerializationTestCompositeDimension);

  static IReadOnlyList<CompositeScope<SupplyAndDemandScope<string>, CompositeScope<SupplyAndDemandScope<string>, SupplyAndDemandScope<string>>>> MakeDistinctScopes()
  {
    var S = SerializationTestSpace;
    var C = SerializationTestCompositeDimension;
    var D = SerializationTestDimensionSpace;
    var T1 = "t1";
    var T2 = "t2";
    var T3 = "t3";

    return
    [
      S.Any,
      S.Unspecified,
      S.Combine(D.Demand(T1), C.Combine(D.Demand(T2), D.Demand(T3)))
    ];
  }



  public static IReadOnlyList<SerializationTestScope> DistinctScopes { get; } = MakeDistinctScopes();

  public static IReadOnlyList<(SerializationTestScope, SerializationTestScope)> EquivalentPairs { get; } =
    [.. MakeDistinctScopes().Zip(MakeDistinctScopes())];

  public static IReadOnlyList<(SerializationTestScope L, SerializationTestScope R)> NonEquivalentPairs
  {
    get;
  }
    = [.. DistinctScopes.SelectMany(L => DistinctScopes.Where(R => R != L).Select(R => (L, R)))];
}