using System.Collections.Generic;

namespace RPG.Stats
{
  /// <summary>
  /// to help implement the bonus system
  /// game objects implementing this interface will have
  /// flat bonuses
  /// percent bonuses
  /// or both
  ///
  /// Modifiers are only for the players
  /// it won't be implemented on the Player
  /// </summary>
  public interface IModifierProvider
  {
    // additive modifier is a flat bonus system
    IEnumerable<float> GetAdditiveModifiers(Stat stat);

    IEnumerable<float> GetPercentageModifier(Stat stat);
  }
}