using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour {

    [Header("Effects")]
    [SerializeField] AudioClip shapeLandsClip = null;
    [SerializeField] AudioClip[] fullLinesClip = null;
    [SerializeField] AudioClip gameOverClip = null;
    [SerializeField] AudioClip newHighScoreClip = null;
    [Header("Background")]
    [SerializeField] AudioSource backgroundMusic = null;

    AudioSource audioSource = null;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        backgroundMusic.loop = true;
        backgroundMusic.Play();
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

    public void PlayBackgroundMusic(bool status) {
        if (status) {
            backgroundMusic.Play();
        } else {
            backgroundMusic.Pause();
        }
    }
}
