using UnityEngine;

public class Fish : MonoBehaviour
{

    private float _progress = 0f;

    /// <summary>
    /// 位置
    /// </summary>
    /// <value></value>
    public Vector3 position
    {
        get { return Vector3.zero; }
    }

    /// <summary>
    /// 速度
    /// </summary>
    /// <value></value>
    public Vector3 velocity
    {
        get { return Vector3.zero; }
    }

    /// <summary>
    /// 方向
    /// </summary>
    /// <value></value>
    public Vector3 direction
    {
        get { return Vector3.Normalize(this.velocity); }
    }

    void Start()
    {

    }

    void Update()
    {
        const float COEFFICIENT = 0.1f;  // 係数
        this._progress += this.velocity.magnitude * COEFFICIENT;
        // TODO: ShaderにTimeProgressをセット
    }

    /// <summary>
    /// 外部からの加速
    /// </summary>
    /// <param name="vel"></param>
    public void AddVelocity(Vector3 vel)
    {

    }

    /// <summary>
    /// 画像をセット
    /// </summary>
    /// <param name="image"></param>
    public void SetImage(string image)
    {
        // TODO: シェーダーに画像を渡す
    }

}