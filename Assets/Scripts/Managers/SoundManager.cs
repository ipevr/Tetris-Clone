using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    [SerializeField] AudioClip shapeLandsClip = null;
    [SerializeField] AudioClip[] fullLinesClip = null;
    [SerializeField] AudioClip gameOverClip = null;
    [SerializeField] AudioClip newHighScoreClip = null;

    AudioSource audioSource = null;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayClipShapeLands() {
        audioSource.PlayOneShot(shapeLandsClip);
    }

    public void PlayFullLinesClip(int index) {
        audioSource.PlayOneShot(fullLinesClip[index]);
    }

    public void PlayGameOverClip() {
        audioSource.PlayOneShot(gameOverClip);
    }

    public void PlayNewHighScoreClip() {
        audioSource.clip = newHighScoreClip;
        audioSource.PlayDelayed(gameOverClip.length + 1f);
    }
}
