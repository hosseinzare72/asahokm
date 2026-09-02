using UnityEngine;
using UnityEngine.UI;

public class RulesScreen : MonoBehaviour
{
    void Start()
    {
        GameObject.Find("BtnAccept").GetComponent<Button>().onClick.AddListener(AcceptRules);
        GameObject.Find("BtnExit").GetComponent<Button>().onClick.AddListener(ExitGame);
    }

    public void AcceptRules()
    {
        PlayerPrefs.SetInt("rules_accepted", 1);
        PlayerPrefs.Save();
        Debug.Log("Rules accepted");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}