using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class Debugger : MonoBehaviour
{
  [SerializeField] private Text _fps;
  [SerializeField] private GameObject _eventGroup;
  [SerializeField] private GameObject _eventPrefab;
  private List<GameObject> _events = new List<GameObject>();

  /// <summary>
  ///
  /// </summary>
  void Start()
  {
    this._MeasureFps();
    this._AddEvent();
  }

  /// <summary>
  ///
  /// </summary>
  void Update()
  {
  }

  /// <summary>
  /// FPS計測
  /// </summary>
  private async void _MeasureFps()
  {
    while (true)
    {
      float fps = 1 / Time.deltaTime;
      this._fps.text = $"fps : {Mathf.Round(fps)}";
      await UniTask.Delay(1000);
    }
  }

  private void _AddEvent()
  {
    this._eventPrefab.SetActive(false);
    FishEvents.onBorn.AddListener(this._onBorn);
    FishEvents.onDie.AddListener(this._onDie);
    Debug.Log("add event");
  }

  private void _onBorn(string id)
  {
    Debug.Log("id born");
    GameObject eventGO = Instantiate(this._eventPrefab, this._eventGroup.transform);
    eventGO.SetActive(true);
    Text t = eventGO.GetComponent<Text>();
    t.text = $"{id} was born";

    this._UpdateEvents(eventGO);
  }

  private void _onDie(string id)
  {
    Debug.Log("id die");
    GameObject eventGO = Instantiate(this._eventPrefab, this._eventGroup.transform);
    eventGO.SetActive(true);
    Text t = eventGO.GetComponent<Text>();
    t.text = $"{id} was dead";

    this._UpdateEvents(eventGO);
  }

  private void _UpdateEvents(GameObject go)
  {
    this._events.Add(go);
    if (this._events.Count > 5)
    {
      Destroy(this._events[0]);
      this._events.RemoveAt(0);
    }
  }

}
