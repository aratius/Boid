using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class Debugger : MonoBehaviour
{
  [SerializeField] private Text _fishCount;
  [SerializeField] private Text _fps;
  [SerializeField] private GameObject _eventGroup;
  [SerializeField] private GameObject _eventPrefab;
  private List<GameObject> _events = new List<GameObject>();

  /// <summary>
  ///
  /// </summary>
  void Start()
  {
    this._Measure();
    this._AddEvent();
  }

  /// <summary>
  ///
  /// </summary>
  void Update()
  {

  }

  /// <summary>
  /// 計測
  /// 最初に一回だけ実行すれば良い
  /// 毎フレームやるのコスト高そうだから
  /// </summary>
  private async void _Measure()
  {
    while (true)
    {
      float fps = 1 / Time.deltaTime;
      this._fps.text = $"fps : {Mathf.Round(fps)}";

      GameObject[] fishes = GameObject.FindGameObjectsWithTag("Fish");
      this._fishCount.text = $"fishCount : {fishes.Length}";

      await UniTask.Delay(1000);
    }
  }

  /// <summary>
  /// 最初に一度イベント登録
  /// </summary>
  private void _AddEvent()
  {
    this._eventPrefab.SetActive(false);
    FishEvents.onBorn.AddListener(this._onBorn);
    FishEvents.onDie.AddListener(this._onDie);
  }

  /// <summary>
  /// だれかが生まれたとき
  /// </summary>
  /// <param name="id"></param>
  private void _onBorn(string id)
  {
    GameObject eventGO = Instantiate(this._eventPrefab, this._eventGroup.transform);
    eventGO.SetActive(true);
    Text t = eventGO.GetComponent<Text>();
    t.text = $"{id} was born";

    this._UpdateEvents(eventGO);
  }

  /// <summary>
  /// 誰かが死んだとき
  /// </summary>
  /// <param name="id"></param>
  private void _onDie(string id)
  {
    GameObject eventGo = Instantiate(this._eventPrefab, this._eventGroup.transform);
    eventGo.SetActive(true);
    Text t = eventGo.GetComponent<Text>();
    t.text = $"{id} was dead";

    this._UpdateEvents(eventGo);
  }

  /// <summary>
  /// だれかのライフイベントが発生したとき
  /// </summary>
  /// <param name="go"></param>
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
