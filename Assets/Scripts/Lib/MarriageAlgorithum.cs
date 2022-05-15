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

    // 既婚か18歳未満
    if (
      me.partner != null ||
      me.age < 18
    ) return partner;

    for (int i = 0; i < others.Count; i++)
    {
      Fish other = others[i];

      // NOTE: 相手を検証する順番も重要
      // NOTE: 条件式がfalse担った時点でその後の条件は評価されない
      // NOTE: これは現実と近い
      if (
        me.sex != other.sex ||
        other.age < 18 ||
        other.partner != null
      ) continue;

      float dist = Vector3.Distance(me.position, other.position);
      if (dist < 1.5)
      {
        float marriageProbability = Time.deltaTime / 20;
        if (Random.Range(0f, 1f) < marriageProbability) partner = other;
      }
    }

    return partner;
  }
}