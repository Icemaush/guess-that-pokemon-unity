using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pokemon : MonoBehaviour
{
    //public Image image;

    public void Awake()
    {
        transform.SetParent(FindObjectOfType<Canvas>().transform);

        Image image = gameObject.GetComponent<Image>();
        image.sprite = Resources.Load<Sprite>(GameManager.instance.currentPokemonImageResourceName);

        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        Debug.Log(audioSource);
        audioSource.clip = Resources.Load<AudioClip>(GameManager.instance.currentPokemonAudioResourceName);
        Debug.Log(audioSource.clip);
    }

    public void RevealImage()
    {
        GetComponent<Image>().color = Color.white;
    }

    public void PlayAudio()
    {
        GetComponent<AudioSource>().Play();
    }
}
