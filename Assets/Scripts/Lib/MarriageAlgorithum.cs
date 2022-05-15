using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 結婚アルゴリズム
/// </summary>
public class MarriageAlgorithum
{
  public static Fish getPartner(
    in Fish me,
    in List<Fish> others
  )
  {
    Fish partner = null;

    // 既婚
    if (me.partner != null) return partner;
    if (me.age < 18) return partner;

    for (int i = 0; i < others.Count; i++)
    {
      Fish other = others[i];
      // 性別が違う
      if (other.partner != null) continue;
      if (me.sex != other.sex) continue;
      if (other.age < 18) continue;

      float dist = Vector3.Distance(me.position, other.position);
      if (dist < 2)
      {
        float marriageProbability = Time.deltaTime / 10;
        if (Random.Range(0f, 1f) < marriageProbability) partner = other;
      }
    }

    return partner;
  }
}