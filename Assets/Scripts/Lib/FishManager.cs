using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitJson;

/// <summary>
/// 魚の種を管理する
/// getOneメソッドを呼ばれたときに、いま作成するべき優先度のアルゴリズムから一匹の魚を返すか
/// </summary>
public class FishManager
{

  private Dictionary<string, FishData> _data = new Dictionary<string, FishData>();
  private List<string> _appearing = new List<string>();  // 登場中の魚管理
  private bool _isLooping = false;

  public void Init()
  {
    this._UpdateLoop();
  }

  /// <summary>
  /// TODO: 一匹要求されてどれを返すかのアルゴリズムをここで
  /// </summary>
  /// <returns></returns>
  public FishData getOne()
  {
    // データないときはnull返す
    if (this._data.Count == 0) return null;

    // Listを作る
    List<FishData> dataList = new List<FishData>();
    foreach (string id in this._data.Keys) { dataList.Add(this._data[id]); }

    // PRIORITY1 画面上に存在しないやつ
    Comparison<FishData> compareByAppearing = new Comparison<FishData>(this._CompareByAppearing);
    // PRIORITY2 作成新しい順
    Comparison<FishData> compareByCreation = new Comparison<FishData>(this._CompareByCreation);

    // NOTE: Sortは優先度低い順に
    // PRIORITY2 作成新しい順
    dataList.Sort(compareByCreation);
    // PRIORITY1 画面上に存在しないやつ
    dataList.Sort(compareByAppearing);

    // 最初のものを返す
    return dataList[0];
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

  private async void _UpdateLoop()
  {
    if (this._isLooping) return;
    this._isLooping = true;

    while (true)
    {
      await this._UpdateFish();
      await UniTask.Delay(10000);
    }
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

  /// <summary>
  /// 作成順にソート
  /// </summary>
  /// <param name="fish1"></param>
  /// <param name="fish2"></param>
  /// <returns></returns>
  private int _CompareByCreation(FishData fish1, FishData fish2)
  {
    // sortkeyが大きいほうが最新 => return -1
    if (fish1.sortkey > fish2.sortkey) return -1;
    else return 1;
  }

  /// <summary>
  /// 画面上にいる匹数でソート
  /// </summary>
  /// <param name="fish1"></param>
  /// <param name="fish2"></param>
  /// <returns></returns>
  private int _CompareByAppearing(FishData fish1, FishData fish2)
  {
    int cnt1 = 0;
    int cnt2 = 0;
    foreach (string id in this._appearing)
    {
      if (fish1.id == id) cnt1++;
      if (fish2.id == id) cnt2++;
    }
    if (cnt1 == cnt2) return 0;
    else if (cnt1 < cnt2) return -1;  // 登場回数が少ない方を前に => return -1
    else return 1;
  }

}