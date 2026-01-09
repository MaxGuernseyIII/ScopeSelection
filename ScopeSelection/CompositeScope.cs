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
/// var TypeDimension = ScopeSpaces.SupplyAndDemand&lt;Type&gt;();
/// var TagsDimension = ScopeSpaces.SupplyAndDemand&lt;string&gt;();
/// var CombinedScopeSpace = ScopeSpaces.Composite(TypeDimension, TagsDimension);
/// var Supplied = CombinedScopeSpace.Combine(
///   TypeDimension.Supply(typeof(Statement)),
///   TagsDimension.Supply(["@ui", "@account-management", "@login"]));
/// var Demanded = CombinedScopeSpace.Combine(
///   TypeDimension.Any,
///   TagsDimension.Demand(["@ui", "@login"])
/// );
/// 
/// Demanded.IsSatisfiedBy(Supplied).ShouldBe(true);
/// </example>
/// <param name="Left">The first dimension in the tuple.</param>
/// <param name="Right">The second dimension in the tuple.</param>
/// <typeparam name="TLeft">The type of the first dimension in the tuple.</typeparam>
/// <typeparam name="TRight">The ype of the second dimension in the tuple.</typeparam>
public sealed class CompositeScope<TLeft, TRight>(TLeft Left, TRight Right) : Scope<CompositeScope<TLeft, TRight>>
  where TLeft : Scope<TLeft>
  where TRight : Scope<TRight>
{
  /// <summary>
  /// The value of the first dimension in the tuple.
  /// </summary>
  public TLeft Left { get; } = Left;


  /// <summary>
  /// The value of the second dimension in the tuple.
  /// </summary>
  public TRight Right { get; } = Right;

  /// <summary>
  /// Check if this scope is satisfied by another scope.
  /// </summary>
  /// <param name="Other">The scope that must satisfy the requirements of this scope.</param>
  /// <returns><c>true</c> if the other scope is acceptable, <c>false</c> if not</returns>
  public bool IsSatisfiedBy(CompositeScope<TLeft, TRight> Other)
  {
    return Left.IsSatisfiedBy(Other.Left) && Right.IsSatisfiedBy(Other.Right);
  }

  /// <summary>
  /// The definition of the 2-dimensional pace in which <see cref="CompositeScope{TLeft,TRight}"/> lives.
  /// </summary>
  /// <param name="LeftDimension">The definition of the first dimension.</param>
  /// <param name="RightDimension">The definition of the second dimension.</param>
  public sealed class Space(ScopeSpace<TLeft> LeftDimension, ScopeSpace<TRight> RightDimension)
    : ScopeSpace<CompositeScope<TLeft, TRight>>
  {
    /// <inheritdoc />
    public CompositeScope<TLeft, TRight> Any { get; } = new(LeftDimension.Any, RightDimension.Any);

    /// <inheritdoc />
    public CompositeScope<TLeft, TRight> Unspecified { get; } = new(LeftDimension.Unspecified, RightDimension.Unspecified);

    /// <inheritdoc />
    public CompositeScope<TLeft, TRight> Union(
      CompositeScope<TLeft, TRight> L,
      CompositeScope<TLeft, TRight> R)
    {
      return new(LeftDimension.Union(L.Left, R.Left), RightDimension.Union(L.Right, R.Right));
    }

    /// <inheritdoc />
    public CompositeScope<TLeft, TRight> Intersection(
      CompositeScope<TLeft, TRight> L,
      CompositeScope<TLeft, TRight> R)
    {
      return new(LeftDimension.Intersection(L.Left, R.Left), RightDimension.Intersection(L.Right, R.Right));
    }

    /// <summary>
    /// Combine two scopes into a tuple.
    /// </summary>
    /// <param name="Left">The first dimension scope.</param>
    /// <param name="Right">The second dimension scope.</param>
    /// <returns>The combined multi-dimensional scope.</returns>
    public CompositeScope<TLeft, TRight> Combine(TLeft Left, TRight Right)
    {
      return new(Left, Right);
    }
  }
}