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

}