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

  /// <inheritdoc />
  public override JsonElement GetMemento()
  {
    return JsonSerializer.SerializeToElement(Structure, SerializationOptions.JsonOptions);
  }

  /// <inheritdoc />
  public override string ToString()
  {
    return Structure.ToString();
  }

  /// <summary>
  ///   The <see cref="ScopeSpace{ScopeImplementation}" /> for the containing class.
  /// </summary>
  public sealed class Space : DistinctSpace, ScopeSpace<SupplyAndDemandScope<T>>
  {
    internal TokenSerializer TokenSerializer;

    internal Space(TokenSerializer? TokenSerializer = null)
    {
      this.TokenSerializer = TokenSerializer ?? new()
      {
        DeserializeToken = Element =>
          JsonSerializer.Deserialize<T>(Element.GetRawText(), SerializationOptions.JsonOptions)!,

        SerializeToken = Token =>
          JsonSerializer.SerializeToDocument(Token, SerializationOptions.JsonOptions).RootElement
      };
      Unspecified = new(this, Always, Never, new());
      Any = new(this, Always, Always, new() {IsAny = true});
    }

    /// <inheritdoc />
    public override SupplyAndDemandScope<T> Any { get; }

    /// <inheritdoc />
    public override SupplyAndDemandScope<T> Unspecified { get; }


    /// <inheritdoc />
    public override SupplyAndDemandScope<T> FromMemento(JsonElement Memento)
    {
      var Structure =
        JsonSerializer.Deserialize<MementoStructure>(Memento.GetRawText(), SerializationOptions.JsonOptions)!;
      return FromStructure(Structure);
    }

    /// <inheritdoc />
    protected override SupplyAndDemandScope<T> UnionWithinSpace(SupplyAndDemandScope<T> L, SupplyAndDemandScope<T> R)
    {
      var LSupportsToken = L.SupportsToken;
      var RSupportsToken = R.SupportsToken;
      var LCheckForSupport = L.CheckForSupport;
      var RCheckForSupport = R.CheckForSupport;
      return new(this, Support => LCheckForSupport(Support) || RCheckForSupport(Support),
        Token => LSupportsToken(Token) || RSupportsToken(Token), new()
        {
          Union = new()
          {
            L = L.Structure,
            R = R.Structure
          }
        });
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
        Token => LSupportsToken(Token) && RSupportsToken(Token), new()
        {
          Intersection = new()
          {
            L = L.Structure,
            R = R.Structure
          }
        });
    }

    /// <summary>
    ///   Creates a <see cref="Scope{Implementation}" />> that both satisfies and demands <paramref name="Token" />.
    /// </summary>
    /// <param name="Token">The token in question.</param>
    /// <returns>The requested <see cref="Scope{Implementation}" />.</returns>
    public SupplyAndDemandScope<T> For(T Token)
    {
      var TokenJson = TokenSerializer.SerializeToken(Token);
      return new(this, DemandToken(Token), SupplyToken(Token), new()
      {
        ForToken = TokenJson
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
        DemandedTokens =
        [
          ..Tokens.Select(TokenSerializer.SerializeToken)
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
        SupplyTokens =
        [
          ..Tokens.Select(TokenSerializer.SerializeToken)
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

    SupplyAndDemandScope<T> FromStructure(MementoStructure Structure)
    {
      return Structure switch
      {
        {IsAny: true} => Any,
        {Union: { } U} => Union(FromStructure(U.L), FromStructure(U.R)),
        {Intersection: { } I} => Intersection(FromStructure(I.L), FromStructure(I.R)),
        {SupplyTokens: null, DemandedTokens: { } Demands} => Demand(Demands.Select(TokenSerializer.DeserializeToken)),
        {SupplyTokens: { } Supplies, DemandedTokens: null} => Supply(Supplies.Select(TokenSerializer.DeserializeToken)),
        {ForToken: { } F} => For(TokenSerializer.DeserializeToken(F)),
        _ => Unspecified
      };
    }
  }

  /// <summary>
  ///   Serializes and deserializes tokens.
  /// </summary>
  public class TokenSerializer
  {
    /// <summary>
    ///   Converts a token to a <see cref="JsonElement" />.
    /// </summary>
    public required Func<T, JsonElement> SerializeToken { get; set; }

    /// <summary>
    ///   Converts a <see cref="JsonElement" /> to a token.
    /// </summary>
    public required Func<JsonElement, T> DeserializeToken { get; set; }
  }

  class MementoSetOperatorStructure
  {
    public required MementoStructure L { get; set; }
    public required MementoStructure R { get; set; }
  }

  class MementoStructure
  {
    public bool IsAny { get; set; }
    public JsonElement? ForToken { get; set; }
    public JsonElement[]? DemandedTokens { get; set; }
    public JsonElement[]? SupplyTokens { get; set; }
    public MementoSetOperatorStructure? Intersection { get; set; }
    public MementoSetOperatorStructure? Union { get; set; }

    public override string ToString()
    {
      return this switch
      {
        {IsAny: true} => "any",
        {Union: { } U} => $"({U.L}) | ({U.R})",
        {Intersection: { } I} => $"({I.L}) & ({I.R})",
        {SupplyTokens: null, DemandedTokens: { } Demands} =>
          $"demand({string.Join(", ", Demands.Select(D => D.GetRawText()))})",
        {SupplyTokens: { } Supplies, DemandedTokens: null} =>
          $"supply({string.Join(", ", Supplies.Select(D => D.GetRawText()))})",
        {ForToken: { } F} => $"for({F.GetRawText()})",
        _ => "unspecified"
      };
    }
  }
}