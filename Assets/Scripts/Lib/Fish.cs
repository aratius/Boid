using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using DG.Tweening;

/// <summary>
/// 一定progress到達したら死ぬ？（一生の間の心拍回数は決まっている？）
/// </summary>
public class Fish : MonoBehaviour
{

  /// <summary>
  /// public
  /// </summary>
  public FishData data;  // 魚情報
  public UnityEvent<Fish> onDie = new UnityEvent<Fish>();  // 死亡イベント
  public UnityEvent<Fish> onGetPregnant = new UnityEvent<Fish>();  // 死亡イベント
  public Sex sex;  // 性別
  public Fish partner;

  /// <summary>
  /// private
  /// </summary>
  private Material _material;
  private float _size = 0f;  // 身長
  private float _progress = 0f;  // 経過
  private Vector3 _velocity = Vector3.zero;
  private float _bornTime;
  private float _illnessToSave;
  private float _painToSave;
  private bool _isIllness = false;  // 病気
  private Tween _illnessTween;
  private bool _isDead = false;

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

  public float size
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
    get { return (int)Mathf.Round(Time.time - this._bornTime); }
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

    // 病気になる
    float illProbability = Time.deltaTime / 10;  // 10sに一回くらいの感覚
    if (Percent.Get(illProbability)) this.GetIll();

    // NetfrixAndChillのトリガー
    if (this.partner != null)
    {
      float distToPartner = Vector3.Distance(this.position, this.partner.position);
      if (distToPartner < 3f)
      {
        float chillProbability = Time.deltaTime / 2;  // 2sに一回くらいの感覚
        if (Percent.Get(chillProbability))
        {
          this.NetfrixAndChillWith(this.partner);
          this.partner.NetfrixAndChillWith(this);
        }
      }
    }

    this._velocity *= 0.99f;
  }

  /// <summary>
  /// 年齢をセット
  /// </summary>
  /// <param name="_age"></param>
  public void SetAge(int _age)
  {
    this._bornTime = Mathf.Round(Time.time - _age);
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
      this,
      others,
      THRESHOLD_REFRECT,
      POWER_REFRECT,
      THRESHOLD_POS,
      POWER_POS,
      THRESHOLD_DIR,
      POWER_DIR
    // ) * Time.deltaTime * 90f * confficient;
    ) * confficient;

    // 結婚するかも
    MarriageAlgorithum.getPartner(this, others);
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
    this.sex = Percent.Get(0.5f) ? Sex.Male : Sex.Female;

    // 出生時間
    this._bornTime = Time.time;

    // 身長
    // this.size = Random.Range(0f, 1f) < 0.95f ? Random.Range(0.5f, 0.8f) : Random.Range(1.5f, 2f);
    this.size = Random.Range(0.5f, 0.8f);
  }

  /// <summary>
  /// 死
  /// </summary>
  public void Die()
  {
    // TODO: await 死ぬアニメーション
    this.onDie.Invoke(this);
    this._isDead = true;
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
  /// <param name="partner"></param>
  public void MarrigeWith(Fish partner)
  {
    // Debug.Log($"Marriage with {partner.data.id}, {partner.age}");
    this.partner = partner;
    this.partner.onDie.AddListener(this.OnDiePartner);
  }

  /// <summary>
  /// 離婚
  /// </summary>
  public void Divorce()
  {

  }

  /// <summary>
  /// 恋に落ちる
  /// </summary>
  /// <param name="angel"></param>
  public void FallInLoveWith(Fish angel)
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
  /// <param name="friend"></param>
  public async void NetfrixAndChillWith(Fish friend)
  {
    // 結婚しているときにこのイベントが発生したら妊娠する可能性がある
    // TODO: tweenでprogressを素早く動かす
    // Debug.Log($"Chill with {partner.data.id}, {partner.age}");

    int cnt = 0;
    bool finished = false;
    while (!finished && this.partner != null && !this._isDead)
    {
      this._progress += 0.005f;
      cnt++;

      // 惹かれ合う力
      float dist = Vector3.Distance(this.position, this.partner.position);
      if (dist < 3f)
      {
        Vector3 magnetPower = this.partner.position - this.position;
        this._velocity += Vector3.Normalize(magnetPower) * dist * 0.001f;
      }

      if (cnt > 180) finished = true;
      await UniTask.WaitForFixedUpdate();
    }

    // 妊娠する
    float getPregnantProbability = 1f / 10f;
    if (this.sex == Sex.Female && Percent.Get(getPregnantProbability)) this.GetPregnant();
  }

  /// <summary>
  /// 妊娠する
  /// NOTE: メスクラスに継承してそちらで実装したほうが良いかも
  /// </summary>
  public void GetPregnant()
  {
    this.onGetPregnant.Invoke(this);
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
  /// 喧嘩
  /// </summary>
  /// <param name="enemy"></param>
  public void FightWith(Fish enemy)
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

  /// <summary>
  /// パートナーと死別
  /// </summary>
  /// <param name="partner"></param>
  private void OnDiePartner(Fish partner)
  {
    this.partner = null;

    // ショックでたまに病気になる
    float illProbability = 3f / 10f;
    if (Percent.Get(illProbability)) this.GetIll();
  }

}