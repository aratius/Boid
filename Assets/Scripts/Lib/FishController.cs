using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 魚のGameObjectをまとめるクラス
/// </summary>
public class FishController : MonoBehaviour
{

  [SerializeField] private GameObject _stage;
  [SerializeField] private GameObject _fishPrefab;
  [SerializeField] private int _FISH_COUNT = 100;

  [Header("反発")]
  [SerializeField, Range(0.01f, 5f)] private float _THRESHOLD_REFRECT = 0.5f;
  [SerializeField, Range(0.01f, 5f)] private float _POWER_REFRECT = 1f;
  [Header("位置平均")]
  [SerializeField, Range(0.01f, 5f)] private float _THRESHOLD_POS = 0.5f;
  [SerializeField, Range(0.01f, 5f)] private float _POWER_POS = 1f;
  [Header("方向平均")]
  [SerializeField, Range(0.01f, 5f)] private float _THRESHOLD_DIR = 0.5f;
  [SerializeField, Range(0.01f, 5f)] private float _POWER_DIR = 1f;

  private FishManager _fishManager = new FishManager();
  private List<Fish> _fishes = new List<Fish>();

  void Start()
  {
    for (int i = 0; i < 10; i++)
    {
      Vector3 randomPos = new Vector3(Random.Range(-10f, 10f), Random.Range(-5f, 5f), 0f);
      int age = Random.Range(0, 70);
      this._CreateOne(FishDummy.instance, randomPos, age);
    }

    this._fishManager.Init();
  }

  void Update()
  {

    if (this._fishes.Count < this._FISH_COUNT)
    {
      FishData fishData = this._fishManager.GetOne();
      if (fishData != null)
      {
        Vector3 pos = new Vector3(0f, MyStage.BOTTOM - 1f, 0f);
        this._CreateOne(fishData, pos);
      }
    }

    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

    for (int i = 0; i < this._fishes.Count; i++)
    {
      Fish fish = this._fishes[i];
      fish.LookAround(
        this._fishes,
        enemies,
        this._THRESHOLD_REFRECT,
        this._POWER_REFRECT,
        this._THRESHOLD_POS,
        this._POWER_POS,
        this._THRESHOLD_DIR,
        this._POWER_DIR
      );
    }
  }

  /// <summary>
  /// 一体作成
  /// </summary>
  /// <param name="data"></param>
  /// <param name="pos"></param>
  private void _CreateOne(FishData data, Vector3 pos, int age = 0)
  {
    GameObject go = Instantiate(_fishPrefab, this._stage.transform);
    Fish fish = go.GetComponent<Fish>();
    fish.position = pos;
    fish.Born(data);
    fish.SetAge(age);
    fish.onDie.AddListener(this._OnDie);
    if (fish.sex == Sex.Female) fish.onGetPregnant.AddListener(this._OnGetPregnant);
    this._fishes.Add(fish);
    this._fishManager.Add(data.id);

    // グローバルにイベント発火
    FishEvents.onBorn.Invoke(fish.data.id);
  }

  /// <summary>
  /// 魚の死
  /// </summary>
  /// <param name="fish"></param>
  private void _OnDie(Fish fish)
  {
    fish.Cremate();
    fish.onDie.RemoveListener(this._OnDie);
    this._fishes.Remove(fish);
    this._fishManager.Remove(fish.data.id);

    // グローバルにイベント発火
    FishEvents.onDie.Invoke(fish.data.id);
  }

  /// <summary>
  /// 魚の妊娠
  /// 卵ではない
  /// </summary>
  /// <param name="mammy"></param>
  /// <returns></returns>
  private async void _OnGetPregnant(Fish mammy)
  {
    Fish daddy = mammy.partner;
    if (mammy == null || daddy == null) return;

    FishData child = await this._fishManager.GetChild(daddy, mammy);
    if (mammy != null) this._CreateOne(child, mammy.position);
  }

}