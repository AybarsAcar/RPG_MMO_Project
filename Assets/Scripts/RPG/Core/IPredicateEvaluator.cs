namespace RPG.Core
{
  /// <summary>
  /// it will evaluate predicates
  /// used in the Dialogue system
  /// </summary>
  public interface IPredicateEvaluator
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate">function name</param>
    /// <param name="parameters"></param>
    /// <returns>nullable boolean</returns>
    bool? Evaluate(string predicate, string[] parameters);
  }
}