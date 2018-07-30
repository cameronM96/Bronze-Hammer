using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectorRotation : MonoBehaviour {

    private GameObject[] characterList;
    private int index;
    public GameObject Carousel;
    public Text CharacterName;
    public Text CharacterBio;
    public int HeavyRotation;
    private bool Estoc = false;
    private bool Lilith = false;
    private bool Crag = false;


    private void Start()
    {
        Estoc = false;
        Lilith = false;
        Crag = false;

        HeavyRotation = 0;
        Carousel.transform.localEulerAngles = new Vector3(0, 0, 0);
        characterList = new GameObject[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
            characterList[i] = transform.GetChild(i).gameObject;

        
    }

    void Update()
    {
        if (index == 0)
        {
            CharacterName.text = ("Estoc Slasher");
            CharacterBio.text = ("A duelist with a silver \n tongue, well versed \n in the arts of \n fine swordplay \n and sleight of hand. \n A charming yet sadistic \n rogue who effortlessly \n and elegantly glides \n through battle with his \n dual use of magic and \n metal.");
        }
        if (index == 1)
        {
            CharacterName.text = ("Lilith Spellshade");
            CharacterBio.text = ("A dark and enigmatic \n sorceress of brilliant \n intellect. \n Although she may not \n be a master of the \n blade, her command \n of the arcane arts \n makes her a fearsome \n foe to be trifled with.");
        }
        if (index == 2)
        {
            CharacterName.text = ("Crag Rockhand");
            CharacterBio.text = ("Son of a great barbarian \n chief, revered by his \n people for his monstrous \n strength and feared by \n his foes for his \n insatiable thirst for \n battle. \n This warrior has yet to \n find a challenge that he \n couldn’t solve with his \n great axe and instinct \n for bloodshed.");
        }
        Wait();
    }


    public void Wait()
    {
        if (Input.GetAxis("Horizontal") < 0)
        {
            //StartCoroutine(Wait());
            GoLeft();
        }
        if (Input.GetAxis("Horizontal") > 0)
        {
            //StartCoroutine(Wait());
            GoRight();
        }
    }
    public void GoLeft()
    {
        HeavyRotation = HeavyRotation - 120;
        index--;
        if (index < 0)
            index = characterList.Length - 1;
        Carousel.transform.localEulerAngles = new Vector3(0, HeavyRotation, 0);
        Wait();
    }

    public void GoRight()
    {
        HeavyRotation = HeavyRotation + 120;
        index++;
        if (index == characterList.Length)
            index = 0;
        Carousel.transform.localEulerAngles = new Vector3(0, HeavyRotation, 0);
        Wait();
    }

    public void Confirm()
    {
        if(index == 0)
        {
            Estoc = true;
        }
        if (index == 1)
        {
            Lilith = true;
        }
        if (index == 2)
        {
            Crag = true;
        }
    }

    //IEnumerator Wait()
    //{
    //    print(Time.time);
    //    yield return new WaitForSeconds(5);
    //}

    //    public Button RightButton;
    //    public Button LeftButton;
    //    public GameObject CharacterCycle;
    //    public Text CharacterName;
    //    public Text CharacterBio;

    //	// Use this for initialization
    //	void Start () {

    //        CharacterName = transform.Find("Text").GetComponent<Text>();
    //        CharacterBio = transform.Find("Text").GetComponent<Text>();

    //        RightButton.onClick(RightClicked);

    //    }

    //	// Update is called once per frame
    //	void Update () {

    //        if(CharacterName.text = ("Estoc Slasher"))

    //	}


}


