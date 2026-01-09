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

using static Samples.ClauseType;

[TestClass]
public class Samples
{
  public enum ClauseType
  {
    Given,
    When,
    Then
  }

  [TestMethod]
  public void SelectItem()
  {
    var ScopeSpace = ScopeSpaces.SupplyAndDemand<ClauseType>();
    var BindingScope = ScopeSpace.Supply(Given);

    var Included = ScopeSpace.Demand(Given).IsSatisfiedBy(BindingScope);

    Included.ShouldBeTrue();
  }

  [TestMethod]
  public void FailToSelectItem()
  {
    var ScopeSpace = ScopeSpaces.SupplyAndDemand<ClauseType>();
    var BindingScope = ScopeSpace.Supply([Given, Then]);

    var Included = ScopeSpace.Demand(When).IsSatisfiedBy(BindingScope);

    Included.ShouldBeFalse();
  }

  [TestMethod]
  public void SelectItemInCompositeSpace()
  {
    var ClauseTypes = ScopeSpaces.SupplyAndDemand<ClauseType>();
    var Features = ScopeSpaces.SupplyAndDemand<string>();
    var CompositeSpace = ScopeSpaces.Composite(ClauseTypes, Features);
    var BindingScope = CompositeSpace.Combine(ClauseTypes.Any, Features.Supply("Scope Resolution"));

    var Included = CompositeSpace.Combine(ClauseTypes.Demand(Given), Features.Any).IsSatisfiedBy(BindingScope);

    Included.ShouldBeTrue();
  }

  [TestMethod]
  public void FailCompositeSelectionDueToOneDimension()
  {
    var ClauseTypes = ScopeSpaces.SupplyAndDemand<ClauseType>();
    var Features = ScopeSpaces.SupplyAndDemand<string>();
    var CompositeSpace = ScopeSpaces.Composite(ClauseTypes, Features);
    var BindingScope = CompositeSpace.Combine(ClauseTypes.Unspecified, Features.Supply("Scope Resolution"));

    var Included = CompositeSpace.Combine(ClauseTypes.Demand(Given), Features.Any).IsSatisfiedBy(BindingScope);

    Included.ShouldBeFalse();
  }
}