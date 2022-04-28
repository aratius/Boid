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

  }
}
