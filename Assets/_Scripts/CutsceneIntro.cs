using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace BlackHole
{
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

        [SerializeField] private Image _blackPanel;

        //[SerializeField] private GameObject _sliders;
        //[SerializeField] private GameObject _scorePanel;

        float _fadedAlpha = 0.3f;
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

        private static List<Dialogue> dialogueListEasyNormal = new List<Dialogue>()
    {
    new Dialogue(Speakers.Racoon, "This is Captain Rockey, requesting immediate assistance!"),
    new Dialogue(Speakers.Cow, "Vachette speaking, what is it, Captain?"),
    new Dialogue(Speakers.Racoon, "Our ship flew too close to this black hole, we're gonna be sucked in!"),
    new Dialogue(Speakers.Cow, "Remember your training!"),
    new Dialogue(Speakers.Cow, "You can escape by blasting incoming asteroids..."),
    new Dialogue(Speakers.Cow, "And picking up any scrap reserves you can find."),
    new Dialogue(Speakers.Cow, "Big asteroids need a big blast, use the Mega-Laser!"),
    new Dialogue(Speakers.Racoon, "It's encoded, we don't know how to use it!"),
    new Dialogue(Speakers.Cow, "Solve the math problems, they can decipher the controls."),
    new Dialogue(Speakers.Cow, "Good luck captain.")
    };

        // to do
        private static List<Dialogue> dialogueListHard = new List<Dialogue>()
    {
    new Dialogue(Speakers.Racoon, "This is Captain Rockey, requesting immediate assistance!"),
    new Dialogue(Speakers.Cow, "Vachette speaking, don't tell me you're trapped in a black hole AGAIN?"),
    new Dialogue(Speakers.Racoon, "Yes we are! And this time it's a scary looking one!"),
    new Dialogue(Speakers.Cow, "Oh no, this sounds like a supermassive black hole."),
    new Dialogue(Speakers.Cow, "Its immense gravity wil drain your fuel reserves more quickly."),
    new Dialogue(Speakers.Cow, "Pick up any spare fuel reserves you can find or you'll be spaghettified!"),
    new Dialogue(Speakers.Racoon, "What? I'm too young to die! Oh God help me please I want to go home *cries*"),
    new Dialogue(Speakers.Cow, "Good luck captain. You'll need it!")
    };

        private int _dialogueIterator = 0;
        private bool _dialogueSkipped = false;
        private List<Dialogue> _selectedDialogue;

        private void Awake()
        {
            playerInputActions = new PlayerInputActions();
        }

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(StartIntro());

            switch (SettingsManager.Instance.SelectedDifficulty)
            {
                case SettingsManager.DifficultySetting.Easy:
                    _selectedDialogue = dialogueListEasyNormal;
                    break;
                case SettingsManager.DifficultySetting.Normal:
                    _selectedDialogue = dialogueListEasyNormal;
                    break;
                case SettingsManager.DifficultySetting.Hard:
                    _selectedDialogue = dialogueListHard;
                    break;
            }
        }

        IEnumerator StartIntro()
        {
            // Fade character 2
            _character2Container.transform.DOScale(0.95f, 0);
            _character2Panel.GetComponent<Image>().CrossFadeAlpha(_fadedAlpha, 0, true);
            _character2Speechbubble.GetComponent<Image>().CrossFadeAlpha(_fadedAlpha, 0, true);
            _character2Text.CrossFadeAlpha(_fadedAlpha, 0, true);

            _blackPanel.DOFade(0f, 1f);
            yield return new WaitForSecondsRealtime(1f);

            // Change from main menu theme to dialogue theme
            SoundManager.Instance.ChangeMusicPairSource(SoundManager.MusicSourceID.MusicSource1);

            // Start dialogue
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
            playerInputActions.Player.Answer1.performed -= SkipDialogue;
            playerInputActions.Player.Answer2.performed -= SkipDialogue;
            playerInputActions.Player.Answer3.performed -= SkipDialogue;

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
                for (int i = _dialogueIterator; i <= dialogueListEasyNormal.Count; i++)
                {
                    AdvanceDialogue(0.2f);
                }
                _dialogueSkipped = true;
            }

        }

        private void DisplayNewDialogue(Dialogue dialogue, float fadeTime = 0.1f)
        {
            bool useClip1 = Random.Range(0f, 1f) < 0.5;

            // Stop preveious character talking if player skips dialogue
            SoundManager.Instance.StopSFX();

            if (dialogue.Speaker == Speakers.Racoon)
            {
                _character1Container.transform.DOScale(1f, fadeTime);
                _character2Container.transform.DOScale(0.95f, fadeTime);

                _character1Panel.GetComponent<Image>().CrossFadeAlpha(1f, fadeTime, true);
                _character2Panel.GetComponent<Image>().CrossFadeAlpha(_fadedAlpha, fadeTime, true);

                _character1Speechbubble.GetComponent<Image>().CrossFadeAlpha(1f, fadeTime, true);
                _character2Speechbubble.GetComponent<Image>().CrossFadeAlpha(_fadedAlpha, fadeTime, true);

                _character1Text.text = dialogue.Text;
                _character1Text.CrossFadeAlpha(1f, fadeTime, true);
                _character2Text.CrossFadeAlpha(_fadedAlpha, fadeTime, true);

                if (useClip1)
                    SoundManager.Instance.PlaySound(_raccoonTalking1);
                else
                    SoundManager.Instance.PlaySound(_raccoonTalking2);

            }
            else
            {
                _character2Container.transform.DOScale(1f, fadeTime);
                _character1Container.transform.DOScale(0.95f, fadeTime);

                _character1Panel.GetComponent<Image>().CrossFadeAlpha(_fadedAlpha, fadeTime, true);
                _character2Panel.GetComponent<Image>().CrossFadeAlpha(1f, fadeTime, true);

                _character1Speechbubble.GetComponent<Image>().CrossFadeAlpha(_fadedAlpha, fadeTime, true);
                _character2Speechbubble.GetComponent<Image>().CrossFadeAlpha(1f, fadeTime, true);

                _character2Text.text = dialogue.Text;
                _character2Text.CrossFadeAlpha(1f, fadeTime, true);
                _character1Text.CrossFadeAlpha(_fadedAlpha, fadeTime, true);

                if (useClip1)
                    SoundManager.Instance.PlaySound(_cowTalking1);
                else
                    SoundManager.Instance.PlaySound(_cowTalking2);
            }
        }

        private void AdvanceDialogue(float fadeTime = 0.2f)
        {
            if (_selectedDialogue.Count <= _dialogueIterator)
            {
                SoundManager.Instance.StopSFX();
                DOTween.KillAll();
                SceneManager.LoadScene("BlackHole");
                return;
            }

            var next = _selectedDialogue[_dialogueIterator];
            DisplayNewDialogue(next, fadeTime);
            _dialogueIterator++;
        }
    }
}