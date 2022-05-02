using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// 一定progress到達したら死ぬ？（一生の間の心拍回数は決まっている？）
/// </summary>
public class Fish : BoidModel
{

  public FishData data;  // 魚情報
  public UnityEvent<Fish> onDie = new UnityEvent<Fish>();  // 死亡イベント
  public Sex sex;  // 性別
  public float size = 0f;  // 身長
  private float _progress = 0f;  // 経過
  private Vector3 _velocity = Vector3.zero;
  private float bornTime;

  /// <summary>
  /// 位置
  /// </summary>
  /// <value></value>
  public override Vector3 position
  {
    get { return this.transform.position; }
    set { this.transform.position = value; }
  }

  public override float direction
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

    // サイズ 0-20歳で 0.6 - 1のlinear
    float[] range = new float[] { 0.6f, 1f };
    float scale = Mathf.Min((range[1] - range[0]) / 20f * this.age + range[0], 1f);
    this.transform.localScale = Vector3.one * scale * this.size;

    this._velocity *= 0.99f;
  }

  public void SetAge(int _age)
  {
    this.bornTime = Mathf.Round(Time.time - _age);
  }

  /// <summary>
  /// 周りを見わたして、周りの魚との位置関係から速度に変化をつける
  /// boidのアルゴリズム
  /// </summary>
  /// <param name="others"></param>
  public void LookAround(
    in List<Fish> others,
    in float THRESHOLD_REFRECT,
    in float POWER_REFRECT,
    in float THRESHOLD_POS,
    in float POWER_POS,
    in float THRESHOLD_DIR,
    in float POWER_DIR
  )
  {
    this._velocity += BoidAlgorithum.getVelociry(
      this as BoidModel,
      others.ConvertAll<BoidModel>(new System.Converter<Fish, BoidModel>((Fish f) => f as BoidModel)),
      THRESHOLD_REFRECT,
      POWER_REFRECT,
      THRESHOLD_POS,
      POWER_POS,
      THRESHOLD_DIR,
      POWER_DIR
    ) * Time.deltaTime * 90f;
  }

  /// <summary>
  /// 初期設定
  /// </summary>
  /// <param name="image"></param>
  public void Born(FishData d)
  {
    this.data = d;

    // 画像テクスチャ
    string image = Regex.Replace(this.data.image, "data:image/(png|jpe??g);base64,", "");
    byte[] bytes = System.Convert.FromBase64String(image);
    Texture2D texture = new Texture2D(1, 1);
    texture.LoadImage(bytes);
    Material material = this.GetComponent<SpriteRenderer>().material;
    material.SetTexture("_MyTex", texture);

    // 性別
    this.sex = Random.Range(0f, 1f) < 0.5f ? Sex.Male : Sex.Female;

    // 出生時間
    this.bornTime = Time.time;

    // 身長
    this.size = Random.Range(0.4f, 0.6f);
  }

  /// <summary>
  /// 死
  /// </summary>
  public void Die()
  {
    // TODO: await 死ぬアニメーション
    this.onDie.Invoke(this);
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