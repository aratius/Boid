using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using LitJson;


public class Api
{
  private static readonly string _URL = "https://hf0xcousg6.execute-api.ap-northeast-1.amazonaws.com/BoidMainStage/";
  private static readonly string _FISH = "fish";
  private static readonly string _CHILD = "child";

  public static async UniTask<JsonData> GetList()
  {
    UnityWebRequest www = new UnityWebRequest(Api._URL + Api._FISH, "GET");
    www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    await www.SendWebRequest();

    if (www.isNetworkError || www.isHttpError)
    {
      Debug.Log(www.error);
      return null;
    }
    else
    {
      JsonData data = JsonMapper.ToObject<JsonData>(www.downloadHandler.text.Trim());
      return data;
    }
  }

}