namespace ScopeSelection;

/// <summary>
/// A utility to find or create instances of <see cref="ScopeSpace{ScopeImplementation}"/>.
/// </summary>
public static class ScopeSpaces
{
  /// <summary>
  /// The null <see cref="ScopeSpace{ScopeImplementation}"/>. Nothing is partitioned from anything.
  /// </summary>
  public static ScopeSpace<NullScope> Null { get; } = new NullScope.Space();

  /// <summary>
  /// Combines two <see cref="ScopeSpace{ScopeImplementation}"/> to create a multidimensional <see cref="ScopeSpace{ScopeImplementation}"/>.
  /// </summary>
  /// <param name="Left">The <see cref="ScopeSpace{ScopeImplementation}"/> that defines the first dimension.</param>
  /// <param name="Right">The <see cref="ScopeSpace{ScopeImplementation}"/> that defines the second dimension.</param>
  /// <typeparam name="TLeft">The type of <see cref="Scope{ScopeImplementation}"/> that ise used to describe the first dimension.</typeparam>
  /// <typeparam name="TRight">The type of <see cref="Scope{ScopeImplementation}"/> that ise used to describe the first dimension.</typeparam>
  /// <returns>The requested <see cref="ScopeSpace{ScopeImplementation}"/>.</returns>
  public static CompositeScope<TLeft, TRight>.Space Composite<TLeft, TRight>(ScopeSpace<TLeft> Left,
    ScopeSpace<TRight> Right)
    where TLeft : Scope<TLeft>
    where TRight : Scope<TRight>
  {
    return new(Left, Right);
  }

  /// <summary>
  /// Gets a <see cref="ScopeSpace{ScopeImplementation}"/> that is based on scopes which supply and demand tokens.
  /// </summary>
  /// <typeparam name="Token">The type of token to supply and/or demand.</typeparam>
  /// <returns>The requested <see cref="ScopeSpace{ScopeImplementation}"/>.</returns>
  public static SupplyAndDemandScope<Token>.Space SupplyAndDemand<Token>()
  {
    return new();
  }
}