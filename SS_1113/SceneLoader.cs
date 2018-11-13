using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    public CanvasGroup fadeCg; // 캔버스그룹 가져옴
    [Range(0.5f, 2.0f)] // 0.5초~2초
    public float fadeDuration = 1.0f; //페이드 되는 시간조정
    public Dictionary<string, LoadSceneMode> loadScenes = new Dictionary<string, LoadSceneMode>();
    //list는 index , Dictionary는 key값으로 데이터 관리

    void InitSceneInfo() //초기씬 정보
    {
        loadScenes.Add("Level1", LoadSceneMode.Additive);
        loadScenes.Add("scPlay", LoadSceneMode.Additive);
    }

    IEnumerator Start() // 스타트 함수를 코르틴으로 만듬, 이유는 제어권을 계속 붙잡지 않고, 
    {
        InitSceneInfo();
        fadeCg.alpha = 1.0f;
        foreach (var item in loadScenes)
        {
            yield return StartCoroutine(LoadScene(item.Key, item.Value));
        }
        StartCoroutine(Fade(0.0f));

    }

    IEnumerator LoadScene(string sceneName, LoadSceneMode mode)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, mode);
        //LoadSceneAsync 비동기 로드 = 게임은 게임대로 돌면서 백그라운드로 로드가 일어남
        //디아블로 화면효과랑 비슷

        Scene loadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        //GetSceneAt = 인덱스로 씬을 가져올 수 있음.
        //1 - 1 = 0
        //2 - 1 = 1
        SceneManager.SetActiveScene(loadedScene);
        //SetActiveScene = 씬이름으로 해당씬을 액티브해줌
    }

    IEnumerator Fade(float finalAlpha) //스타트함수가 출력이되면 fadein 되면서 부드럽게 보여주게하는효과
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Level1")); //Level1를 액티브하면서
        fadeCg.blocksRaycasts = true; //
        float fadeSpeed = Mathf.Abs(fadeCg.alpha - finalAlpha) / fadeDuration;
        while (Mathf.Approximately(fadeCg.alpha, finalAlpha)) //Approximately근사값
        {
            fadeCg.alpha = Mathf.MoveTowards(fadeCg.alpha, finalAlpha, fadeSpeed * Time.deltaTime); //부드럽게 
            yield return null;
        }
        fadeCg.blocksRaycasts = false;
        SceneManager.UnloadSceneAsync("SceneLoader");
    }
}
