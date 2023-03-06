using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartWindow : Window {
    public RectTransform loading;
    public RectTransform loadingBg;
    public Text lvTxt;

    public override void OnInit() {
        base.OnInit();
    }

    public override void OnShow(params object[] args) {
        lvTxt.text = App.Data.PlayData.lv.ToString().PadLeft(3,'0');
        loading.sizeDelta = new Vector2(0, loading.sizeDelta.y);
        StartCoroutine(Loading());
        App.gameMgr.InitSudoku();
        base.OnShow(args);
    }

    public override void RegistEvents() {
        base.RegistEvents();
    }


    private IEnumerator Loading() {
        float duration = 2f;
        float delta = loadingBg.sizeDelta.x / duration;
        while (true) {
            float width = loading.sizeDelta.x;
            width += Time.deltaTime * delta;
            loading.sizeDelta= new Vector2(width, loading.sizeDelta.y);
            duration -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
            if (duration <= 0f) {
                StopAllCoroutines();
                App.UI.ShowWindow(UIType.game);
            }
        }
    }
}

