using UnityEngine;
using System.Collections.Generic;
using LitJson;

public class FishData
{
  public string id;
  public int sortkey;
  public List<Vector2> points = new List<Vector2>();
  public Vector2 center;
  public List<string> parentIds = new List<string>();
  public int generation;
  public string image;

  public FishData(JsonData json)
  {
    this.id = json["id"].ToString();
    this.sortkey = (int)json["sortkey"];
    this.generation = JsonUtils.ToInt(json["generation"]);
    // center
    JsonData c = JsonMapper.ToObject<JsonData>(json["center"].ToString());
    this.center = new Vector2(JsonUtils.ToFloat(c["x"]), JsonUtils.ToFloat(c["y"]));
    // // parentIds
    JsonData pIds = json["parentIds"];
    for (int i = 0; i < pIds.Count; i++)
    {
      string id = pIds[i].ToString();
      this.parentIds.Add(id);
    }
    // points
    JsonData ps = JsonMapper.ToObject(json["points"].ToString());
    for (int i = 0; i < ps.Count; i++)
    {
      JsonData pJson = ps[i];
      Vector2 p = new Vector2(JsonUtils.ToFloat(pJson["x"]), JsonUtils.ToFloat(pJson["y"]));
      this.points.Add(p);
    }

    this.image = json["image"].ToString();

  }

  public FishData(
    in string id,
    in int sortkey,
    in List<Vector2> points,
    in Vector2 center,
    in List<string> parentIds,
    in int generation,
    in string image
  )
  {
    this.id = id;
    this.sortkey = sortkey;
    this.points = points;
    this.center = center;
    this.parentIds = parentIds;
    this.generation = generation;
    this.image = image;
  }

}