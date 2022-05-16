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

  public string ToJson()
  {
    return JsonMapper.ToJson(
      new FishJson(
        this.id,
        this.sortkey,
        this.points,
        this.center,
        this.parentIds,
        this.generation,
        this.image
      )
    );
  }

}

class V2
{
  public float x;
  public float y;
  public V2(float _x, float _y)
  {
    this.x = _x;
    this.y = _y;
  }
}

class FishJson
{
  public string id;
  public int sortkey;
  public List<V2> points = new List<V2>();
  public V2 center;
  public List<string> parentIds = new List<string>();
  public int generation;
  public string image;

  public FishJson(
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
    this.parentIds = parentIds;
    this.generation = generation;
    this.image = image;

    // Vector2をV2に変換
    List<V2> ps = new List<V2>();
    for (int i = 0; i < points.Count; i++)
    {
      ps.Add(new V2(points[i].x, points[i].y));
    }
    this.points = ps;

    this.center = new V2(center.x, center.y);
  }
}