using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URG;
using UnityEditor;
using Config;

#if UNITY_EDITOR
[CustomEditor(typeof(UrgsHandler))]
public class UrgsHandlerEditor : Editor {
	public override void OnInspectorGUI(){
		base.OnInspectorGUI ();

		UrgsHandler urgsHandler = target as UrgsHandler;

		if (GUILayout.Button("AddUrg")) urgsHandler.AddUrg();
		if (GUILayout.Button("DeleteUrg")) urgsHandler.DeleteUrg();
		if (GUILayout.Button("BaseSave")) urgsHandler.BaseSave();
		if (GUILayout.Button("TopLeftSave")) urgsHandler.TopLeftSave();
		if (GUILayout.Button("TopRightSave")) urgsHandler.TopRightSave();
		if (GUILayout.Button("BottomLeftSave")) urgsHandler.BottomLeftSave();
		if (GUILayout.Button("BottomRightSave")) urgsHandler.BottomRightSave();
		if (GUILayout.Button("CalibrationClear")) urgsHandler.CalibrationClear();
		if (GUILayout.Button("SaveJson")) urgsHandler.SaveJson();
		if (GUILayout.Button("LoadJson")) urgsHandler.LoadJson();
	}
}
#endif
public class UrgsHandler : MonoBehaviour
{
	[SerializeField]
	GameObject urg; //複製用情報
	
	[SerializeField]
	TouchStartPosition touchStartPosition = TouchStartPosition.LEFT_BOTTOM; //タッチ位置の起点 [左上 or 左下]
	
	[SerializeField]
	Vector2 urgRect = new Vector2(30, 15);
	
	[SerializeField, Range(0, 0.01f)]
	float scale = 0.0045f; //URGの表示スケール
	
	[SerializeField, Range(1, 200)]
	int touchDistance = 100; // 認識の閾値　連続したもののみを取得するため (mm)

	[SerializeField, Range(0, 0.4f)]
	float touchThoreshold = 0.05f; // 別々のURGからみたポイントを同じポイントにするかのしきい値(0-1)


	// 最小タッチサイズ
	[SerializeField, Range(0, 500)]
	int minTouchSize = 0;

	[SerializeField, Range(1, 2000)]
	int maxTouchSize = 500; // 最大タッチサイズ

	List<UrgHandler> urgs = new List<UrgHandler>(); //URGセンサーリスト
	
	//送信用タッチ情報
	internal List<List<float>> sendPosition  = new List<List<float>>();
	
	void Start() {
		SetData();
		SetUrgs();
	}

	void Update() {
		List<List<Vector2>> positions = new List<List<Vector2>>();
		sendPosition.Clear();
		
		//すべてのURGからのポイントを近いものでリスト化する
		for (int i = 0; i < urgs.Count; i++) {
			UrgHandler urg = urgs[i];
			if (urg.pointGroups.Count == 0) continue;
			for (int j = 0; j < urg.pointGroups.Count; j++) {
				Vector2 pos = urg.pointGroups[j].rangeTouch.position;

				bool isAdd = false;
				//近い距離の点があれば格納
				foreach(List<Vector2> pList in positions) {
					foreach(Vector2 p in pList) {
						float d = Vector2.Distance(p, pos);
						if (touchThoreshold > d) {
							pList.Add(pos);
							isAdd = true;
							break;
						}
					}
					if (isAdd) break;
				}

				//新規追加
				if (!isAdd) {
					List<Vector2> list = new List<Vector2>();
					list.Add(new Vector2(pos.x, pos.y));
					positions.Add(list);
				}
			}
		}

		//グルーピングした点の中心を取る
		foreach(List<Vector2> pList in positions) {
			Vector2 pos = Vector2.zero;
			int cnt = 0;
			foreach(Vector2 p in pList) {
				pos += p;
				cnt++;
			}
			pos /= cnt;
			List<float> list = new List<float>() {pos.x, pos.y};
			sendPosition.Add(list);
		}
		
		// sendPositionが実際のタッチ情報として扱われる値
		// OSC送信にチェック入れている場合MainControllerの方で送信処理を実行されている
	}

	// インスペクターの変更を検知
	void OnValidate() {
		SetData();
	}

	//シーン上に存在しているURGをセット
	private void SetUrgs() {
		urgs.Clear();
		GameObject[] objects = GameObject.FindGameObjectsWithTag("URG");
		for (int i = 0; i < objects.Length; i++) {
			urgs.Add(objects[i].GetComponent<UrgHandler>());
		}
	}

	//センサーで使用する情報を設定に保存
	private void SetData() {
		Settings.urgRect = urgRect;
		Settings.scale = scale;
		Settings.touchDistance = touchDistance;
		Settings.touchThoreshold = touchThoreshold;
		Settings.maxTouchSize = maxTouchSize;
		Settings.minTouchSize = minTouchSize;
		Settings.touchStartPosition = touchStartPosition;
	}

	// 追加
	public void AddUrg() {
		int num = transform.childCount;
		GameObject g = Instantiate(urg);
		g.name = "URG_" + num.ToString();
		g.transform.parent = transform;
		SetUrgs();
	}

	// 削除
	public void DeleteUrg() {
		GameObject[] objects = GameObject.FindGameObjectsWithTag("URG");
		if (objects.Length == 0) return;
		#if UNITY_EDITOR
		if (EditorApplication.isPlaying) {
			Destroy(objects[objects.Length-1]);
		} else {
			DestroyImmediate(objects[objects.Length-1]);
		}
		#else
			Destroy(objects[objects.Length-1]);
		#endif
		SetUrgs();
	}

	//初期値保存
	public void BaseSave () {
		Debug.Log("---- BaseSave ----");
		SetUrgs();
		for (int i = 0; i < urgs.Count; i ++) {
			urgs[i].SavePoint(UrgSavePosition.BASE, true);
		}
	}

	//左上保存
	public void TopLeftSave () {
		Debug.Log("---- TopLeftSave ----");
		SetUrgs();
		for (int i = 0; i < urgs.Count; i ++) {
			urgs[i].SavePoint(UrgSavePosition.TOP_LEFT, false);
		}
	}

	//右上保存
	public void TopRightSave () {
		Debug.Log("---- TopRightSave ----");
		SetUrgs();
		for (int i = 0; i < urgs.Count; i ++) {
			urgs[i].SavePoint(UrgSavePosition.TOP_RIGHT, false);
		}
	}

	//左下保存
	public void BottomLeftSave () {
		Debug.Log("---- BottomLeftSave ----");
		SetUrgs();
		for (int i = 0; i < urgs.Count; i ++) {
			urgs[i].SavePoint(UrgSavePosition.BOTTOM_LEFT, false);
		}
	}

	//右下保存
	public void BottomRightSave () {
		Debug.Log("---- BottomRightSave ----");
		SetUrgs();
		for (int i = 0; i < urgs.Count; i ++) {
			urgs[i].SavePoint(UrgSavePosition.BOTTOM_RIGHT, false);
		}
	}

	//クリア
	public void CalibrationClear () {
		Debug.Log("---- CalibrationClear ----");
		SetUrgs();
		for (int i = 0; i < urgs.Count; i ++) {
			urgs[i].CalibrationClear();
		}
	}

	// json保存
	public void SaveJson() {
		SetUrgs();

		JsonData data = new JsonData();
		data.scale = scale;
		data.touchDistance = touchDistance;
		data.touchThoreshold = touchThoreshold;
		data.minTouchSize = minTouchSize;
		data.maxTouchSize = maxTouchSize;
		data.touchStartPosition = touchStartPosition;
		data.urgData = new JsonUrgData[urgs.Count];

		for (int i = 0; i < urgs.Count; i++) {
			data.urgData[i] = urgs[i].GetSaveData();
		}
		Json.Save(data);
		Debug.Log("INFO: JSONを保存しました。");
	}

	// json読み込む
	public void LoadJson() {
		SetUrgs();
		JsonData data = Json.Load();

		touchDistance = data.touchDistance;
		touchThoreshold = data.touchThoreshold;
		minTouchSize = data.minTouchSize;
		maxTouchSize = data.maxTouchSize;
		touchStartPosition = data.touchStartPosition;
		SetData();

		//読み込み情報よりURGが多い場合は削除
		// Debug.Log(data.urgData.Length + "," + urgs.Count);
		for (int i = urgs.Count; i > data.urgData.Length; i--) {
			DeleteUrg();
		}

		//読み込み情報よりURGが少ない場合は追加
		for (int i = urgs.Count; i < data.urgData.Length; i++) {
			AddUrg();
		}

		for (int i = 0; i < data.urgData.Length; i++) {
			urgs[i].SetSaveData(data.urgData[i]);
		}



		// for (int i = 0; i < urgs.Count; i++) {
		// 	data.urgData[i] = urgs[i].GetSaveData();
		// }

		Debug.Log("INFO: JSONを読み込みました。");

	}
}
