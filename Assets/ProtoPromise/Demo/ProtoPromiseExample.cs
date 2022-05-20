using Proto.Promises;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;

#if UNITY_2017_2_OR_NEWER
using UnityEngine.Networking;
#endif

public class ProtoPromiseExample : MonoBehaviour
{
    public Image image;
    public string imageUrl = "https://file02.16sucai.com/d/file/2014/0829/372edfeb74c3119b666237bd4af92be5.jpg";

    private void Awake()
    {
        image.preserveAspect = true;
    }

    public void OnClick()
    {
        StartTest();
    }

    async void DownLoad() {
        Texture2D texture =  await DownloadTexture(imageUrl);
        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        Debug.Log("完成");
    }

    async void StartTest() {
        await TestStrAsync();
        Debug.Log("StartTest 完成");
    }

    async Promise<bool> TestStrAsync() {
        string str = await Test();
        Debug.Log("完成=" + str);

        return await Promise.New<bool>(deferred => {
                deferred.Resolve(true);
        });
    }

    Promise<string> Test() {
        return Promise.New<string>(deferred => {
            StartCoroutine(Delay(3, () => {
                deferred.Resolve("hello");
            }));
        });
    }

    IEnumerator Delay(float time, System.Action cb) {
        yield return new WaitForSeconds(time);

        cb();
    }

    public static Promise<Texture2D> DownloadTexture(string url)
    {
#if UNITY_2017_2_OR_NEWER
        var www = UnityWebRequestTexture.GetTexture(url);
        return PromiseYielder.WaitFor(www.SendWebRequest())
        .Then(asyncOperation =>
        {
            if (asyncOperation.webRequest.isHttpError || asyncOperation.webRequest.isNetworkError)
            {
                Debug.Log("1111111");
                throw Promise.RejectException(asyncOperation.webRequest.error);
            }
            Debug.Log("2222222222");

            return ((DownloadHandlerTexture) asyncOperation.webRequest.downloadHandler).texture;
        })
        .Finally(www.Dispose);
#else
        var www = new WWW(url);
        return PromiseYielder.WaitFor(www)
        .Then(asyncOperation =>
        {
            if (!string.IsNullOrEmpty(asyncOperation.error))
            {
                throw Promise.RejectException(asyncOperation.error);
            }
            return asyncOperation.texture;
        })
        .Finally(www.Dispose);
#endif
    }
}
