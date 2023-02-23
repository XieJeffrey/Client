using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proto.Promises;
using System;

namespace XX {
    public class DataComponent : MonoBehaviour {
        private long todayLeftTime;
        public PlayDataObj PlayData=new PlayDataObj();
        public GameDataObj GameData = new GameDataObj();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public Promise Init() {
            return Promise.New((defeered) => {
                //加载本地玩家数据
                PlayDataUtil.instance.Init();
                //初始化每日任务之后加载游戏使用到的数据
                EveryDataTask().Then(GameDataUtil.instance.Init)
                .Then(() => {
                    defeered.Resolve();
                });           
            });
        }

        /// <summary>
        /// 检测每日0点任务
        /// </summary>
        /// <returns></returns>
        public Promise EveryDataTask() {
            return Promise.New((deferred) => {
                long lastDay = long.Parse(PlayerPrefs.GetString("everyDataTask_" + AppConst.storeKey, "0"));
                if (lastDay != Util.Today) {
                    DoEveryDataTask();
                }
                todayLeftTime = Util.GetTomorrowLeftTime();
                StartCoroutine(CountDown());
                deferred.Resolve();
            });
        }

        /// <summary>
        /// 执行每日零点任务
        /// </summary>
        private void DoEveryDataTask() {
            PlayDataUtil.instance.EveryDataTask();
            PlayerPrefs.SetString("everyDataTask_" + AppConst.storeKey, Util.Today.ToString());
            todayLeftTime = Util.GetTomorrowLeftTime();
        }

        /// <summary>
        /// 当天倒计时
        /// </summary>
        /// <returns></returns>
        public IEnumerator CountDown() {
            while (true) {
                yield return new WaitForSeconds(1);
                todayLeftTime--;
                if (todayLeftTime <= 0) {
                    DoEveryDataTask();
                }
            }
        }
    }
}
