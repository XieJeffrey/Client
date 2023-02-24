using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proto.Promises;

public class AudioComponent : MonoBehaviour
{
    private AudioSource m_bgmPlayer = null;
    private List<AudioSource> m_sfxPlayer = new List<AudioSource>();
    private AudioClip m_bgmCache = null;
    private Dictionary<string, AudioClip> m_sfxCache = new Dictionary<string, AudioClip>();

    public Promise Init() {
        return Promise.New((defeered) => {
            //初始化音频播放器
            m_bgmPlayer = gameObject.AddComponent<AudioSource>();
            m_bgmPlayer.loop = true;
            for (int i = 0; i < 2; i++) {
                m_sfxPlayer.Add(gameObject.AddComponent<AudioSource>());
                m_sfxPlayer[i].loop = false;
            }
            //预加载音频文件
            //todo
            defeered.Resolve();
        });
    }

    public void PlayBgm() {
        if (m_bgmCache == null) {
            //todo
            //音频路径未定义
            App.ResourceMgr.LoadAssetPro<AudioClip>("").Then((AudioClip clip) => {
                m_bgmCache = clip;
                m_bgmPlayer.clip = clip;
                m_bgmPlayer.Play();
            });
        }
        else {
            this.m_bgmPlayer.Play();
        }
    }

    public void StopBgm() {
        if (this.m_bgmPlayer != null)
            this.m_bgmPlayer.Stop();
    }

    public void PlaySfx(string sfxName) {
        //先取个播放器出来
        AudioSource player = null;
        for (int i = 0; i < m_sfxPlayer.Count; i++) {
            if (!m_sfxPlayer[i].isPlaying) {
                player = m_sfxPlayer[i];
                break;
            }
        }
        if (player == null) {
            player = gameObject.AddComponent<AudioSource>();
            m_sfxPlayer.Add(player);
        }
        //获取音频资源进行播放
        if (m_sfxCache.ContainsKey(sfxName)) {
            player.clip = m_sfxCache[sfxName];
            player.Play();
        }
        else {
            //todo
            //音频资源路径未定义
            App.ResourceMgr.LoadAssetPro<AudioClip>("" + sfxName).Then((AudioClip clip) => {
                if (this.m_sfxCache.ContainsKey(sfxName))
                    return;
                this.m_sfxCache.Add(sfxName, clip);
                player.clip = clip;
                player.Play();
            });   
        }


    }
}
