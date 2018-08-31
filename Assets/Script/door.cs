using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class door : MonoBehaviour {
    [SerializeField] string sceneName;
    [SerializeField] AudioClip stairSound;  
    AudioSource audioSource;
    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        audioSource.PlayOneShot(stairSound);
        Invoke("ChangeLevel", 0.1F);
    }

    private void ChangeLevel()
    {
        SceneManager.LoadScene(sceneName);
    }
}
