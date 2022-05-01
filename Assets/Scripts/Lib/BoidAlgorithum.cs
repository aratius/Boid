using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Boidアルゴリズム
/// </summary>
public class BoidAlgorithum
{
  public static Vector3 getVelociry(
    in BoidModel me,
    in List<BoidModel> others,
    in float THRESHOLD_REFRECT,
    in float POWER_REFRECT,
    in float THRESHOLD_POS,
    in float POWER_POS,
    in float THRESHOLD_DIR,
    in float POWER_DIR
  )
  {
    Vector3 addVel = Vector3.zero;
    float addDir = 0;
    float dirCount = 1e-4f;
    Vector3 addPos = Vector3.zero;
    float posCount = 1e-4f;

    for (int j = 0; j < others.Count; j++)
    {
      BoidModel other = others[j];
      if (me.position == other.position) continue;
      float dist = Vector3.Distance(me.position, other.position);

      // 反発
      if (dist < THRESHOLD_REFRECT)
      {
        addVel += (
          -1f
          * Vector3.Normalize(other.position - me.position)
          * (THRESHOLD_REFRECT - dist) * 0.05f * POWER_REFRECT
        );
      }

      // 位置平均
      if (dist < THRESHOLD_POS)
      {
        addPos += other.position;
        posCount++;
      }

      // 方向平均
      if (dist < THRESHOLD_DIR)
      {
        addDir += other.direction;
        dirCount++;
      }
    }
    addPos /= posCount;
    addDir /= dirCount;

    addVel += Vector3.Normalize(addPos - me.position) * 0.001f * POWER_POS;
    addVel += new Vector3(Mathf.Sin(addDir), Mathf.Cos(addDir), 0f) * 0.001f * POWER_DIR;

    return addVel;
  }
}