using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class TimerComponent : MonoBehaviour {
    private List<timer> timerList = new List<timer>();

    private class timer {
        public float duration = 0;
        public float time = 0;
        public int loopCount;
        public Action callBack;
        public timer(float _duration, int _loopCount, Action  _func) {
            duration = _duration;
            time = duration;
            loopCount = _loopCount;
            callBack = _func;
        }
    }

    public void Update() {
        for (int i = timerList.Count - 1; i >= 0; i--) {
            timerList[i].time -= Time.deltaTime;
            if (timerList[i].time <= 0) {
                timerList[i].callBack();
                timerList[i].loopCount--;
                if (timerList[i].loopCount <= 0) {
                    timerList.RemoveAt(i);
                }
                else
                    timerList[i].time = timerList[i].duration;
            }
        }
    }

    /// <summary>
    /// 设置定时器
    /// </summary>
    /// <param name="duration">n秒之后触发</param>
    /// <param name="func">回调函数</param>
    public void SetTimeOut(float duration, Action func) {
        timer m_timer = new timer(duration, 1, func);
        timerList.Add(m_timer);
    }

    /// <summary>
    /// 下一帧触发
    /// </summary>
    /// <param name="cb"></param>
    public void Next(Action cb) {
        StartCoroutine(NextFrameTimer(cb));
    }

    private IEnumerator NextFrameTimer(Action cb) {
        yield return new WaitForEndOfFrame();
        cb?.Invoke();
    }
}

