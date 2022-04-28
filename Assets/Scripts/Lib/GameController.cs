using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

  [SerializeField] GameObject stage;
  [SerializeField] GameObject fishPrefab;
  [SerializeField] int fishCount = 100;

  [Header("反発")]
  [SerializeField, Range(0.01f, 5f)] private float _THRESHOLD_REFRECT = 0.5f;
  [SerializeField, Range(0.01f, 5f)] private float _POWER_REFRECT = 1f;
  [Header("位置平均")]
  [SerializeField, Range(0.01f, 5f)] private float _THRESHOLD_POS = 0.5f;
  [SerializeField, Range(0.01f, 5f)] private float _POWER_POS = 1f;
  [Header("方向平均")]
  [SerializeField, Range(0.01f, 5f)] private float _THRESHOLD_DIR = 0.5f;
  [SerializeField, Range(0.01f, 5f)] private float _POWER_DIR = 1f;

  private List<Fish> _fishes = new List<Fish>();

  // Start is called before the first frame update
  void Start()
  {
    for (int i = 0; i < fishCount; i++)
    {
      GameObject fish = Instantiate(fishPrefab, this.stage.transform);
      Fish script = fish.GetComponent<Fish>();
      Vector3 randomPos = new Vector3(Random.Range(-10f, 10f), Random.Range(-5f, 5f), 0f);
      script.position = randomPos;
      this._fishes.Add(script);
    }
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

        if (dist < this._THRESHOLD_REFRECT)
        {
          // 反発
          addVel += -1f * Vector3.Normalize(other.position - me.position) * (this._THRESHOLD_REFRECT - dist) * 0.05f * this._POWER_REFRECT;
        }

        if (dist < this._THRESHOLD_POS)
        {
          // 位置平均
          addPos += other.position;
          posCount++;
        }

        if (dist < this._THRESHOLD_DIR)
        {
          // 方向平均
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
