using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Weird behaviour: 
///  1. I have a SoundManager singleton prefab in Resources/_Prefabs folder that will instantiate itself through code
///  2. Assign a reference to this prefab in a button's OnClick event
///  3. Play a sound OnClick using a function from the SoundManager
///  4. Sound should be played from an audio source that is a child of the SoundManager prefab
///  5. I get "cannot play from a disabled audio source" warning, even though the audio source is enabled
///  6. Ergo, make this silly wrapper gameobject
/// </summary>
namespace BlackHole
{
    public class SoundManagerWrapper : MonoBehaviour
    {

        private SoundManager _soundManager;

        private void Start()
        {
            // Instantiate the sound manager by calling the getter from singleton instance
            var dummy = SoundManager.Instance.name;

            _soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
        }

        public void PlayButtonPress(bool failed = false)
        {
            _soundManager.PlayButtonPress(failed);
        }

        public void PlayButtonSelect()
        {
            _soundManager.PlayButtonSelect();
        }
    }
}