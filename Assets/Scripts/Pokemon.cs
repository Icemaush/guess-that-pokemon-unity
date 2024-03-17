using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pokemon : MonoBehaviour
{
    //public static Pokemon instance;

    private GameManager gm;
    public Image image;

    public void Awake()
    {
        //if (instance == null)
        //{
        //    instance = this;
        //}

        gm = GameManager.instance;

        transform.SetParent(FindObjectOfType<Canvas>().transform);

        image = gameObject.GetComponent<Image>();

        Debug.Log(gm.currentPokemonName);
        Debug.Log(gm.currentPokemonResourceName);
        image.sprite = Resources.Load<Sprite>(gm.currentPokemonResourceName);
    }

    public void RevealImage()
    {
        GetComponent<Image>().color = Color.white;
    }
}
