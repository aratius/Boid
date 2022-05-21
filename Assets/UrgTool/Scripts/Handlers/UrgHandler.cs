using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using URG;
using Config;

public class UrgHandler : MonoBehaviour
{
  UrgDeviceEthernet urg;

  [SerializeField]
  string ip_address = "192.168.1.10";

  [SerializeField]
  int port_number = 10940;

  [SerializeField]
  private UrgStartPosition urgStartPosition = UrgStartPosition.TOP;

  Vector3[] directions;

  bool cached = false;

  // 距離
  List<long> distances = new List<long>();

  [SerializeField]
  Color distanceColor = Color.white;

  //変更前URG位置
  UrgStartPosition lastedUrgStartPosition = UrgStartPosition.NONE;

  // URG位置
  Vector3 startPoint = Vector3.zero;

  //強度
  List<long> strengths = new List<long>();

  Color strengthColor = Color.blue;

  [SerializeField]
  Vector2 topLeft; //キャリブレーション 左上

  [SerializeField]
  Vector2 topRight; //キャリブレーション 右上

  [SerializeField]
  Vector2 bottomLeft; //キャリブレーション 左下

  [SerializeField]
  Vector2 bottomRight; //キャリブレーション 右下

  [SerializeField]
  bool isDebug = true; //デバッグ表示確認
  UrgDebug urgDebug;

  [System.NonSerialized]
  public bool isStop = false;
  List<UrgPoint> points = new List<UrgPoint>();
  public List<UrgPointGroup> pointGroups = new List<UrgPointGroup>();
  List<UrgTouch> touches = new List<UrgTouch>(); // mm単位のtouches
  List<UrgTouch> pxTouches = new List<UrgTouch>(); // px単位のtouches
  List<UrgTouch> lastPxTouches = new List<UrgTouch>(); // 直近のpx単位のtouches

  bool isSave = false; //平均データ保存用フラグ

  const int saveNum = 10; //保存する回数
  int saveCnt = 0; //保存カウント用

  UrgSavePosition savePosition; //保存する位置

  UrgPoints basePoints = new UrgPoints(); //キャリブレーション前ポイント情報
  UrgPoints compairPoints = new UrgPoints(); //比較用ポイント情報


  Point2f[] srcPt = new Point2f[4];
  Point2f[] dstPt = new Point2f[4];

  void Start()
  {
    urgDebug = GetComponent<UrgDebug>();
    urg = this.gameObject.AddComponent<UrgDeviceEthernet>();

    // 接続
    urg.StartTCP(ip_address, port_number);

    if (urg.isConnected)
    {
      //MD: (計測＆送信要求)
      urg.Write(SCIP_library.SCIP_Writer.MD(0, 1080, 1, 0, 0));

      //ME: (計測＆距離データ・受光強度値送信要求)
      //urg.Write(SCIP_library.SCIP_Writer.ME(0, 1080, 1, 1, 0));
    }
  }


  void Update()
  {
    // 未接続の場合は処理なし
    if (!urg.isConnected) return;

    //スペースで検出一時停止
    if (Input.GetKeyDown(KeyCode.Space))
    {
      isStop = !isStop;
      Debug.Log("STOP");
    }

    if (!isStop)
    {
      // 強度取得
      try
      {
        if (urg.strengths.Count > 0)
        {
          strengths.Clear();
          strengths.AddRange(urg.strengths);
        }
      }
      catch
      {
      }

      // 距離取得
      try
      {
        if (urg.distances.Count > 0)
        {
          distances.Clear();
          distances.AddRange(urg.distances);
        }
      }
      catch
      {
      }
    }

    //起点設定
    if (urgStartPosition != lastedUrgStartPosition)
    {
      cached = false; //キャッシュクリア
      lastedUrgStartPosition = urgStartPosition;
      startPoint = UrgUtil.GetStartPoint(urgStartPosition);
    }

    //角度設定
    //角度の計算キャッシュがないときのみ
    if (urg.distances.Count > 0 && !cached)
    {
      directions = new Vector3[urg.distances.Count];
      directions = UrgUtil.GetDirections(urgStartPosition, directions);
      cached = true;
    }

    // strengths
    for (int i = 0; i < strengths.Count; i++)
    {
      Vector3 dir = new Vector3(directions[i].x, directions[i].y, 0);
      long dist = strengths[i];
      DrawRay(startPoint, Mathf.Abs(dist) * dir * Settings.scale, strengthColor);
    }

    // distances
    for (int i = 0; i < distances.Count; i++)
    {
      Vector3 dir = new Vector3(directions[i].x, directions[i].y, 0);
      long dist = distances[i];
      DrawRay(startPoint, dist * dir * Settings.scale, distanceColor);
    }

    points.Clear();
    pointGroups.Clear();
    touches.Clear();
    pxTouches.Clear();
    float radian, d, x, y;

    for (int i = 0; i < distances.Count; i++)
    {
      d = distances[i]; //距離
      x = d * directions[i].x;
      y = d * directions[i].y;
      radian = directions[i].z;

      UrgPoint point = new UrgPoint();
      point.distance = d;
      point.radian = radian;
      point.position.x = x;
      point.position.y = y;
      points.Add(point);

      // 連続している座標をグループに分ける
      int pointGroupIndex = pointGroups.Count - 1;
      if (i > 0 && Vector2.Distance(point.position, points[i - 1].position) < Settings.touchDistance)
      {
        // 距離が近いから最後に作成されたグループに点を追加
        pointGroups[pointGroupIndex].Add(point);
      }
      else
      {
        // 距離が離れてるから新しいグループを作成して、はじめの点を追加（[0]）
        pointGroups.Add(new UrgPointGroup());
        pointGroupIndex = pointGroups.Count - 1;
        pointGroups[pointGroupIndex].Add(new UrgPoint());
        pointGroups[pointGroupIndex][0].distance = d;
        pointGroups[pointGroupIndex][0].radian = radian;
        pointGroups[pointGroupIndex][0].position.x = x;
        pointGroups[pointGroupIndex][0].position.y = y;
      }
    }

    //各位置保存押された時「saveNum」回位置情報を保存する
    if (isSave)
    {
      List<UrgPoint> p = new List<UrgPoint>(points); //deep copy
      if (savePosition != UrgSavePosition.BASE) compairPoints.Add(p);
      else basePoints.Add(p);
      saveCnt++;

      //保存上限に達したとき
      if (saveNum == saveCnt)
      {
        // リセット処理
        saveCnt = 0;
        isSave = false;
        //ベース以外のときはベースと比較して位置を設定する
        if (savePosition != UrgSavePosition.BASE)
        {
          ComparerWithBase();
        }
      }
    }

    //キャリブレーション確認用
    bool isCompleted = true;
    if (topLeft != Vector2.zero)
    {
      Vector3 pos = new Vector3(topLeft.x, topLeft.y, 0);
      pos = pos * Settings.scale;
      DrawRay(startPoint, pos, Color.red);
    }
    else
    {
      isCompleted = false;
    }

    if (topRight != Vector2.zero)
    {
      Vector3 pos = new Vector3(topRight.x, topRight.y, 0);
      pos = pos * Settings.scale;
      DrawRay(startPoint, pos, Color.red);
    }
    else
    {
      isCompleted = false;
    }

    if (bottomLeft != Vector2.zero)
    {
      Vector3 pos = new Vector3(bottomLeft.x, bottomLeft.y, 0);
      pos = pos * Settings.scale;
      DrawRay(startPoint, pos, Color.red);
    }
    else
    {
      isCompleted = false;
    }

    if (bottomRight != Vector2.zero)
    {
      Vector3 pos = new Vector3(bottomRight.x, bottomRight.y, 0);
      pos = pos * Settings.scale;
      DrawRay(startPoint, pos, Color.red);
    }
    else
    {
      isCompleted = false;
    }

    //左下起点
    if (Settings.touchStartPosition == TouchStartPosition.LEFT_BOTTOM)
    {
      srcPt[0] = new Point2f(bottomLeft.x, bottomLeft.y);   // [0, 0]
      srcPt[1] = new Point2f(bottomRight.x, bottomRight.y); // [1, 0]
      srcPt[2] = new Point2f(topRight.x, topRight.y);       // [1, 1]
      srcPt[3] = new Point2f(topLeft.x, topLeft.y);         // [0, 1]
    }
    else
    { //左上起点
      srcPt[0] = new Point2f(topLeft.x, topLeft.y);         // [0, 0]
      srcPt[1] = new Point2f(topRight.x, topRight.y);       // [1, 0]
      srcPt[2] = new Point2f(bottomRight.x, bottomRight.y); // [1, 1]
      srcPt[3] = new Point2f(bottomLeft.x, bottomLeft.y);   // [0, 1]
    }

    dstPt[0] = new Point2f(0, 0);
    dstPt[1] = new Point2f(1, 0);
    dstPt[2] = new Point2f(1, 1);
    dstPt[3] = new Point2f(0, 1);
    Mat perspective = Cv2.GetPerspectiveTransform(srcPt, dstPt);

    // 座標のグループごとに中心点とサイズを計算して、タッチポイントとする
    for (int i = 0; i < pointGroups.Count; i++)
    {
      UrgTouch touch = new UrgTouch(pointGroups[i]);

      // 指定されたサイズにおさまるタッチのみ
      if (touch.size >= Settings.minTouchSize && touch.size <= Settings.maxTouchSize)
      {
        Vector3 pos = new Vector3(touch.position.x, touch.position.y, 0);
        // pos = pos * Settings.scale;
        // Debug.DrawRay(startPoint, pos, Color.blue);
        // pos += startPoint;

        //キャリブレーションが完了してなければスキップ
        if (!isCompleted) continue;

        // 0 - 1に射影変換
        Mat srcPoint1 = new Mat(1, 1, MatType.CV_32FC2, new Scalar(touch.position.x, touch.position.y));
        Mat dstPoint1 = new Mat(1, 1, MatType.CV_32FC2);
        Cv2.PerspectiveTransform(srcPoint1, dstPoint1, perspective);
        Vec2f v = dstPoint1.At<Vec2f>(0);

        Mat srcPoint2 = new Mat(1, 1, MatType.CV_32FC2, new Scalar(touch.position.x + touch.size, touch.position.y + touch.size));
        Mat dstPoint2 = new Mat(1, 1, MatType.CV_32FC2);
        Cv2.PerspectiveTransform(srcPoint2, dstPoint2, perspective);
        Vec2f s = dstPoint2.At<Vec2f>(0);

        float size = ((s[0] - v[0]) + (s[1] - v[1])) / 2.0f;
        // 範囲内のタッチのみに限定する
        if (v[0] >= 0 && v[0] <= 1.0f && v[1] >= 0 && v[1] <= 1.0f)
        {
          // Debug.Log("---------- v[0] = " + v[0] + ", v[1] = " + v[1]);
          pointGroups[i].isActive = true;
          pointGroups[i].rangeTouch = new UrgTouch(v[0], v[1], size);
        }
      }
    }

    //実際の足を検知
    if (isDebug)
    {
      urgDebug.Debug(isStop, pointGroups);
    }

    for (int i = 0; i < pointGroups.Count; i++)
    {
      if (!pointGroups[i].isActive)
      {
        pointGroups.Remove(pointGroups[i]);
        i--;
      }
    }
  }

  ///<summary>
  /// basePositionと保存したい位置情報を比較して位置を決める
  ///</summary>
  void ComparerWithBase()
  {
    bool isError = false;
    switch (savePosition)
    {
      case UrgSavePosition.TOP_LEFT:
        topLeft = UrgPoints.GetDiffPoiont(basePoints, compairPoints);
        isError = (topLeft == Vector2.zero) ? true : false;
        break;
      case UrgSavePosition.TOP_RIGHT:
        topRight = UrgPoints.GetDiffPoiont(basePoints, compairPoints);
        isError = (topRight == Vector2.zero) ? true : false;
        break;
      case UrgSavePosition.BOTTOM_LEFT:
        bottomLeft = UrgPoints.GetDiffPoiont(basePoints, compairPoints);
        isError = (bottomLeft == Vector2.zero) ? true : false;
        break;
      case UrgSavePosition.BOTTOM_RIGHT:
        bottomRight = UrgPoints.GetDiffPoiont(basePoints, compairPoints);
        isError = (bottomRight == Vector2.zero) ? true : false;
        break;
    }

    // エラー削除
    GameObject[] game = GameObject.FindGameObjectsWithTag("ErrorDebug");
    foreach (GameObject obj in game)
    {
      Destroy(obj);

      //複数ポイントが検出された場合はエラー
      //エラー箇所にcube表示させる
      if (isError)
      {
        //GameObject.Find();
        for (int i = 0; i < UrgPoints.errorPoints.Count; i++)
        {
          Vector2 errorPoint = UrgPoints.errorPoints[i];
          Vector3 pos = new Vector3(errorPoint.x, errorPoint.y, 0);
          pos = pos * Settings.scale + startPoint;
          GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
          cube.tag = "ErrorDebug";
          cube.transform.position = pos;
        }
      }
    }
  }

  ///<summary>
  /// レイ描画
  ///</summary>
  void DrawRay(Vector3 start, Vector3 dir, Color color)
  {
    if (!isDebug) return;
    Debug.DrawRay(start, dir, color);
  }

  ///<summary>
  /// 位置情報保存
  ///</summary>
  public void SavePoint(UrgSavePosition savePosition, bool isBase)
  {
    isSave = true;
    this.savePosition = savePosition;
    if (isBase) basePoints.Clear();
    else compairPoints.Clear();
  }

  ///<summary>
  /// キャリブレーション情報クリア
  ///</summary>
  public void CalibrationClear()
  {
    topLeft = Vector2.zero;
    topRight = Vector2.zero;
    bottomLeft = Vector2.zero;
    bottomRight = Vector2.zero;
    compairPoints.Clear();
    basePoints.Clear();
  }

  ///<summary>
  /// json保存するデータを取得
  ///</summary>
  public JsonUrgData GetSaveData()
  {
    urgDebug = GetComponent<UrgDebug>();
    JsonUrgData data = new JsonUrgData();
    data.ip_address = ip_address;
    data.port_number = port_number;
    data.urgStartPosition = urgStartPosition;
    data.topLeft = topLeft;
    data.topRight = topRight;
    data.bottomLeft = bottomLeft;
    data.bottomRight = bottomRight;
    data.distanceColor = distanceColor;
    data.isDebug = isDebug;
    data.color = urgDebug.color;
    return data;
  }

  public void SetSaveData(JsonUrgData data)
  {
    urgDebug = GetComponent<UrgDebug>();

    ip_address = data.ip_address;
    port_number = data.port_number;
    urgStartPosition = data.urgStartPosition;
    topLeft = data.topLeft;
    topRight = data.topRight;
    bottomLeft = data.bottomLeft;
    bottomRight = data.bottomRight;
    distanceColor = data.distanceColor;
    isDebug = data.isDebug;
    urgDebug.color = data.color;
  }
}
