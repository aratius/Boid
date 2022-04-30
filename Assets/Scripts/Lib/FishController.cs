using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class FishController : MonoBehaviour
{

  [SerializeField] private GameObject _stage;
  [SerializeField] private GameObject _fishPrefab;
  [SerializeField] private int _fishCount = 100;

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

  // Start is called before the first frame update
  void Start()
  {
    for (int i = 0; i < 10; i++)
    {
      Vector3 randomPos = new Vector3(Random.Range(-10f, 10f), Random.Range(-5f, 5f), 0f);
      this._CreateOne(FishDummy.instance, randomPos);
    }

    this._fishManager.Init();
  }

  // Update is called once per frame
  void Update()
  {

    FishData fishData = this._fishManager.getOne();
    if (fishData != null)
    {
      GameObject fish = Instantiate(_fishPrefab, this._stage.transform);
      Fish script = fish.GetComponent<Fish>();
      Vector3 pos = new Vector3(0f, MyStage.BOTTOM - 1f, 0f);
      script.position = pos;
      script.Born(
        Regex.Replace(fishData.image, "data:image/(png|jpe??g);base64,", ""),
        fishData.generation
      );
      this._fishes.Add(script);
      this._fishManager.Add(fishData.id);
    }

    for (int i = 0; i < this._fishes.Count; i++)
    {
      Fish me = this._fishes[i];
      me.LookAround(
        this._fishes,
        this._THRESHOLD_REFRECT,
        this._POWER_REFRECT,
        this._THRESHOLD_POS,
        this._POWER_POS,
        this._THRESHOLD_DIR,
        this._POWER_DIR
      );
    }
  }

  private void _CreateOne(
    FishData data,
    Vector3 pos
  )
  {
    GameObject fish = Instantiate(_fishPrefab, this._stage.transform);
    Fish script = fish.GetComponent<Fish>();
    script.position = pos;
    script.Born(
      Regex.Replace(data.image, "data:image/(png|jpe??g);base64,", ""),
      data.generation
    );
    this._fishes.Add(script);
    this._fishManager.Add(data.id);
  }

}
