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

#if NETSTANDARD2_0
namespace System.Runtime.CompilerServices
{
  // Needed for `init` setters
  static class IsExternalInit
  {
  }

  // Needed for `required` members (C# 11)
  [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
  sealed class CompilerFeatureRequiredAttribute : Attribute
  {
    public CompilerFeatureRequiredAttribute(string featureName)
    {
    }
  }

  // Some toolchains expect this one in this namespace too
  [AttributeUsage(AttributeTargets.All, Inherited = false)]
  sealed class RequiredMemberAttribute : Attribute
  {
  }
}

namespace System.Diagnostics.CodeAnalysis
{
  // Required-members metadata (commonly expected here)
  [AttributeUsage(
    AttributeTargets.Class |
    AttributeTargets.Struct |
    AttributeTargets.Field |
    AttributeTargets.Property,
    Inherited = false)]
  sealed class RequiredMemberAttribute : Attribute
  {
  }

  // Marks ctors that set all required members
  [AttributeUsage(AttributeTargets.Constructor)]
  sealed class SetsRequiredMembersAttribute : Attribute
  {
  }
}

#endif