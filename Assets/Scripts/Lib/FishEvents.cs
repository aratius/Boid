using UnityEngine.Events;

public class FishEvents
{
  public static UnityEvent<string> onBorn = new UnityEvent<string>();
  public static UnityEvent<string> onDie = new UnityEvent<string>();
}