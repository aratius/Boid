using System.Collections.Generic;
using LitJson;
using Cysharp.Threading.Tasks;

/// <summary>
/// 魚の種を管理する
/// イベント駆動にしたい
/// </summary>
public class FishManager
{

  private List<FishData> _dataList = new List<FishData>();

  public void Init()
  {
    this._UpdateFish();
  }

  /// <summary>
  /// 一匹要求されてどれを返すかのアルゴリズムをここで
  /// </summary>
  /// <returns></returns>
  public FishData getOne()
  {
    return this._dataList[0];
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
      bool found = false;
      for (int j = 0; j < this._dataList.Count; j++)
      {
        string otherId = this._dataList[j].id;
        if (id == otherId) found = true;
      }

      if (!found) this._dataList.Add(fishData);
    }
  }
}