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
    Debug.Log(data);
  }
}