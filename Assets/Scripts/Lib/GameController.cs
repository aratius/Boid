using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameController : MonoBehaviour
{

  [SerializeField] private GameObject _debug;

  void Start()
  {
    Debug.unityLogger.logEnabled = false;
  }

  void Update()
  {
    if (Input.GetKeyUp(KeyCode.D))
    {
      this._debug.SetActive(!this._debug.active);
    }
  }

}
