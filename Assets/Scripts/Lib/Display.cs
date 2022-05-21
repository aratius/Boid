using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Mode
{
  standBy,
  w,
  h
}

public class Display : MonoBehaviour
{
  private Mode _mode = Mode.standBy;

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    Vector3 scl = this.transform.localScale;
    Vector3 pos = this.transform.localPosition;
    if (Input.GetKeyUp(KeyCode.W))
    {
      scl.x *= 0.9f;
      scl.y *= 0.9f;
    }
    if (Input.GetKeyUp(KeyCode.Q))
    {
      scl.x /= 0.9f;
      scl.y /= 0.9f;
    }
    if (Input.GetKeyUp(KeyCode.R))
    {
      pos.x += 0.1f; ;
    }
    if (Input.GetKeyUp(KeyCode.T))
    {
      pos.x -= 0.1f; ;
    }
    if (Input.GetKeyUp(KeyCode.Y))
    {
      pos.y += 0.1f; ;
    }
    if (Input.GetKeyUp(KeyCode.U))
    {
      pos.y -= 0.1f; ;
    }
    this.transform.localScale = scl;
    this.transform.localPosition = pos;
  }

  /// <summary>
  ///
  /// </summary>
  private void _onChange()
  {

  }
}
