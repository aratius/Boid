using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using DG.Tweening;

/// <summary>
/// 一定progress到達したら死ぬ？（一生の間の心拍回数は決まっている？）
/// </summary>
public class Fish : BoidModel
{

  public FishData data;  // 魚情報
  public UnityEvent<Fish> onDie = new UnityEvent<Fish>();  // 死亡イベント
  public Sex sex;  // 性別
  public float _size = 0f;  // 身長
  private Material _material;
  private float _progress = 0f;  // 経過
  private Vector3 _velocity = Vector3.zero;
  private float bornTime;
  private float _illnessToSave;
  private float _painToSave;
  private bool _isIllness = false;  // 病気
  private Tween _illnessTween;

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

  public override float size
  {
    get { return this._size; }
    set { this._size = value; }
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

  /// <summary>
  /// 病気度合い
  /// </summary>
  /// <value></value>
  private float _illness
  {
    set
    {
      if (this._illnessTween != null) this._illnessTween.Kill();
      this._illnessTween = DOTween.To(
        () => this._illnessToSave,
        (v) =>
        {
          this._illnessToSave = v;
          this._material.SetFloat("_IllProgress", this._illnessToSave);
          if (this._illnessToSave >= 1f) this.Die();
        },
        value,
        0.3f
      );
    }
    get { return this._illnessToSave; }
  }

  /// <summary>
  /// 身体的な傷
  /// </summary>
  /// <value></value>
  private float _pain
  {
    get { return this._painToSave; }
  }

  void Start()
  {
    this._material = this.GetComponent<SpriteRenderer>().material;
  }

  void Update()
  {
    this._progress += this._velocity.magnitude;
    this._material.SetFloat("_Progress", this._progress);

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

    float rand = Random.Range(0f, 1f);
    float illProbability = Time.deltaTime / 10;  // 10sに一回病気になるくらいの感覚
    if (rand < illProbability) this.GetIll();

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
    float confficient = (1f - this._illness);
    confficient = Mathf.Pow(confficient, 1.2f);

    this._velocity += BoidAlgorithum.getVelociry(
      this as BoidModel,
      others.ConvertAll<BoidModel>(new System.Converter<Fish, BoidModel>((Fish f) => f as BoidModel)),
      THRESHOLD_REFRECT,
      POWER_REFRECT,
      THRESHOLD_POS,
      POWER_POS,
      THRESHOLD_DIR,
      POWER_DIR
    ) * Time.deltaTime * 90f * confficient;
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
    this.size = Random.Range(0f, 1f) < 0.95f ? Random.Range(0.5f, 0.8f) : Random.Range(1.5f, 2f);
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
  /// 火葬する
  /// </summary>
  public void Cremate()
  {
    Destroy(this.gameObject);
    Destroy(this._material);
    Destroy(this._material.GetTexture("_MyTex"));
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
    // TODO: tweenでprogressを素早く動かす
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
  public async void GetIll()
  {
    if (this._isIllness) return;
    this._isIllness = true;

    float deathProbability = 0.3f;  // 死亡確率

    while (true)
    {
      int waitTime = Random.Range(300, 5000);
      await UniTask.Delay(waitTime);

      deathProbability += Random.Range(-0.4f, 0.4f);
      this._illness = Mathf.Max(Mathf.Min(deathProbability, 1f), 0f);

      if (deathProbability <= 0f || deathProbability >= 1f) break;  // 病気完治
    }

    this._isIllness = false;
  }

}