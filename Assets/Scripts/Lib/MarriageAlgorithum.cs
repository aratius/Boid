using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 結婚アルゴリズム
/// </summary>
public class MarriageAlgorithum
{
  public static void getPartner(
    in Fish me,
    in List<Fish> others
  )
  {
    Fish partner = null;

    // 既婚か18歳未満
    if (
      me.partner != null ||
      me.age < 18
    ) return;

    for (int i = 0; i < others.Count; i++)
    {
      Fish other = others[i];

      // NOTE: 相手を検証する順番も重要
      // NOTE: 条件式がfalse担った時点でその後の条件は評価されない
      // NOTE: これは現実と近い
      if (
        me.sex != other.sex ||  // 性別は？そもそも恋愛対象に入るかどうか LGBTQの実装はまだ
        other.age < 18 ||  // 年齢は結婚できる年齢か？
        other.partner != null  // 独身か？
      ) continue;

      float dist = Vector3.Distance(me.position, other.position);
      if (dist < 1.5)
      {
        float marriageProbability = Time.deltaTime / 20;
        if (Random.Range(0f, 1f) < marriageProbability) partner = other;
      }
    }

    if (partner != null)
    {
      me.MarrigeWith(partner);
      partner.MarrigeWith(me);
    }
  }
}