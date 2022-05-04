using UnityEngine;

public abstract class BoidModel : MonoBehaviour
{

  public abstract float size { get; set; }
  public abstract Vector3 position { get; set; }
  public abstract float direction { get; }

}