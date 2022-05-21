using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URG;

namespace URG {
	public class UrgDebug : MonoBehaviour {
		[SerializeField]
		GameObject DebugObj; //デバッグ用オブジェ
		
		[SerializeField]
		public Color color = Color.green; //デバッグカラー
		
		List<GameObject> cubeObjects = new List<GameObject>();
		
		
		public void Debug(bool isStop, List<UrgPointGroup> pointGroups) {
			if (isStop) return;
			// 削除
			for (int i = 0; i < cubeObjects.Count; i++) {
				Destroy(cubeObjects[i]);
			}
			
			//クリア
			cubeObjects.Clear();
			cubeObjects = new List<GameObject>();
			
			// タッチがあればその場にスフィアを生成
			for (int i = 0; i < pointGroups.Count; i++) {
				UrgPointGroup group = pointGroups[i];
				
				if (!group.isActive) continue;
				// Debug.Log(group.rangeTouch.position.x + ", " +  group.rangeTouch.position.y);
				
				Vector3 px = new Vector3(Screen.width * group.rangeTouch.position.x, Screen.height * group.rangeTouch.position.y, 10.0f);
				// Debug.Log(px.x + ", " + px.y);

				Camera camera = Camera.main;

				Vector3 pos = camera.ScreenToWorldPoint(px);
				// Debug.Log(pos.x + ", " + pos.y + ", " + pos.z);
				
				GameObject cube = Instantiate(DebugObj);
				cube.GetComponent<Renderer>().material.color = color;
				cube.transform.position = pos;
				cubeObjects.Add(cube);
			}
		}
	}
}