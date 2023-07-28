using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

// dumbest dialogue system imaginable due to lack of time
public class CutsceneIntro : MonoBehaviour
{

    private PlayerInputActions playerInputActions;

    [SerializeField] private GameObject _character1Panel;
    [SerializeField] private GameObject _character2Panel;

    [SerializeField] private GameObject _character1Speechbubble;
    [SerializeField] private GameObject _character2Speechbubble;

    [SerializeField] private TMP_Text _character1Text;
    [SerializeField] private TMP_Text _character2Text;

    [SerializeField] private GameObject _sliders;
    [SerializeField] private GameObject _scorePanel;

    private enum Speakers
    {
        Racoon, // Speaker 1
        Cow // Speaker 2
    }

    private struct Dialogue
    {
        public Dialogue(Speakers speaker, string text)
        {
            Speaker = speaker;
            Text = text;
        }

        public Speakers Speaker { get; }
        public string Text { get; }
    }

    private static List<Dialogue> dialogueList = new List<Dialogue>()
    {
	new Dialogue(Speakers.Racoon, "This is captain Rockey, requesting immediate assistance!"),
        new Dialogue(Speakers.Cow, "Vachette speaking, what is it captain?"),
        new Dialogue(Speakers.Racoon, "Our ship flew too close to this black hole, we're gonna be sucked in!"),
	new Dialogue(Speakers.Cow, "Remember your training!"),
	new Dialogue(Speakers.Cow, "You can escape by blasting incoming asteroids..."),
	new Dialogue(Speakers.Cow, "And picking up any spare fuel reserves you can find."),
	new Dialogue(Speakers.Cow, "Big asteroids need a big blast, use the Mega-Laser!"),
	new Dialogue(Speakers.Racoon, "It's encoded, we don't know how to use it!"),
	new Dialogue(Speakers.Cow, "Solve the math problems, they can decipher the controls."),
	new Dialogue(Speakers.Cow, "Good luck captain.")
    };

    private int _dialogueIterator = 0;
    private bool _dialogueSkipped = false;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }

    // Start is called before the first frame update
    void Start()
    {
        AdvanceDialogue();
        Debug.Log(dialogueList);
    }

    private void OnEnable()
    {
        playerInputActions.Player.Enable();
        playerInputActions.Player.Fire.performed += AdvanceDialogueAction;
        playerInputActions.Player.Answer1.performed += SkipDialogue;
        playerInputActions.Player.Answer2.performed += SkipDialogue;
        playerInputActions.Player.Answer3.performed += SkipDialogue;
    }

    void OnDisable()
    {
        playerInputActions.Player.Disable();
        playerInputActions.Player.Fire.performed -= AdvanceDialogueAction;

    }

    private void AdvanceDialogueAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            AdvanceDialogue();
        }
    }

    private void SkipDialogue(InputAction.CallbackContext context)
    {
        if (context.performed && !_dialogueSkipped)
        {
            Debug.Log("skippin:" + dialogueList.Count + " " + _dialogueIterator);

            for (int i = _dialogueIterator; i <= dialogueList.Count; i++)
            {
                AdvanceDialogue(0.2f);
                Debug.Log("skip");
            }
            _dialogueSkipped = true;
        }
            
    }

    private void DisplayNewDialogue(Dialogue dialogue, float fadeTime = 0.1f)
    {
        float fadedAlpha = 0.3f;
        if (dialogue.Speaker == Speakers.Racoon)
        {
            _character1Panel.GetComponent<Image>().CrossFadeAlpha(1f, fadeTime, true);
            _character2Panel.GetComponent<Image>().CrossFadeAlpha(fadedAlpha, fadeTime, true);

            _character1Speechbubble.GetComponent<Image>().CrossFadeAlpha(1f, fadeTime, true);
            _character2Speechbubble.GetComponent<Image>().CrossFadeAlpha(fadedAlpha, fadeTime, true);

            _character1Text.text = dialogue.Text;
            _character1Text.CrossFadeAlpha(1f, fadeTime, true);
            _character2Text.CrossFadeAlpha(fadedAlpha, fadeTime, true);
        } else
        {
            _character1Panel.GetComponent<Image>().CrossFadeAlpha(fadedAlpha, fadeTime, true);
            _character2Panel.GetComponent<Image>().CrossFadeAlpha(1f, fadeTime, true);

            _character1Speechbubble.GetComponent<Image>().CrossFadeAlpha(fadedAlpha, fadeTime, true);
            _character2Speechbubble.GetComponent<Image>().CrossFadeAlpha(1f, fadeTime, true);

            _character2Text.text = dialogue.Text;
            _character2Text.CrossFadeAlpha(1f, fadeTime, true);
            _character1Text.CrossFadeAlpha(fadedAlpha, fadeTime, true);
        }
    }

    private void AdvanceDialogue(float fadeTime = 1.0f)
    {
        if (dialogueList.Count <= _dialogueIterator)
        {
            GameManager.Instance.StartGame();
            _sliders.SetActive(true);
            _scorePanel.SetActive(true);
            gameObject.SetActive(false);
            return;
        }

        var next = dialogueList[_dialogueIterator];
        DisplayNewDialogue(next, fadeTime);
        _dialogueIterator++;
    }
}

