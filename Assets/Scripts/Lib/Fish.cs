using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 一定progress到達したら死ぬ？（一生の間の心拍回数は決まっている？）
/// </summary>
public class Fish : MonoBehaviour
{

  public FishData data;  // 魚情報
  public Sex sex = Sex.Male;  // 性別
  private float _progress = 0f;  // 経過
  private Vector3 _velocity = Vector3.zero;
  private float bornTime;

  /// <summary>
  /// 位置
  /// </summary>
  /// <value></value>
  public Vector3 position
  {
    get { return this.transform.position; }
    set { this.transform.position = value; }
  }

  public float direction
  {
    get { return Mathf.Atan2(this._velocity.x, this._velocity.y); }
  }

  /// <summary>
  /// 年齢
  /// </summary>
  /// <value></value>
  public int age
  {
    get { return (int)Mathf.Round(Time.time - this.bornTime); }
  }

  /// <summary>
  /// 前回の位置
  /// 正確でない可能性
  /// </summary>
  /// <value></value>
  private Vector3 _lastPosition
  {
    get { return this.position - this._velocity; }
  }

  void Start() { }

  void Update()
  {
    this._progress += this._velocity.magnitude;
    Material material = this.GetComponent<SpriteRenderer>().material;
    material.SetFloat("_Progress", this._progress);

    // 位置更新
    Vector3 pos = this.position;
    pos += this._velocity;
    pos = MyStage.getPositionAddBias(pos, 2f, 2f);
    this.transform.position = pos;

    // 回転角
    Vector3 euler = this.transform.rotation.eulerAngles;
    euler.z = -this.direction * 180f / Mathf.PI;
    this.transform.rotation = Quaternion.Euler(euler);

    this._velocity *= 0.99f;
  }

  /// <summary>
  /// 周りを見わたして、周りの魚との位置関係から速度に変化をつける
  /// boidのアルゴリズム
  /// </summary>
  /// <param name="others"></param>
  public void LookAround(
    List<Fish> others,
    in float THRESHOLD_REFRECT,
    in float POWER_REFRECT,
    in float THRESHOLD_POS,
    in float POWER_POS,
    in float THRESHOLD_DIR,
    in float POWER_DIR
  )
  {
    Vector3 addVel = BoidAlgorithum.getVelociry(
      this,
      others,
      THRESHOLD_REFRECT,
      POWER_REFRECT,
      THRESHOLD_POS,
      POWER_POS,
      THRESHOLD_DIR,
      POWER_DIR
    );

    this._velocity += addVel;
  }

  /// <summary>
  /// 初期設定
  /// </summary>
  /// <param name="image"></param>
  public void Born(string image, int generation)
  {
    // TODO: シェーダーに画像を渡す
    byte[] bytes = System.Convert.FromBase64String(image);
    Texture2D texture = new Texture2D(1, 1);
    texture.LoadImage(bytes);
    Material material = this.GetComponent<SpriteRenderer>().material;
    material.SetTexture("_MyTex", texture);

    this.bornTime = Time.time;
  }

  /// <summary>
  /// 死
  /// </summary>
  public void Die()
  {
    // await 死ぬアニメーション
    Destroy(this.gameObject);
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
    // 低確率で死亡
  }

  /// <summary>
  /// 病気になる
  /// </summary>
  public void GetIll()
  {
    // 低確率で死亡
  }

}