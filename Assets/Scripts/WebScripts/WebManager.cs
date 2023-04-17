using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebManager : MonoBehaviour
{
    private const string FIREBASE_URL = "https://timba-technical-test-default-rtdb.firebaseio.com/";
    private const string DATA_JSON = "Ranking.json";

    private List<RankSlotData> rankSlots;

    public event Action OnRankingDeleted;
    public event Action<List<RankSlotData>> OnRankingGet;

    private void Awake()
    {
        rankSlots = new List<RankSlotData>();
    }

    public void GetData() 
    {
        StartCoroutine(GettingData());
    }

    public void PostData(RankSlotData data)
    {
        StartCoroutine(PostingData(data.ToJSON()));
    }

    public void PutData(string jsonPath)
    {
        var rankingJSON = (TextAsset) Resources.Load(jsonPath);

        StartCoroutine(PuttingData(rankingJSON.text));
    }

    public void DeleteData()
    {
        StartCoroutine(DeletingData());
    }

    //Download data from the server.
    private IEnumerator GettingData()
    {
        var request = UnityWebRequest.Get(FIREBASE_URL + DATA_JSON);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            rankSlots = new List<RankSlotData>();
            var rankSlotsInfos = request.downloadHandler.text.Split('}');

            foreach (var slotInfo in rankSlotsInfos)
            {
                var newSlot = new RankSlotData(slotInfo);
                if (newSlot.IsNotDefault)
                {
                    rankSlots.Add(newSlot);
                }
            }
        }
        else rankSlots = null;

        //Sort from higher to lower
        rankSlots.Sort((s1, s2) => s2.Score.CompareTo(s1.Score));

        OnRankingGet?.Invoke(rankSlots);
    }

    //Upload data to the server.
    private IEnumerator PostingData(string info)
    {
        var i = System.Text.Encoding.UTF8.GetBytes(info);
        var request = new UnityWebRequest(FIREBASE_URL + DATA_JSON, "POST");
        request.uploadHandler = new UploadHandlerRaw(i);

        yield return request.SendWebRequest();
    }

    //Set a new JSON as the data on the server.
    private IEnumerator PuttingData(string ranking)
    {
        var rankingBytes = System.Text.Encoding.UTF8.GetBytes(ranking);

        var request = UnityWebRequest.Put(FIREBASE_URL + DATA_JSON, rankingBytes);
        yield return request.SendWebRequest();

        RefreshData(request);
    }

    //Clear all data on the server.
    private IEnumerator DeletingData()
    {
        var request = UnityWebRequest.Delete(FIREBASE_URL + DATA_JSON);
        yield return request.SendWebRequest();

        RefreshData(request);
    }

    private void RefreshData(UnityWebRequest request) 
    {
        OnRankingDeleted?.Invoke();

        if (request.result == UnityWebRequest.Result.Success)
        {
            GetData();
        }
    }
}
