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
/// The set of rules governing a <see cref="Scope{Implementation}"/>.
/// </summary>
/// <typeparam name="ScopeImplementation">The type of <see cref="Scope{Implementation}"/> governed.</typeparam>
public interface ScopeSpace<ScopeImplementation>
  where ScopeImplementation : Scope<ScopeImplementation>
{
  /// <summary>
  /// A <see cref="Scope{Implementation}"/> that can be satisfied by any other scope.
  /// </summary>
  public ScopeImplementation Any { get; }

  /// <summary>
  /// A <see cref="Scope{Implementation}"/> that has no declarations. Typically, this means an "empty" <see cref="Scope{Implementation}"/> - one that will not satisfy any other <see cref="Scope{Implementation}"/> upon interrogation.
  /// </summary>
  public ScopeImplementation Unspecified { get; }

  /// <summary>
  /// A <see cref="Scope{Implementation}"/> that represents the union of <paramref name="L"/> and <paramref name="R"/>.
  /// </summary>
  /// <param name="L">A <see cref="Scope{Implementation}"/> to merge.</param>
  /// <param name="R">A <see cref="Scope{Implementation}"/> to merge.</param>
  /// <returns>The requested <see cref="Scope{Implementation}"/>.</returns>
  public ScopeImplementation Union(ScopeImplementation L, ScopeImplementation R);

  /// <summary>
  /// A <see cref="Scope{Implementation}"/> that represents the intersection of <paramref name="L"/> and <paramref name="R"/>.
  /// </summary>
  /// <param name="L">A <see cref="Scope{Implementation}"/> to merge.</param>
  /// <param name="R">A <see cref="Scope{Implementation}"/> to merge.</param>
  /// <returns>The requested <see cref="Scope{Implementation}"/>.</returns>
  public ScopeImplementation Intersection(ScopeImplementation L, ScopeImplementation R);
}