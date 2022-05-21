using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;

namespace URG {
	public class UrgPoint {
		public float radian;
		public float distance;
		public Vector2 position = new Vector2();
		public bool touched;
		
		public void Add(UrgPoint point) {
			distance += point.distance;
			position += point.position;
		}

		public void Clone(UrgPoint point) {
			radian = point.radian;
			distance = point.distance;
			position = point.position;
			touched = point.touched;
		}
	}

	public class UrgPoints : List<List<UrgPoint>> {
		public static List<Vector2> errorPoints = new List<Vector2>();
		static List<UrgPointGroup> groups = new List<UrgPointGroup>();
		
		public static Vector2 GetDiffPoiont(UrgPoints basePoints, UrgPoints compairPoints) {
			float touchDistance = Settings.touchDistance;
			float minTouchSize = Settings.minTouchSize;
			float maxTouchSize = Settings.maxTouchSize;
			
			List<UrgPoint> avgBasePoints = new List<UrgPoint>();
			List<UrgPoint> avgCompairPoints = new List<UrgPoint>();

			List<List<UrgPoint>> basePointsList = new List<List<UrgPoint>>();
			List<List<UrgPoint>> compairPointsList = new List<List<UrgPoint>>();
			List<UrgPoint> medianBasePoints = new List<UrgPoint>();
			List<UrgPoint> medianCompairPoints = new List<UrgPoint>();

			int pointCnt = basePoints[0].Count; //ポイントの数
			int loopCnt = basePoints.Count;     //保存した回数
			
			errorPoints.Clear();
			groups.Clear();
			
			// 配列整理 [0~10:保存回数][0~1xxx:ポイント数] → [0~1xxx:ポイント数][0~10:保存回数]
			for (int i = 0; i < pointCnt; i++) {
				List<UrgPoint> baseList = new List<UrgPoint>();
				List<UrgPoint> compairList = new List<UrgPoint>();
				for (int j = 0; j < loopCnt; j++) {
					try {
						baseList.Add(basePoints[j][i]);
						compairList.Add(compairPoints[j][i]);
					} catch {
						Debug.Log("error");
						Debug.Log(basePoints.Count + ", " + basePoints[j].Count);
						Debug.Log(compairPoints.Count + ", " + compairPoints[j].Count);
					}
				}
				
				
				basePointsList.Add(baseList);
				compairPointsList.Add(compairList);
			}

			//ソートして中央値取得
			for (int i = 0; i < basePointsList.Count; i++) {
				List<UrgPoint> baseList = basePointsList[i];
				List<UrgPoint> compairList = compairPointsList[i];
				baseList.Sort((a, b) => (int)(a.distance - b.distance));
				compairList.Sort((a, b) => (int)(a.distance - b.distance));

				// 中央値を取得
				UrgPoint bp = new UrgPoint();
				UrgPoint cp = new UrgPoint();
				int half = (int)Mathf.Round((float)baseList.Count / 2.0f);
				if (half % 2 == 1) {
					bp = baseList[half];
					cp = compairList[half];
				} else {
					bp.Clone(baseList[half]);
					bp.distance = (bp.distance + baseList[half - 1].distance) / 2;
					bp.position = (bp.position + baseList[half - 1].position) / 2;
				}

				medianBasePoints.Add(bp);
				medianCompairPoints.Add(cp);
			}

			
			//グループ化
			for (int i = 0; i < medianBasePoints.Count; i++) {
				// 何もおいてないときと差分が小さければ無視
				if (Vector2.Distance(medianBasePoints[i].position, medianCompairPoints[i].position) < 100) {
					continue;
				}
				
				// グループが一つもなければ作る
				if ( groups.Count < 1 ) {
					groups.Add(new UrgPointGroup());
					groups[0].Add( new UrgPoint());
					groups[0][0].distance = medianCompairPoints[i].distance;
					groups[0][0].radian = medianCompairPoints[i].radian;
					groups[0][0].position.x = medianCompairPoints[i].position.x;
					groups[0][0].position.y = medianCompairPoints[i].position.y;
				} else {
					UrgPointGroup lastGroup = groups[groups.Count-1];
					UrgPoint lastPoint = lastGroup[lastGroup.Count-1];
					// 同じグループとみなすべき距離に収まらない場合は別のグループを作成する
					if ( Vector2.Distance(medianCompairPoints[i].position, lastPoint.position) > touchDistance) {
						lastGroup =  new UrgPointGroup();
						groups.Add(lastGroup);
					}
					lastGroup.Add(medianCompairPoints[i]);
				}
			}

			//ノイズ削除
			for (int i=0; i<groups.Count; i++) {
				if (groups[i].Count < 5) {
					groups.Remove(groups[i]);
					i--;
				} else {
					Debug.Log("groups[i].Count = " + groups[i].Count);
				}
			}

			// 複数存在時エラー
			if (groups.Count > 1) {
				Debug.Log("INFO: 複数ポイントがあります。");
				
				//エラー場所にキュープ表示
				for( int i = 0; i < groups.Count; i++ ) {
					UrgTouch touch = new UrgTouch(groups[i]);
					// 指定されたサイズにおさまるタッチのみ
					if ( touch.size >= minTouchSize && touch.size <= maxTouchSize ) {
						UrgPoint point = groups[i][0];
						float minDistance = point.distance;
						for ( int j=0; j<groups[i].Count; j++ ) {
							point = groups[i][j];
							if ( point.distance < minDistance ) {
								Vector2 p = new Vector2(point.position.x, point.position.y);
								errorPoints.Add(p);
								minDistance = point.distance;
							}
						}
					}
				}
				return new Vector2();
			} else if (groups.Count == 0) {
				Debug.Log("INFO: 変更ポイントがありません。");
				return new Vector2();
			}


			// 座標のグループごとに中心点とサイズを計算して、タッチポイントとする
			Vector2 touchPoint = new Vector2();
			for( int i = 0; i < groups.Count; i++ ) {
				UrgTouch touch = new UrgTouch(groups[i]);
				
				// 指定されたサイズにおさまるタッチのみ
				if ( touch.size >= minTouchSize && touch.size <= maxTouchSize ) {
					touchPoint = touch.position;
				}
			}
			
			return touchPoint;
		}
	}

	public class UrgTouch {
		public float size = 0;
		public bool isNew = true;
		public int releaseCount = 0;

		public Vector2 position = new Vector2();
		
		public UrgTouch() {}
		
		public UrgTouch(float x, float y, float size=1.0f) {
			position.x = x;
			position.y = y;
			this.size = size;
		}

		public UrgTouch(List<UrgPoint> points) {
			//ポイントグループの中でURGから一番近い点を取得
			UrgPoint point = points[0];
			float minDistance = point.distance;
			for ( int i=0; i<points.Count; i++ ) {
				point = points[i];
				if ( point.distance < minDistance ) {
					minDistance = point.distance;
					position.x = point.position.x;
					position.y = point.position.y;
				}
			}
			
			//一番離れている箇所をサイズと設定
			size = 0;
			Vector2 center = new Vector2(position.x, position.y);
			float distance;
			for ( int i=0; i<points.Count; i++ ) {
				point = points[i];
				distance = Vector2.Distance(center, point.position);
				if ( distance > size ) size = distance;
			}
		}
	}

	public class UrgPointGroup : List<UrgPoint> {
		public bool isActive = false;
		public UrgTouch touch;
		public UrgTouch rangeTouch;
	}

}