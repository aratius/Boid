using System;
using LitJson;

public class JsonUtils
{

  public static int ToInt(JsonData json)
  {
    return int.Parse(json.ToString());
  }

  public static float ToFloat(JsonData json)
  {
    return float.Parse(json.ToString());
  }

}