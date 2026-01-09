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

class MockScope2(
  IReadOnlyList<MockScope2> Included,
  IReadOnlyList<(MockScope2 Other, MockScope2 Result)> Ored,
  IReadOnlyList<(MockScope2 Other, MockScope2 Result)> Anded) : Scope<MockScope2>
{
  readonly IReadOnlyList<(MockScope2 Other, MockScope2 Result)> Anded = Anded;
  readonly IReadOnlyList<(MockScope2 Other, MockScope2 Result)> Ored = Ored;
  public static MockScope2 Any => new([], [], []);

  public static MockScope2 Unspecified => new([], [], []);

  public bool IsSatisfiedBy(MockScope2 Other)
  {
    return Included.Contains(Other);
  }

  public static MockScope2 operator |(MockScope2 L, MockScope2 R)
  {
    return L.Ored.Single(O => O.Other == R).Result;
  }

  public static MockScope2 operator &(MockScope2 L, MockScope2 R)
  {
    return L.Anded.Single(O => O.Other == R).Result;
  }

  public class Space : ScopeSpace<MockScope2>
  {
    public MockScope2 Any { get; } = new([], [], []);

    public MockScope2 Unspecified { get; } = new([], [], []);


    public MockScope2 Union(MockScope2 L, MockScope2 R)
    {
      return L.Ored.Single(O => O.Other == R).Result;
    }

    public MockScope2 Intersection(MockScope2 L, MockScope2 R)
    {
      return L.Anded.Single(O => O.Other == R).Result;
    }
  }
}