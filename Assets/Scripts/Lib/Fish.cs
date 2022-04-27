using UnityEngine;

/// <summary>
/// 一定progress到達したら死ぬ？（一生の間の心拍回数は決まっている？）
/// </summary>
public class Fish : MonoBehaviour
{

  public Sex sex = Sex.Male;  // 性別
  public int generation = -1;  // 世代
  private float _progress = 0f;  // 経過

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
  /// 初期設定
  /// </summary>
  /// <param name="image"></param>
  public void Init(string image, int generation)
  {
    // TODO: シェーダーに画像を渡す
  }

  /// <summary>
  /// 外部からの加速
  /// </summary>
  /// <param name="vel"></param>
  public void AddVelocity(Vector3 vel)
  {

  }

  /// <summary>
  /// 誕生
  /// </summary>
  public void Born()
  {

  }

  /// <summary>
  /// 死
  /// </summary>
  public void Die()
  {

  }

  /// <summary>
  /// 結婚
  /// </summary>
  /// <param name="with"></param>
  public void Marrige(Fish with)
  {

  }

  /// <summary>
  /// 喧嘩
  /// </summary>
  /// <param name="with"></param>
  public void Fight(Fish with)
  {
    // 低確率で死亡
  }

  /// <summary>
  /// 恋に落ちる
  /// </summary>
  /// <param name="with"></param>
  public void FallInLove(Fish with)
  {

  }

  /// <summary>
  /// 付き合う
  /// </summary>
  public void GoOut()
  {

  }

  /// <summary>
  /// 破局
  /// </summary>
  public void BreakUp()
  {

  }

  /// <summary>
  /// Netfrix見てChillする
  /// </summary>
  /// <param name="with"></param>
  public void NetfrixAndChill(Fish with)
  {
    // 結婚しているときにこのイベントが発生したら妊娠する可能性がある
  }

  /// <summary>
  /// 病気になる
  /// </summary>
  public void GetIll()
  {
    // 低確率で死亡
  }

}