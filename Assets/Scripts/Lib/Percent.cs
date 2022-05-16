using UnityEngine;

public class Percent
{

  public static bool Get(float percent)
  {
    return Random.Range(0f, 1f) < percent;
  }

  /// <summary>
  /// 年齢による発生確率
  /// </summary>
  /// <param name="age"></param>
  /// <param name="peak"></param>
  /// <param name="range"></param>
  /// <returns></returns>
  public static float AgeRange(float age, float peak, float range)
  {
    float percent = (range - Mathf.Abs(peak - age)) / range;
    return Mathf.Max(Mathf.Min(percent, 1f), 0f);
  }

  /// <summary>
  /// 年齢による発生確率
  /// </summary>
  /// <param name="age"></param>
  /// <param name="peak"></param>
  /// <param name="range"></param>
  /// <returns></returns>
  public static float AgeMax(float age, float max, float range)
  {
    float percent = (range - (max - age)) / range;
    return Mathf.Max(Mathf.Min(percent, 1f), 0f);
  }

  /// <summary>
  /// 年齢による発生確率
  /// </summary>
  /// <param name="age"></param>
  /// <param name="peak"></param>
  /// <param name="range"></param>
  /// <returns></returns>
  public static float AgeMin(float age, float max, float range)
  {
    float percent = (range + (max - age)) / range;
    return Mathf.Max(Mathf.Min(percent, 1f), 0f);
  }

}