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
/// A kind of scope based on supply and demand of tokens.
/// </summary>
/// <typeparam name="T">The type of token.</typeparam>
public sealed class SupplyAndDemandScope<T> 
  : DistinctSpaceScope<SupplyAndDemandScope<T>.Space, SupplyAndDemandScope<T>>
{
  internal SupplyAndDemandScope(Space Origin, Predicate<Predicate<T>> CheckForSupport, Predicate<T> SupportsToken) : base(Origin)
  {
    this.CheckForSupport = CheckForSupport;
    this.SupportsToken = SupportsToken;
  }

  readonly Predicate<Predicate<T>> CheckForSupport;
  readonly Predicate<T> SupportsToken;

  /// <inheritdoc />
  protected override bool IsSatisfiedByWithinSpace(SupplyAndDemandScope<T> Other)
  {
    return CheckForSupport(Other.SupportsToken);
  }

  static bool Always(T _)
  {
    return true;
  }

  static bool Never(T _)
  {
    return false;
  }

  static bool Always(Predicate<T> _)
  {
    return true;
  }

  static bool Never(Predicate<T> _)
  {
    return false;
  }

  /// <summary>
  /// The <see cref="ScopeSpace{ScopeImplementation}" /> for the containing class.
  /// </summary>
  public sealed class Space : DistinctSpace, ScopeSpace<SupplyAndDemandScope<T>>
  {
    /// <summary>
    /// Constructs a new, distinct space.
    /// </summary>
    public Space()
    {
      Unspecified = new(this, Always, Never);
      Any = new(this, Always, Always);
    }

    /// <inheritdoc />
    public override SupplyAndDemandScope<T> Any { get; }

    /// <inheritdoc />
    public override SupplyAndDemandScope<T> Unspecified { get; }

    /// <inheritdoc />
    protected override SupplyAndDemandScope<T> UnionWithinSpace(SupplyAndDemandScope<T> L, SupplyAndDemandScope<T> R)
    {
      var LSupportsToken = L.SupportsToken;
      var RSupportsToken = R.SupportsToken;
      var LCheckForSupport = L.CheckForSupport;
      var RCheckForSupport = R.CheckForSupport;
      return new(this, Support => LCheckForSupport(Support) || RCheckForSupport(Support), Token => LSupportsToken(Token) || RSupportsToken(Token));
    }

    /// <inheritdoc />
    protected override SupplyAndDemandScope<T> IntersectionWithinSpace(SupplyAndDemandScope<T> L, SupplyAndDemandScope<T> R)
    {
      var LSupportsToken = L.SupportsToken;
      var RSupportsToken = R.SupportsToken;
      var LCheckForSupport = L.CheckForSupport;
      var RCheckForSupport = R.CheckForSupport;
      return new(this, Support => LCheckForSupport(Support) && RCheckForSupport(Support), Token => LSupportsToken(Token) && RSupportsToken(Token));
    }

    /// <summary>
    /// Creates a <see cref="Scope{Implementation}"/>> that both satisfies and demands <paramref name="Token"/>.
    /// </summary>
    /// <param name="Token">The token in question.</param>
    /// <returns>The requested <see cref="Scope{Implementation}"/>.</returns>
    public SupplyAndDemandScope<T> For(T Token)
    {
      return new(this, DemandToken(Token), SupplyToken(Token));
    }

    /// <summary>
    /// Creates a <see cref="Scope{Implementation}"/> that demands <paramref name="Token"/>.
    /// </summary>
    /// <param name="Token">The demanded token.</param>
    /// <returns>The requested <see cref="Scope{Implementation}"/>.</returns>
    public SupplyAndDemandScope<T> Demand(T Token)
    {
      return Demand([Token]);
    }

    /// <summary>
    /// Creates a <see cref="Scope{Implementation}"/> that demands <paramref name="Tokens"/>.
    /// </summary>
    /// <param name="Tokens">The demanded tokens.</param>
    /// <returns>The requested <see cref="Scope{Implementation}"/>.</returns>
    public SupplyAndDemandScope<T> Demand(IEnumerable<T> Tokens)
    {
      return new(this, DemandTokens(Tokens), Never);
    }

    /// <summary>
    /// Creates a <see cref="Scope{Implementation}"/> that demands <paramref name="Token"/>.
    /// </summary>
    /// <param name="Token">The demanded token.</param>
    /// <returns>The requested <see cref="Scope{Implementation}"/>.</returns>
    public SupplyAndDemandScope<T> Supply(T Token)
    {
      return new(this, Never, SupplyToken(Token));
    }

    /// <summary>
    /// Creates a <see cref="Scope{Implementation}"/> that demands <paramref name="Tokens"/>.
    /// </summary>
    /// <param name="Tokens">The demanded tokens.</param>
    /// <returns>The requested <see cref="Scope{Implementation}"/>.</returns>
    public SupplyAndDemandScope<T> Supply(IEnumerable<T> Tokens)
    {
      return new(this, Never, SupplyTokens(Tokens));
    }

    static Predicate<Predicate<T>> DemandTokens(IEnumerable<T> Tokens)
    {
      return Demanded => Tokens.All(Token => Demanded(Token));
    }

    static Predicate<T> SupplyTokens(IEnumerable<T> Tokens)
    {
      return Tokens.Contains;
    }

    static Predicate<T> SupplyToken(T Required)
    {
      return Checked => Equals(Required, Checked);
    }

    static Predicate<Predicate<T>> DemandToken(T Required)
    {
      return Supplied => Supplied(Required);
    }
  }
}