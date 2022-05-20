using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proto.Promises;
using System;

namespace XX {
    public class DataComponent : MonoBehaviour,IProcedure {
        private long todayLeftTime;
        public PlayDataObj PlayData=new PlayDataObj();

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <returns></returns>
        public void Init() {
            PlayDataUtil.instance.Init();
            EveryDataTask().Then(() => {
                InitFinish();
            });         
        }

        public void InitFinish() { }

        public void UpdateProgress(float value) { 
            
        }

        /// <summary>
        /// ���ÿ��0������
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
        /// ִ��ÿ���������
        /// </summary>
        private void DoEveryDataTask() {
            PlayDataUtil.instance.EveryDataTask();
            PlayerPrefs.SetString("everyDataTask_" + AppConst.storeKey, Util.Today.ToString());
            todayLeftTime = Util.GetTomorrowLeftTime();
        }

        /// <summary>
        /// ���쵹��ʱ
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
