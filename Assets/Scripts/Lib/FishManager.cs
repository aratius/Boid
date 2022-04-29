using UnityEngine;
using LitJson;
using Cysharp.Threading.Tasks;

/// <summary>
/// 魚の種を管理する
/// イベント駆動にしたい
/// </summary>
public class FishManager
{

  public void Init()
  {
    this._UpdateFish();
  }


  private async UniTask _UpdateFish()
  {
    JsonData data = await Api.GetList();
    JsonData fishes = data["body"]["Items"];
    for (int i = 0; i < fishes.Count; i++)
    {
      JsonData fishJson = fishes[i];
      // FishData fishData = new FishData(fishJson);
    }
  }
}