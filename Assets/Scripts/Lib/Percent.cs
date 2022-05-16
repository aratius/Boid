using UnityEngine;

public class Percent
{

  public static bool Get(float percent)
  {
    return Random.Range(0f, 1f) < percent;
  }

}