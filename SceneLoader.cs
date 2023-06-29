using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadARScene()
    {
        //씬 호출
        SceneManager.LoadScene("artest");
    }
}