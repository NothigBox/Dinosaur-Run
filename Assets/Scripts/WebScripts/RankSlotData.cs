using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RankSlotData
{
    private string name;
    private int score;

    public string Name => name;
    public int Score => score;
    public bool IsNotDefault => name != default && score != default;

    public RankSlotData(string slotInfo)
    {
        name = default;
        score = default;

        var dataInfo = slotInfo.Split('"');
        for (int i = 0; i < dataInfo.Length; i++)
        {
            var data = dataInfo[i];

            if (i + 2 >= dataInfo.Length) break;

            var info = dataInfo[i + 2];

            switch (data.ToLower())
            { 
                case "name":
                    name = info;
                    break;
                        
                case "score":
                    score = int.Parse(info);
                    break;
            }
        }
    }

    public RankSlotData(string name, int score) 
    {
        this.name = name;
        this.score = score;
    }

    public string ToJSON() 
    {
        return "{" + string.Format("\"Name\": \"{0}\", \"Score\": \"{1}\"", name, score.ToString("000000")) + "}";
    }
}
