using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;

namespace URG {
	public enum UrgSavePosition {
		BASE,
		TOP_LEFT,
		TOP_RIGHT,
		BOTTOM_LEFT,
		BOTTOM_RIGHT,
	}
	
	public enum UrgStartPosition {
		TOP,
		RIGHT,
		BOTTOM,
		LEFT,
		TOP_RIGHT,
		TOP_LEFT,
		BOTTOM_RIGHT,
		BOTTOM_LEFT,
		NONE
	}

	public enum TouchStartPosition {
		LEFT_BOTTOM,
		LEFT_TOP,
	}

	class UrgUtil {
		///<summary>
		/// URGセンサーの位置を設定
		///</summary>
		public static Vector3 GetStartPoint(UrgStartPosition urgStartPosition) {
			Vector3 point = Vector3.zero;
			const float z = 10.0f;
			float width = Settings.urgRect.x;
			float height = Settings.urgRect.y;
			
			Camera camera = Camera.main;
			switch (urgStartPosition) {
				case UrgStartPosition.TOP:
					point = new Vector3(0, height * 0.5f, z);
					break;

				case UrgStartPosition.BOTTOM:
					point = new Vector3(0, height * -0.5f, z);
					break;

				case UrgStartPosition.RIGHT:
					point = new Vector3(width * 0.5f, 0, z);
					break;
				
				case UrgStartPosition.LEFT:
					point = new Vector3(-width * 0.5f, 0, z);
					break;

				case UrgStartPosition.TOP_LEFT:
					point = new Vector3(-width * 0.5f, height * 0.5f, z);
					break;

				case UrgStartPosition.TOP_RIGHT:
					point = new Vector3(width * 0.5f, height * 0.5f, z);
					break;
				
				case UrgStartPosition.BOTTOM_LEFT:
					point = new Vector3(-width * 0.5f, -height * 0.5f, z);
					break;
				case UrgStartPosition.BOTTOM_RIGHT:
					point = new Vector3(width * 0.5f, -height * 0.5f, z);
					break;
			}
			// point = camera.ScreenToWorldPoint(px);

			Debug.Log(point);
			return point;
		}
		
		///<summary>
		/// URGセンサーの位置を元に角度を設定
		///</summary>
		public static Vector3[] GetDirections(UrgStartPosition urgStartPosition, Vector3[] _directions) {
			Vector3[] directions = _directions;

			//謎数値?
			float d = Mathf.PI * 2 / 1440;
			float offset = d * 540;


			float r = 0;
			
			switch (urgStartPosition) {
				case UrgStartPosition.TOP:
					break;
				case UrgStartPosition.BOTTOM:
					r = Mathf.PI;
					break;
				case UrgStartPosition.RIGHT:
					r = Mathf.PI * 1.5f;
					break;
				case UrgStartPosition.LEFT:
					r = Mathf.PI * 0.5f;
					break;
				case UrgStartPosition.TOP_RIGHT:
					r = Mathf.PI * 1.75f;
					break;
				case UrgStartPosition.TOP_LEFT:
					r = Mathf.PI * 0.25f;
					break;
				case UrgStartPosition.BOTTOM_RIGHT:
					r = Mathf.PI * 1.25f;
					break;
				case UrgStartPosition.BOTTOM_LEFT:
					r = Mathf.PI * 0.75f;
					break;
			}

			for(int i = 0; i < directions.Length; i++){
				float a = d * i + offset + r;
				directions[i] = new Vector3(Mathf.Cos(a), Mathf.Sin(a), a);
			}

			return directions;
		}
	}

	
}
