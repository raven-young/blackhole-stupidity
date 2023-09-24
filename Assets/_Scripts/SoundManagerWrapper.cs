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

        public void PlayButtonPress(bool failed = false)
        {
            SoundManager.Instance.PlayButtonPress(failed);
        }

        public void PlayButtonSelect()
        {
            SoundManager.Instance.PlayButtonSelect();
        }
    }
}