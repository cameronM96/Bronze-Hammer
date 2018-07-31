using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelectorRotation : MonoBehaviour {

    private GameObject[] characterList;
    private int index;
    public GameObject Carousel;
    public Text CharacterName;
    public Text CharacterBio;
    public int HeavyRotation;
    private bool allowRotate;
    [SerializeField] private float waitTimer;
    private SelectedPlayer selectedPlayer;
    private ProceedToLevel proceedToLevel;

    private void Start()
    {
        selectedPlayer = GameObject.FindGameObjectWithTag("Character Selector").GetComponent<SelectedPlayer>();
        proceedToLevel = GameObject.FindGameObjectWithTag("Proceed To Level").GetComponent<ProceedToLevel>();

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
            CharacterBio.text = ("A DEULIST WITH A SILVER \n TONGUE, WELL VERSED \n IN THE ARTS OF \n FINE SWORDPLAY \n AND SLEIGHT OF HAND. \n A CHARMING YET SADISTIC \n ROGUE WHO EFFORTLESSLY \n AND ELEGANTLY GLIDES \n THROUGH BATTLE WITH HIS \n DUAL USE OF MAGIC AND \n METAL.");
        }
        if (index == 1)     //Reads the aray's index and adjusts other game objects based upon which entry in the array is selected
        {
            CharacterName.text = ("Lilith Spellshade");
            CharacterBio.text = ("A DARK AND ENIGMATIC \n SORCERESS OF BRILLIANT \n INTELLECT. \n ALTHOUGH SHE MAY NOT \n BE A MASTER OF THE \n BLADE, HER COMMAND \n OF THE ARCANE ARTS \n MAKES HER A FEARSOME \n FOE TO BE TRIFLED WITH.");
        }
        if (index == 2)     //Reads the aray's index and adjusts other game objects based upon which entry in the array is selected
        {
            CharacterName.text = ("Crag Rockhand");
            CharacterBio.text = ("SON OF A GREAT BARBARIAN \n CHIEF, REVERED BY HIS \n PEOPLE FOR HIS MONSTROUS \n STRENGTH AND FEARED BY \n HIS FOES FOR HIS \n INSATIABLE THIRST FOR \n BATTLE. \n THIS WARRIOR HAS YET TO \n FIND A CHALLENGE THAT HE \n COULDN’T SOLVE WITH HIS \n GREAT AXE AND INSTINCT \n FOR BLOODSHED.");
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
            selectedPlayer.EstocSelected();
        }
        if (index == 1)
        {
            selectedPlayer.LilithSelected();
        }
        if (index == 2)
        {
            selectedPlayer.CragSelected();
        }

        SceneManager.LoadScene(proceedToLevel.nextScene);
    }

    IEnumerator WaitTimer()
    {
        print(Time.time);
        yield return new WaitForSeconds(waitTimer);
        allowRotate = true;
    }
}


