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

using System.Text.Json;

namespace ScopeSelection;

/// <summary>
///   A kind of scope based on supply and demand of tokens.
/// </summary>
/// <typeparam name="T">The type of token.</typeparam>
public sealed class SupplyAndDemandScope<T>
  : DistinctSpaceScope<SupplyAndDemandScope<T>.Space, SupplyAndDemandScope<T>>
{
  readonly Predicate<Predicate<T>> CheckForSupport;
  readonly MementoStructure Structure;
  readonly Predicate<T> SupportsToken;

  SupplyAndDemandScope(
    Space Origin, 
    Predicate<Predicate<T>> CheckForSupport, 
    Predicate<T> SupportsToken,
    MementoStructure Structure) : base(Origin)
  {
    this.CheckForSupport = CheckForSupport;
    this.SupportsToken = SupportsToken;
    this.Structure = Structure;
  }

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
  ///   Gets a representation of this <see cref="Scope{Implementation}" /> that can be used to create an equivalent in
  ///   another <see cref="ScopeSpace{ScopeImplementation}" />.
  /// </summary>
  /// <returns>A JSON memento.</returns>
  public JsonElement GetMemento()
  {
    return JsonSerializer.SerializeToElement(Structure);
  }

  /// <summary>
  ///   The <see cref="ScopeSpace{ScopeImplementation}" /> for the containing class.
  /// </summary>
  public sealed class Space : DistinctSpace, ScopeSpace<SupplyAndDemandScope<T>>
  {
    internal Func<T, JsonElement> TokenSerializer = Token => JsonSerializer.SerializeToDocument(Token).RootElement;
    internal Func<JsonElement, T> TokenDeserializer = Element => JsonSerializer.Deserialize<T>(Element.GetRawText())!;

    /// <summary>
    ///   Constructs a new, distinct space.
    /// </summary>
    public Space()
    {
      Unspecified = new(this, Always, Never, new());
      Any = new(this, Always, Always, new() { IsAny = true });
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
      return new(this, Support => LCheckForSupport(Support) || RCheckForSupport(Support),
        Token => LSupportsToken(Token) || RSupportsToken(Token), new());
    }

    /// <inheritdoc />
    protected override SupplyAndDemandScope<T> IntersectionWithinSpace(SupplyAndDemandScope<T> L,
      SupplyAndDemandScope<T> R)
    {
      var LSupportsToken = L.SupportsToken;
      var RSupportsToken = R.SupportsToken;
      var LCheckForSupport = L.CheckForSupport;
      var RCheckForSupport = R.CheckForSupport;
      return new(this, Support => LCheckForSupport(Support) && RCheckForSupport(Support),
        Token => LSupportsToken(Token) && RSupportsToken(Token), new());
    }

    /// <summary>
    ///   Creates a <see cref="Scope{Implementation}" />> that both satisfies and demands <paramref name="Token" />.
    /// </summary>
    /// <param name="Token">The token in question.</param>
    /// <returns>The requested <see cref="Scope{Implementation}" />.</returns>
    public SupplyAndDemandScope<T> For(T Token)
    {
      var TokenJson = TokenSerializer(Token);
      return new(this, DemandToken(Token), SupplyToken(Token), new()
      {
        SupplyTokens = [TokenJson],
        DemandedTokens = [TokenJson]
      });
    }

    /// <summary>
    ///   Creates a <see cref="Scope{Implementation}" /> that demands <paramref name="Token" />.
    /// </summary>
    /// <param name="Token">The demanded token.</param>
    /// <returns>The requested <see cref="Scope{Implementation}" />.</returns>
    public SupplyAndDemandScope<T> Demand(T Token)
    {
      return Demand([Token]);
    }

    /// <summary>
    ///   Creates a <see cref="Scope{Implementation}" /> that demands <paramref name="Tokens" />.
    /// </summary>
    /// <param name="Tokens">The demanded tokens.</param>
    /// <returns>The requested <see cref="Scope{Implementation}" />.</returns>
    public SupplyAndDemandScope<T> Demand(IEnumerable<T> Tokens)
    {
      return new(this, DemandTokens(Tokens), Never, new()
      {
        DemandedTokens = [
          ..Tokens.Select(TokenSerializer)
        ]
      });
    }

    /// <summary>
    ///   Creates a <see cref="Scope{Implementation}" /> that demands <paramref name="Token" />.
    /// </summary>
    /// <param name="Token">The demanded token.</param>
    /// <returns>The requested <see cref="Scope{Implementation}" />.</returns>
    public SupplyAndDemandScope<T> Supply(T Token)
    {
      return Supply([Token]);
    }

    /// <summary>
    ///   Creates a <see cref="Scope{Implementation}" /> that demands <paramref name="Tokens" />.
    /// </summary>
    /// <param name="Tokens">The demanded tokens.</param>
    /// <returns>The requested <see cref="Scope{Implementation}" />.</returns>
    public SupplyAndDemandScope<T> Supply(IEnumerable<T> Tokens)
    {
      return new(this, Never, SupplyTokens(Tokens), new()
      {
        SupplyTokens = [
          ..Tokens.Select(TokenSerializer)
        ]
      });
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

    /// <summary>
    /// Reconstitute a <see cref="Scope{Implementation}"/> from a memento.
    /// </summary>
    /// <param name="Memento">The definition of the <see cref="Scope{Implementation}"/> to reconstitute.</param>
    /// <returns>The requested object.</returns>
    public SupplyAndDemandScope<T> FromMemento(JsonElement Memento)
    {
      var Structure = JsonSerializer.Deserialize<MementoStructure>(Memento.GetRawText())!;
      return Structure.IsAny ? Any : Unspecified;
    }
  }

  class MementoStructure
  {
    public bool IsAny { get; set; }
    public JsonElement[]? DemandedTokens { get; set; }
    public JsonElement[]? SupplyTokens { get; set; }
  }
}