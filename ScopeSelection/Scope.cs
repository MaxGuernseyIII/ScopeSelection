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
/// The definition of a scope. Can be used to define the scope an object lives in or check if an object lives in a scope.
/// </summary>
/// <typeparam name="Implementation">The actual type of <see cref="Scope{Implementation}"/> being used.</typeparam>
public interface Scope<in Implementation>
  where Implementation : Scope<Implementation>
{
  /// <summary>
  /// Treats the current <see cref="Scope{Implementation}"/> as a requirement and checks the other scope to determine if it satisfies said requirement.
  /// </summary>
  /// <param name="Other">The scope that supplies satisfaction.</param>
  /// <returns><c>true</c> of the requirement manifest in this <see cref="Scope{Implementation}"/> is met by <paramref name="Other"/> and <c>false</c> if not.</returns>
  public bool IsSatisfiedBy(Implementation Other);
}