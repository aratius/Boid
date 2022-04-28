using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

  [SerializeField] GameObject stage;
  [SerializeField] GameObject fishPrefab;
  [SerializeField] int fishCount = 100;

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
      Vector3 addPos = Vector3.zero;
      float nearCount = 1e-4f;

      for (int j = 0; j < this._fishes.Count; j++)
      {
        if (i == j) continue;
        Fish other = this._fishes[j];
        float dist = Vector3.Distance(me.position, other.position);
        const float DIST_THRESHOLD = 1f;

        if (dist < DIST_THRESHOLD)
        {
          // 反発
          addVel += -1f * Vector3.Normalize(other.position - me.position) * (DIST_THRESHOLD - dist) * 0.03f;
          // 方向平均
          addDir += other.direction;
          // 位置平均
          addPos += other.position;

          nearCount++;
        }
      }
      addDir /= nearCount;
      addPos /= nearCount;

      addVel += Vector3.Normalize(addPos - me.position) * 0.001f;
      addVel += new Vector3(Mathf.Sin(addDir), Mathf.Cos(addDir), 0f) * 0.001f;

      me.AddVelocity(addVel);
    }
  }
}
