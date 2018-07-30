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
    private bool allowRotate;
    [SerializeField] private float waitTimer;

    private void Start()
    {
        //Sets all characters to false to wait for selection
        Estoc = false;
        Lilith = false;
        Crag = false;

        allowRotate = true;     //Dictates wether or not the Game Object housing the player models rotates and if the character list array's index can change
        HeavyRotation = 0;      //Sets the Game Object housing the player models rotation to 0
        Carousel.transform.localEulerAngles = new Vector3(0, 0, 0);
        characterList = new GameObject[transform.childCount];       //Sets the array's length to be the number of children under the parent Game Object
        for(int i = 0; i < transform.childCount; i++)               //Initializes the array
            characterList[i] = transform.GetChild(i).gameObject;
    }

    void Update()
    {
        if (index == 0)     //Reads the aray's index and adjusts other game objects based upon which entry in the array is selected
        {
            CharacterName.text = ("Estoc Slasher");
            CharacterBio.text = ("A duelist with a silver \n tongue, well versed \n in the arts of \n fine swordplay \n and sleight of hand. \n A charming yet sadistic \n rogue who effortlessly \n and elegantly glides \n through battle with his \n dual use of magic and \n metal.");
        }
        if (index == 1)     //Reads the aray's index and adjusts other game objects based upon which entry in the array is selected
        {
            CharacterName.text = ("Lilith Spellshade");
            CharacterBio.text = ("A dark and enigmatic \n sorceress of brilliant \n intellect. \n Although she may not \n be a master of the \n blade, her command \n of the arcane arts \n makes her a fearsome \n foe to be trifled with.");
        }
        if (index == 2)     //Reads the aray's index and adjusts other game objects based upon which entry in the array is selected
        {
            CharacterName.text = ("Crag Rockhand");
            CharacterBio.text = ("Son of a great barbarian \n chief, revered by his \n people for his monstrous \n strength and feared by \n his foes for his \n insatiable thirst for \n battle. \n This warrior has yet to \n find a challenge that he \n couldn’t solve with his \n great axe and instinct \n for bloodshed.");
        }
        Wait();
    }

    public void Wait()
    {
        if (allowRotate)
        {
            if (Input.GetAxis("Horizontal") < 0)    //Once the horizontal axis is below '0' or 'left'
            {
                GoLeft();                   //Runs the GoLeft Method
                allowRotate = false;             //Stops the player from being able to change array index or cycle characters
            }
            if (Input.GetAxis("Horizontal") > 0)    //Once the horizontal axis is above '0' or 'right'
            {
                GoRight();                  //Runs the GoRight Method
                allowRotate = false;            //Stops the player from being able to change array index or cycle characters
            }
        }
    }

    public void GoLeft()
    {
        HeavyRotation = HeavyRotation - 120;        //Reduces the value of the int 'HeavyRotation' 120
        index--;                            //Lowers the array index by 1
        if (index < 0)
            index = characterList.Length - 1;       //Once the array tries to go below its first entry, brings the array to loop around back to its last entry
        Carousel.transform.localEulerAngles = new Vector3(0, HeavyRotation, 0);     //Sets the parent objects Y rotational value to match the int value of HeavyRotation
        StartCoroutine(WaitTimer());
    }

    public void GoRight()
    {
        HeavyRotation = HeavyRotation + 120;    //Increases the value of the int 'HeavyRotation' 120
        index++;                            //Increases the array index by 1
        if (index == characterList.Length)
            index = 0;                              //Once the array tries to go above its last entry, brings the array to loop around back to its first entry '0'
        Carousel.transform.localEulerAngles = new Vector3(0, HeavyRotation, 0);     //Sets the parent objects Y rotational value to match the int value of HeavyRotation
        StartCoroutine(WaitTimer());
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

    IEnumerator WaitTimer()
    {
        print(Time.time);
        yield return new WaitForSeconds(waitTimer);
        allowRotate = true;
    }
}


