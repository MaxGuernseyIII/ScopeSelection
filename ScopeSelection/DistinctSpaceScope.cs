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
///   A base class for scopes that live within distinct spaces and cannot be compared with the same kind of scope in
///   another space.
/// </summary>
public abstract class DistinctSpaceScope<SpaceImplementation, Implementation>(SpaceImplementation Origin)
  : Scope<Implementation>
  where Implementation : DistinctSpaceScope<SpaceImplementation, Implementation>
  where SpaceImplementation : DistinctSpaceScope<SpaceImplementation, Implementation>.DistinctSpace
{
  /// <summary>
  /// The <typeparamref name="SpaceImplementation"/> of origin.
  /// </summary>
  public SpaceImplementation Origin { get; } = Origin;

  /// <summary>
  ///   A base class a space that contains scopes which cannot be compared with other scopes of the same type in a different
  ///   space.
  /// </summary>
  public abstract class DistinctSpace : ScopeSpace<Implementation>
  {
    /// <inheritdoc />
    public abstract Implementation Any { get; }

    /// <inheritdoc />
    public abstract Implementation Unspecified { get; }

    /// <inheritdoc />
    public Implementation Union(Implementation L, Implementation R)
    {
      return UnionWithinSpace(L, R);
    }

    /// <inheritdoc />
    public Implementation Intersection(Implementation L, Implementation R)
    {
      return IntersectionWithinSpace(L, R);
    }

    /// <inheritdoc cref="Union" />
    protected abstract Implementation UnionWithinSpace(Implementation DistinctSpaceScope, Implementation Implementation);

    /// <inheritdoc cref="Intersection" />
    protected abstract Implementation IntersectionWithinSpace(Implementation DistinctSpaceScope, Implementation Implementation);
  }

  /// <inheritdoc />
  public bool IsSatisfiedBy(Implementation Other)
  {
    RequireSameSpace(Origin, Other.Origin);

    return IsSatisfiedByWithinSpace(Other);
  }

  static void RequireSameSpace(SpaceImplementation L, SpaceImplementation R)
  {
    if (L != R)
      throw new InvalidOperationException("Cannot compare scopes from different spaces.");
  }

  /// <inheritdoc cref="IsSatisfiedBy" />
  protected abstract bool IsSatisfiedByWithinSpace(Implementation Other);
}