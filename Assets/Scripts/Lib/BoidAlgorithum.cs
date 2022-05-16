using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Boidアルゴリズム
/// </summary>
public class BoidAlgorithum
{
  public static Vector3 getVelociry(
    in Fish me,
    in List<Fish> others,
    in GameObject[] enemies,
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
      Fish other = others[j];
      if (me.position == other.position) continue;
      float dist = Vector3.Distance(me.position, other.position);

      bool isPartner = false;
      if (other.partner != null && me.data.id == other.partner.data.id) isPartner = true;
      bool isFamily = false;
      if ((
          me.data.parentIds.Count > 0 &&
          other.data.parentIds.Count > 0
      ))
      {
        if (
          (
            me.data.id == other.data.parentIds[0] ||
            me.data.id == other.data.parentIds[1]
          ) ||
          (
            other.data.id == me.data.parentIds[0] ||
            other.data.id == me.data.parentIds[1]
          )
        ) isFamily = true;
      }

      // 反発
      if (dist < THRESHOLD_REFRECT * other.size)
      {

        // NOTE: 身内じゃなければ
        if (!isPartner && !isFamily)
        {
          addVel += (
            -1f  // 逆方向
            * Vector3.Normalize(other.position - me.position)
            * (THRESHOLD_REFRECT * other.size - dist) * 0.05f * POWER_REFRECT
          );
        }
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

    for (int i = 0; i < enemies.Length; i++)
    {
      GameObject enemy = enemies[i];
      float dist = Vector3.Distance(me.position, enemy.transform.position);

      if (dist < THRESHOLD_REFRECT)
      {
        addVel += (
          -1f  // 逆方向
          * Vector3.Normalize(enemy.transform.position - me.position)
          * (THRESHOLD_REFRECT - dist) * 0.05f * POWER_REFRECT
        );
      }
    }

    addPos /= posCount;
    addDir /= dirCount;

    addVel += Vector3.Normalize(addPos - me.position) * 0.001f * POWER_POS;
    addVel += new Vector3(Mathf.Sin(addDir), Mathf.Cos(addDir), 0f) * 0.001f * POWER_DIR;

    return addVel;
  }
}