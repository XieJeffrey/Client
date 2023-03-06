using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace XX
{
    public class PlayDataObj
    {
        public long id;
        public int gold;
        public string name;
        public int lv;
    }

    public class PlayDataUtil : Singleton<PlayDataUtil>, IDataUtil
    {
        public string storeKey => "playData_" + AppConst.storeKey;

        public void Init() {
            Load();
        }

        public void Load() {
            string jsonStr = PlayerPrefs.GetString(storeKey, "");
            if (string.IsNullOrEmpty(jsonStr)) {
                InitPlayerData();
            }
            else {
                App.Data.PlayData = JsonConvert.DeserializeObject<PlayDataObj>(jsonStr);
            }
        }

        public void InitPlayerData() {
            App.Data.PlayData.id = Util.Now + Random.Range(1, 1000);
            App.Data.PlayData.name = "player_" + Util.Now.ToString();
            App.Data.PlayData.gold = 0;
            App.Data.PlayData.lv = 1;

            Save();
        }

        public void Save() {
            string jsonStr = JsonConvert.SerializeObject(App.Data.PlayData);
            PlayerPrefs.SetString(storeKey, jsonStr);
        }

        /// <summary>
        /// 执行每日0点任务，比如弹出每日奖励，每日签到等等
        /// </summary>
        public void EveryDataTask() {
             
        }
    }
}
