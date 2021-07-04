namespace RPG.Saving
{
  /// <summary>
  /// interface responsible for implementing the saving and restoring behaviour
  /// in Components that implements it
  /// </summary>
  public interface ISavable
  {
    object CaptureState();
    
    void RestoreState(object state);
  }
}