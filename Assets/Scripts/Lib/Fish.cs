﻿using UnityEngine;

/// <summary>
/// 一定progress到達したら死ぬ？（一生の間の心拍回数は決まっている？）
/// </summary>
public class Fish : MonoBehaviour
{

  public FishData data;  // 魚情報
  public Sex sex = Sex.Male;  // 性別
  public int age = 0;  // 年齢
  private float _progress = 0f;  // 経過
  private Vector3 _velocity = Vector3.zero;

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
  /// 前回の位置
  /// 正確でない可能性
  /// </summary>
  /// <value></value>
  private Vector3 _lastPosition
  {
    get { return this.position - this._velocity; }
  }

  void Start()
  {

  }

  void Update()
  {
    const float COEFFICIENT = 0.1f;  // 係数
    this._progress += this._velocity.magnitude * COEFFICIENT;
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
  /// 初期設定
  /// </summary>
  /// <param name="image"></param>
  public void Init(string image, int generation)
  {
    // TODO: シェーダーに画像を渡す
    byte[] bytes = System.Convert.FromBase64String(image);
    Texture2D texture = new Texture2D(1, 1);
    texture.LoadImage(bytes);
    Material material = this.GetComponent<SpriteRenderer>().material;
    material.SetTexture("_MyTex", texture);
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