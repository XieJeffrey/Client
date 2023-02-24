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
            //��ʼ����Ƶ������
            m_bgmPlayer = gameObject.AddComponent<AudioSource>();
            m_bgmPlayer.loop = true;
            for (int i = 0; i < 2; i++) {
                m_sfxPlayer.Add(gameObject.AddComponent<AudioSource>());
                m_sfxPlayer[i].loop = false;
            }
            //Ԥ������Ƶ�ļ�
            //todo
            defeered.Resolve();
        });
    }

    public void PlayBgm() {
        if (m_bgmCache == null) {
            //todo
            //��Ƶ·��δ����
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
        //��ȡ������������
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
        //��ȡ��Ƶ��Դ���в���
        if (m_sfxCache.ContainsKey(sfxName)) {
            player.clip = m_sfxCache[sfxName];
            player.Play();
        }
        else {
            //todo
            //��Ƶ��Դ·��δ����
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
