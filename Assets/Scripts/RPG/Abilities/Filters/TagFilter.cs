using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Abilities.Filters
{
  [CreateAssetMenu(fileName = "New Tag Filter", menuName = "Abilities/Filter Strategy/New Tag Filter")]
  public class TagFilter : FilterStrategy
  {
    [SerializeField] private string tagToFilter;

    public override IEnumerable<GameObject> Filter(IEnumerable<GameObject> objectsToFilter)
    {
      return objectsToFilter.Where(gameObject => gameObject.CompareTag(tagToFilter));
    }
  }
}