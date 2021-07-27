using System;
using System.Collections.Generic;

namespace RPG.Core
{
  /// <summary>
  /// allows us to build a Conjunctive Normal Form
  /// </summary>
  [Serializable]
  public class Condition
  {
    public Disjunction[] and;

    public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
    {
      foreach (var disjunction in and)
      {
        if (disjunction.Check(evaluators) == false)
        {
          return false;
        }
      }

      return true;
    }

    [Serializable]
    public class Disjunction
    {
      public Predicate[] or;

      public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
      {
        foreach (var predicate in or)
        {
          if (predicate.Check(evaluators))
          {
            return true;
          }
        }

        return false;
      }
    }

    [Serializable]
    public class Predicate
    {
      public string predicate;
      public string[] parameters;
      public bool negate;

      public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
      {
        foreach (var evaluator in evaluators)
        {
          var result = evaluator.Evaluate(predicate, parameters);

          if (result == null) continue;

          if (result == negate) return false;
        }

        return true;
      }
    }
  }
}