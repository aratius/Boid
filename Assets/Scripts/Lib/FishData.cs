using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class FishData
{

  public static JsonData DUMMY
  {
    get
    {
      JsonData data = JsonMapper.ToObject("{'id': '', 'points': [], center: {'x': 0, 'y': 0}}");
      return data;
    }
  }

  public string id;
  public List<Vector2> points;
  public Vector2 center;
  public string[] parentIds;
  public int generation;

  public FishData(JsonData json)
  {

  }

}