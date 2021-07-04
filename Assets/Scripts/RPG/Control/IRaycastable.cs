namespace RPG.Control
{
  /// <summary>
  /// objects will implement that can interact with the Raycast
  /// </summary>
  public interface IRaycastable
  {
    bool HandleRaycast(PlayerController callingController);

    /**
     * The object which the raycast hits handles which cursor
     * it wants to be displayed
     */
    CursorType GetCursorType();
  }
}