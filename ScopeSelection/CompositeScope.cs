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

namespace ScopeSelection;

/// <summary>
///   A <see cref="Scope{Implementation}" /> that combines two other
///   <see cref="Scope{Implementation}" />s into two isolated dimensions.
///   Inclusion in the resulting combined scope requires the independent inclusion on the left and right dimensions.
/// </summary>
/// <example>
///   var TypeDimension = ScopeSpaces.SupplyAndDemand&lt;Type&gt;();
///   var TagsDimension = ScopeSpaces.SupplyAndDemand&lt;string&gt;();
///   var CombinedScopeSpace = ScopeSpaces.Composite(TypeDimension, TagsDimension);
///   var Supplied = CombinedScopeSpace.Combine(
///   TypeDimension.Supply(typeof(Statement)),
///   TagsDimension.Supply(["@ui", "@account-management", "@login"]));
///   var Demanded = CombinedScopeSpace.Combine(
///   TypeDimension.Any,
///   TagsDimension.Demand(["@ui", "@login"])
///   );
///   Demanded.IsSatisfiedBy(Supplied).ShouldBe(true);
/// </example>
/// <typeparam name="TLeft">The type of the first dimension in the tuple.</typeparam>
/// <typeparam name="TRight">The ype of the second dimension in the tuple.</typeparam>
public sealed class CompositeScope<TLeft, TRight> :
  DistinctSpaceScope<CompositeScope<TLeft, TRight>.Space, CompositeScope<TLeft, TRight>>
  where TLeft : Scope<TLeft>
  where TRight : Scope<TRight>
{
  internal CompositeScope(Space Origin,
    TLeft Left,
    TRight Right) : base(Origin)
  {
    this.Left = Left;
    this.Right = Right;
  }

  /// <summary>
  ///   The value of the first dimension in the tuple.
  /// </summary>
  public TLeft Left { get; }

  /// <summary>
  ///   The value of the second dimension in the tuple.
  /// </summary>
  public TRight Right { get; }

  /// <inheritdoc />
  protected override bool IsSatisfiedByWithinSpace(CompositeScope<TLeft, TRight> Other)
  {
    return Left.IsSatisfiedBy(Other.Left) && Right.IsSatisfiedBy(Other.Right);
  }

  /// <summary>
  ///   The definition of the 2-dimensional pace in which <see cref="CompositeScope{TLeft,TRight}" /> lives.
  /// </summary>
  public sealed class Space : DistinctSpace, ScopeSpace<CompositeScope<TLeft, TRight>>
  {
    readonly ScopeSpace<TLeft> LeftDimension;
    readonly ScopeSpace<TRight> RightDimension;

    /// <summary>
    ///   The definition of the 2-dimensional pace in which <see cref="CompositeScope{TLeft,TRight}" /> lives.
    /// </summary>
    /// <param name="LeftDimension">The definition of the first dimension.</param>
    /// <param name="RightDimension">The definition of the second dimension.</param>
    public Space(ScopeSpace<TLeft> LeftDimension, ScopeSpace<TRight> RightDimension)
    {
      this.LeftDimension = LeftDimension;
      this.RightDimension = RightDimension;
      Any = new(this, LeftDimension.Any, RightDimension.Any);
      Unspecified = new(this, LeftDimension.Unspecified, RightDimension.Unspecified);
    }

    /// <inheritdoc />
    public override CompositeScope<TLeft, TRight> Any { get; }

    /// <inheritdoc />
    public override CompositeScope<TLeft, TRight> Unspecified { get; }

    /// <inheritdoc />
    protected override CompositeScope<TLeft, TRight> UnionWithinSpace(
      CompositeScope<TLeft, TRight> L,
      CompositeScope<TLeft, TRight> R)
    {
      return new(this, LeftDimension.Union(L.Left, R.Left), RightDimension.Union(L.Right, R.Right));
    }

    /// <inheritdoc />
    protected override CompositeScope<TLeft, TRight> IntersectionWithinSpace(
      CompositeScope<TLeft, TRight> L,
      CompositeScope<TLeft, TRight> R)
    {
      return new(this, LeftDimension.Intersection(L.Left, R.Left), RightDimension.Intersection(L.Right, R.Right));
    }

    /// <summary>
    ///   Combine two scopes into a tuple.
    /// </summary>
    /// <param name="Left">The first dimension scope.</param>
    /// <param name="Right">The second dimension scope.</param>
    /// <returns>The combined multi-dimensional scope.</returns>
    public CompositeScope<TLeft, TRight> Combine(TLeft Left, TRight Right)
    {
      return new(this, Left, Right);
    }
  }
}