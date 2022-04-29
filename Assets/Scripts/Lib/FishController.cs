using System.Collections;
using System.Collections.Generic;
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
    for (int i = 0; i < _fishCount; i++)
    {
      GameObject fish = Instantiate(_fishPrefab, this._stage.transform);
      Fish script = fish.GetComponent<Fish>();
      Vector3 randomPos = new Vector3(Random.Range(-10f, 10f), Random.Range(-5f, 5f), 0f);
      script.position = randomPos;
      string image = "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";
      script.Init(image, 0);
      this._fishes.Add(script);
    }

    this._fishManager.Init();
  }

  // Update is called once per frame
  void Update()
  {
    for (int i = 0; i < this._fishes.Count; i++)
    {
      Fish me = this._fishes[i];
      Vector3 addVel = Vector3.zero;
      float addDir = 0;
      float dirCount = 1e-4f;
      Vector3 addPos = Vector3.zero;
      float posCount = 1e-4f;

      for (int j = 0; j < this._fishes.Count; j++)
      {
        if (i == j) continue;
        Fish other = this._fishes[j];
        float dist = Vector3.Distance(me.position, other.position);

        // 反発
        if (dist < this._THRESHOLD_REFRECT)
        {
          addVel += -1f * Vector3.Normalize(other.position - me.position) * (this._THRESHOLD_REFRECT - dist) * 0.05f * this._POWER_REFRECT;
        }

        // 位置平均
        if (dist < this._THRESHOLD_POS)
        {
          addPos += other.position;
          posCount++;
        }

        // 方向平均
        if (dist < this._THRESHOLD_DIR)
        {
          addDir += other.direction;
          dirCount++;
        }
      }
      addPos /= posCount;
      addDir /= dirCount;

      addVel += Vector3.Normalize(addPos - me.position) * 0.001f * this._POWER_POS;
      addVel += new Vector3(Mathf.Sin(addDir), Mathf.Cos(addDir), 0f) * 0.001f * this._POWER_DIR;

      me.AddVelocity(addVel);
    }
  }
}
