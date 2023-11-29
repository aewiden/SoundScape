using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
   public void mellow()
    {
        SceneManager.LoadSceneAsync("mellowFinal");
    }

    public void jazz()
    {
        SceneManager.LoadSceneAsync("jazzFinal");
    }

    public void techno()
    {
        SceneManager.LoadSceneAsync("technoFinal");
    }

    public void synth()
    {
        SceneManager.LoadSceneAsync("synthFinal");
    }

    public void back()
    {
        SceneManager.LoadSceneAsync("landingScene");
    }

}
