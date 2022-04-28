using UnityEngine;

/// <summary>
/// 一定progress到達したら死ぬ？（一生の間の心拍回数は決まっている？）
/// </summary>
public class Fish : MonoBehaviour
{

  public Sex sex = Sex.Male;  // 性別
  public int generation = -1;  // 世代
  private float _progress = 0f;  // 経過
  private Vector3 _velocity = Vector3.zero;
  private Vector3 _lastPosition = Vector3.zero;

  /// <summary>
  /// 位置
  /// </summary>
  /// <value></value>
  public Vector3 position
  {
    get { return this.transform.position; }
    set { this.transform.position = value; }
  }

  void Start()
  {

  }

  void Update()
  {
    const float COEFFICIENT = 0.1f;  // 係数
    this._progress += this._velocity.magnitude * COEFFICIENT;

    Vector3 pos = this.position;
    pos += this._velocity;
    this.transform.position = pos;

    Vector3 euler = this.transform.rotation.eulerAngles;
    euler.z = Mathf.Atan2(this._velocity.x, this._velocity.y) * 180f / Mathf.PI;
    this.transform.rotation = Quaternion.Euler(euler);

    this._velocity *= 0.9f;

    this._lastPosition = this.position;
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
    this._velocity += vel;
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
  /// 妊娠する
  /// NOTE: メスクラスに継承してそちらで実装したほうが良いかも
  /// </summary>
  public void GetPregnant()
  {

  }

  /// <summary>
  /// 出産する
  /// NOTE: メスクラスに継承してそちらで実装したほうが良いかも
  /// </summary>
  public void GiveBirth()
  {

  }

  /// <summary>
  /// 病気になる
  /// </summary>
  public void GetIll()
  {
    // 低確率で死亡
  }

}