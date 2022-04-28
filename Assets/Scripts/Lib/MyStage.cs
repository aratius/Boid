using UnityEngine;

public class MyStage
{

  public static float RIGHT = 10f;
  public static float LEFT = -10f;
  public static float TOP = 5f;
  public static float BOTTOM = -5f;

  /// <summary>
  /// 画面外に出たときに逆側から登場するように丸めた位置情報
  /// </summary>
  /// <param name="pos"></param>
  /// <returns></returns>
  public static Vector3 getPosition(Vector3 pos)
  {
    return MyStage.getPositionAddBias(pos, 0f, 0f);
  }

  /// <summary>
  /// 画面外に出たときに逆側から登場するように丸めた位置情報
  /// </summary>
  /// <param name="pos"></param>
  /// <returns></returns>
  public static Vector3 getPositionAddBias(Vector3 pos, float x, float y)
  {
    return MyStage.getPositionAddBias(pos, x, x, y, y);
  }

  /// <summary>
  /// 画面外に出たときに逆側から登場するように丸めた位置情報
  /// </summary>
  /// <param name="pos"></param>
  /// <returns></returns>
  public static Vector3 getPositionAddBias(Vector3 pos, float l, float r, float t, float b)
  {
    float right = MyStage.RIGHT + r;
    float left = MyStage.LEFT - l;
    float top = MyStage.TOP + t;
    float bottom = MyStage.BOTTOM - b;

    return MyStage._getPosition(pos, left, right, top, bottom);
  }

  private static Vector3 _getPosition(Vector3 pos, float l, float r, float t, float b)
  {
    if (pos.x > r)
    {
      pos.x = l;
    }
    else if (pos.x < l)
    {
      pos.x = r;
    }
    else if (pos.y > t)
    {
      pos.y = b;
    }
    else if (pos.y < b)
    {
      pos.y = t;
    }
    return pos;
  }

}