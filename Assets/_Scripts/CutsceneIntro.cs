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
        new Dialogue(Speakers.Racoon, "hello"),
        new Dialogue(Speakers.Racoon, "anybody here"),
        new Dialogue(Speakers.Cow, "moo")
    };

    private int _dialogueIterator = 0;

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

    private void DisplayNewDialogue(Dialogue dialogue)
    {
        float fadeTime = 0.1f;
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

    private void AdvanceDialogue()
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
        DisplayNewDialogue(next);
        _dialogueIterator++;
    }
}

