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
    if (pos.x > MyStage.RIGHT)
    {
      pos.x = MyStage.LEFT;
    }
    else if (pos.x < MyStage.LEFT)
    {
      pos.x = MyStage.RIGHT;
    }
    else if (pos.y > MyStage.TOP)
    {
      pos.y = MyStage.BOTTOM;
    }
    else if (pos.y < MyStage.BOTTOM)
    {
      pos.y = MyStage.TOP;
    }
    return pos;
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

    if (pos.x > right)
    {
      pos.x = left;
    }
    else if (pos.x < left)
    {
      pos.x = right;
    }
    else if (pos.y > top)
    {
      pos.y = bottom;
    }
    else if (pos.y < bottom)
    {
      pos.y = top;
    }
    return pos;
  }

  /// <summary>
  /// 画面外に出たときに逆側から登場するように丸めた位置情報
  /// </summary>
  /// <param name="pos"></param>
  /// <returns></returns>
  public static Vector3 getPositionAddBias(Vector3 pos, float x, float y)
  {
    float right = MyStage.RIGHT + x;
    float left = MyStage.LEFT - x;
    float top = MyStage.TOP + y;
    float bottom = MyStage.BOTTOM - y;

    if (pos.x > right)
    {
      pos.x = left;
    }
    else if (pos.x < left)
    {
      pos.x = right;
    }
    else if (pos.y > top)
    {
      pos.y = bottom;
    }
    else if (pos.y < bottom)
    {
      pos.y = top;
    }
    return pos;
  }

}