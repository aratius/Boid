using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

  [SerializeField] private GameObject _debug;

  void Update()
  {
    if (Input.GetKeyUp(KeyCode.D))
    {
      this._debug.SetActive(!this._debug.active);
    }
  }
}
