using System.Collections.Generic;
using LitJson;
using Cysharp.Threading.Tasks;

/// <summary>
/// 魚の種を管理する
/// イベント駆動にしたい
/// </summary>
public class FishManager
{

  private Dictionary<string, FishData> _data = new Dictionary<string, FishData>();

  public void Init()
  {
    this._UpdateFish();
  }

  /// <summary>
  /// TODO: 一匹要求されてどれを返すかのアルゴリズムをここで
  /// </summary>
  /// <returns></returns>
  public FishData getOne()
  {
    FishData _d = null;
    foreach (string id in this._data.Keys)
    {
      _d = this._data[id];
    }
    return _d;
  }

  /// <summary>
  /// ポーリング実行
  /// </summary>
  /// <returns></returns>
  private async UniTask _UpdateFish()
  {
    JsonData data = await Api.GetList();
    JsonData fishes = data["body"]["Items"];
    for (int i = 0; i < fishes.Count; i++)
    {
      JsonData fishJson = fishes[i];
      FishData fishData = new FishData(fishJson);

      string id = fishData.id;
      if (!this._data.ContainsKey(id))
      {
        this._data.Add(id, fishData);
      }
    }
  }
}