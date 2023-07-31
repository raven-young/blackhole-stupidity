using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;

// dumbest dialogue system imaginable due to lack of time
public class CutsceneIntro : MonoBehaviour
{

    private PlayerInputActions playerInputActions;

    [SerializeField] private GameObject _character1Container;
    [SerializeField] private GameObject _character2Container;

    [SerializeField] private GameObject _character1Panel;
    [SerializeField] private GameObject _character2Panel;

    [SerializeField] private GameObject _character1Speechbubble;
    [SerializeField] private GameObject _character2Speechbubble;

    [SerializeField] private TMP_Text _character1Text;
    [SerializeField] private TMP_Text _character2Text;

    [SerializeField] private AudioClip _raccoonTalking1, _raccoonTalking2;
    [SerializeField] private AudioClip _cowTalking1, _cowTalking2;

    //[SerializeField] private GameObject _sliders;
    //[SerializeField] private GameObject _scorePanel;

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
	new Dialogue(Speakers.Racoon, "This is Captain Rockey, requesting immediate assistance!"),
    new Dialogue(Speakers.Cow, "Vachette speaking, what is it, Captain?"),
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
        // Change from main menu theme to music source theme
        SoundManager.Instance.ChangeMusicPairSource(SoundManager.MusicSourceID.MusicSource1);
        AdvanceDialogue();
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
            for (int i = _dialogueIterator; i <= dialogueList.Count; i++)
            {
                AdvanceDialogue(0.2f);
            }
            _dialogueSkipped = true;
        }
            
    }

    private void DisplayNewDialogue(Dialogue dialogue, float fadeTime = 0.1f)
    {
        float fadedAlpha = 0.3f;
        bool useClip1 = Random.Range(0f, 1f) < 0.5;

        // Stop preveious character talking if player skips dialogue
        SoundManager.Instance.StopSFX();

        if (dialogue.Speaker == Speakers.Racoon)
        {
            _character1Container.transform.DOScale(1f, fadedAlpha);
            _character2Container.transform.DOScale(0.95f, fadedAlpha);

            _character1Panel.GetComponent<Image>().CrossFadeAlpha(1f, fadeTime, true);
            _character2Panel.GetComponent<Image>().CrossFadeAlpha(fadedAlpha, fadeTime, true);

            _character1Speechbubble.GetComponent<Image>().CrossFadeAlpha(1f, fadeTime, true);
            _character2Speechbubble.GetComponent<Image>().CrossFadeAlpha(fadedAlpha, fadeTime, true);

            _character1Text.text = dialogue.Text;
            _character1Text.CrossFadeAlpha(1f, fadeTime, true);
            _character2Text.CrossFadeAlpha(fadedAlpha, fadeTime, true);

            if (useClip1) 
                SoundManager.Instance.PlaySound(_raccoonTalking1);
            else
                SoundManager.Instance.PlaySound(_raccoonTalking2);

        } else
        {
            _character2Container.transform.DOScale(1f, fadedAlpha);
            _character1Container.transform.DOScale(0.95f, fadedAlpha);

            _character1Panel.GetComponent<Image>().CrossFadeAlpha(fadedAlpha, fadeTime, true);
            _character2Panel.GetComponent<Image>().CrossFadeAlpha(1f, fadeTime, true);

            _character1Speechbubble.GetComponent<Image>().CrossFadeAlpha(fadedAlpha, fadeTime, true);
            _character2Speechbubble.GetComponent<Image>().CrossFadeAlpha(1f, fadeTime, true);

            _character2Text.text = dialogue.Text;
            _character2Text.CrossFadeAlpha(1f, fadeTime, true);
            _character1Text.CrossFadeAlpha(fadedAlpha, fadeTime, true);

            if (useClip1)
                SoundManager.Instance.PlaySound(_cowTalking1);
            else
                SoundManager.Instance.PlaySound(_cowTalking2);
        }
    }

    private void AdvanceDialogue(float fadeTime = 0.2f)
    {
        if (dialogueList.Count <= _dialogueIterator)
        {
            SoundManager.Instance.StopSFX();
            DOTween.KillAll();
            SceneManager.LoadScene("BlackHole");
            return;
        }

        var next = dialogueList[_dialogueIterator];
        DisplayNewDialogue(next, fadeTime);
        _dialogueIterator++;
    }
}

