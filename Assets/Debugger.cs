using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class Debugger : MonoBehaviour
{
  [SerializeField] private Text _fps;
  // Start is called before the first frame update
  void Start()
  {
    this._MeasureFps();
  }

  // Update is called once per frame
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

}
