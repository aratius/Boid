using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class FishData
{

  public string id;
  public List<Vector2> points;
  public Vector2 center;
  public string[] parentIds;
  public int generation;

  public FishData(JsonData json)
  {

  }

}