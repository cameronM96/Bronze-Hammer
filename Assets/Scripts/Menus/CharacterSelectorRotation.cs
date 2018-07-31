using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class CharacterSelectorRotation : MonoBehaviour {

    public Image StatScreen;
    public Sprite EstocStats;
    public Sprite LilithStats;
    public Sprite CragStats;
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
    [SerializeField] private EventSystem eventSystem;

    private void Start()
    {
        selectedPlayer = GameObject.FindGameObjectWithTag("Character Selector").GetComponent<SelectedPlayer>();
        proceedToLevel = GameObject.FindGameObjectWithTag("Proceed To Level").GetComponent<ProceedToLevel>();
        
        StatScreen = GameObject.FindGameObjectWithTag("Stats").GetComponent<Image>(); //Searches for the tagged object and gets its source image component
        StatScreen.sprite = EstocStats;         //Sets the source image

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
            CharacterBio.text = ("A DEULIST WITH A \n SILVER TONGUE, WELL \n VERSED IN THE ARTS \n OF FINE SWORDPLAY \n AND SLEIGHT OF HAND. \n A CHARMING YET \n SADISTIC ROGUE WHO \n EFFORTLESSLY AND ELEGANTLY GLIDES \n THROUGH BATTLE \n WITH HIS DUAL USE \n OF MAGIC AND METAL.");
            StatScreen.sprite = EstocStats;
        }
        if (index == 1)     //Reads the aray's index and adjusts other game objects based upon which entry in the array is selected
        {
            CharacterName.text = ("Lilith Spellshade");
            CharacterBio.text = ("A DARK AND ENIGMATIC \n SORCERESS OF \n BRILLIANT INTELLECT. \n ALTHOUGH SHE MAY NOT \n BE A MASTER OF THE \n BLADE, HER COMMAND \n OF THE ARCANE ARTS \n MAKES HER A \n FEARSOME FOE TO BE \n TRIFLED WITH.");
            StatScreen.sprite = LilithStats;
        }
        if (index == 2)     //Reads the aray's index and adjusts other game objects based upon which entry in the array is selected
        {
            CharacterName.text = ("Crag Rockhand");
            CharacterBio.text = ("SON OF A GREAT \n BARBARIAN CHIEF, \n REVERED BY HIS \n PEOPLE FOR HIS \n MONSTROUS STRENGTH \n AND FEARED BY \n HIS FOES FOR HIS \n INSATIABLE THIRST \n FOR BATTLE. \n THIS WARRIOR HAS YET \n TO FIND A CHALLENGE \n THAT HE COULDN’T \n SOLVE WITH HIS \n GREAT AXE AND \n INSTINCT FOR \n BLOODSHED.");
            StatScreen.sprite = CragStats;
        }
        Wait();

        if (Input.GetKeyDown("joystick button 0"))
        {
            Confirm();
        }

        if (Input.GetKeyDown("joystick button 1"))
        {
            SceneManager.LoadScene(0);
        }
    }

    public void Wait()
    {
        if (allowRotate && eventSystem.currentSelectedGameObject.tag == "SelectingCharacter")
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
        if(index == 0)          //Depending on what index of the array is selected by confirm, the corresponding character's bool is set to true to be used in other scripts that load them into the level.
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

    IEnumerator WaitTimer()     //Forced the program to wait between completing any more operations or accepting any more inputs
    {
        print(Time.time);
        yield return new WaitForSeconds(waitTimer);
        allowRotate = true;
    }
}


