using System.Collections.Generic;
using LitJson;
using Cysharp.Threading.Tasks;

/// <summary>
/// 魚の種を管理する
/// getOneメソッドを呼ばれたときに、いま作成するべき優先度のアルゴリズムから一匹の魚を返す
/// イベント駆動にしたい
/// </summary>
public class FishManager
{

  private Dictionary<string, FishData> _data = new Dictionary<string, FishData>();
  private List<string> _appearing = new List<string>();

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
    // NOTE: 画面上に存在しないやつ
    // NOTE: 作成新しい順
    FishData fd = null;
    foreach (string id in this._data.Keys)
    {
      fd = this._data[id];
    }
    return fd;
  }

  /// <summary>
  /// appearingに追加
  /// </summary>
  /// <param name="id"></param>
  public void Add(string id)
  {
    this._appearing.Add(id);
  }

  /// <summary>
  /// appearingから削除
  /// </summary>
  /// <param name="id"></param>
  public void Remove(string id)
  {
    this._appearing.Remove(id);
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