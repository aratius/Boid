using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class FishData
{

  public static JsonData DUMMY
  {
    get
    {
      JsonData data = JsonMapper.ToObject("{'id': '', 'points': [], 'parentIds': [\"hoge\", \"hoge\"], 'center': '{\"x\": 0, \"y\": 0}', 'generation': -1}");
      return data;
    }
  }

  public string id;
  public List<Vector2> points;
  public Vector2 center;
  public List<string> parentIds;
  public int generation;
  public string image;

  public FishData(JsonData json)
  {
    this.id = json["id"].ToString();
    this.generation = JsonUtils.ToInt(json["generation"]);
    // center
    JsonData c = JsonMapper.ToObject<JsonData>(json["center"].ToString());
    this.center = new Vector2(JsonUtils.ToFloat(c["x"]), JsonUtils.ToFloat(c["y"]));
    // parentIds
    JsonData parentIds = json["parentIds"];
    if (parentIds.IsArray)
    {
      for (int i = 0; i < parentIds.Count; i++)
      {
        string id = parentIds[i].ToString();
        parentIds.Add(id);
      }
    }
    else
    {
      Debug.LogError("ParentIds is not an array.");
    }
    // points
    JsonData points = json["points"];

  }

}