using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour {

    [Header("Inputs")]
    [SerializeField] AudioClip shapeMoveLeftRightClip = null;
    [SerializeField] AudioClip shapeRotateClip = null;
    [SerializeField] AudioClip shapeDropDownClip = null;
    [Header("Effects")]
    [SerializeField] AudioClip shapeLandsClip = null;
    [SerializeField] AudioClip[] fullLinesClip = null;
    [SerializeField] AudioClip gameOverClip = null;
    [SerializeField] AudioClip newHighScoreClip = null;
    [SerializeField] IconToggle iconToggleSound = null;
    [Header("Background")]
    [SerializeField] AudioSource backgroundMusic = null;
    [SerializeField] IconToggle iconToggleMusic = null;

    AudioSource audioSource = null;
    bool musicToggledOff = false;
    bool soundToggledOff = false;
    bool gamePaused = false;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        backgroundMusic.loop = true;
        backgroundMusic.Play();
    }

    public void PlayClipShapeMoveLeftRight() {
        if (!soundToggledOff) {
            audioSource.PlayOneShot(shapeMoveLeftRightClip);
        }
    }

    public void PlayClipShapeRotate() {
        if (!soundToggledOff) {
            audioSource.PlayOneShot(shapeRotateClip);
        }
    }

    public void PlayClipShapeDropDown() {
        if (!soundToggledOff) {
            audioSource.PlayOneShot(shapeDropDownClip);
        }
    }

    public void PlayClipShapeLands() {
        if (!soundToggledOff) {
            audioSource.PlayOneShot(shapeLandsClip);
        }
    }

    public void PlayFullLinesClip(int index) {
        if (!soundToggledOff) {
            audioSource.PlayOneShot(fullLinesClip[index]);
        }
    }

    public void PlayGameOverClip() {
        if (!soundToggledOff) {
            audioSource.PlayOneShot(gameOverClip);
        }
    }

    public void PlayNewHighScoreClip() {
        if (!soundToggledOff) {
            audioSource.clip = newHighScoreClip;
            audioSource.PlayDelayed(gameOverClip.length + 1f);
        }
    }

    public void ToggleSound() {
        if (gamePaused) {
            return;
        }
        soundToggledOff = !soundToggledOff;
        iconToggleSound.ToggleIcon(!soundToggledOff);
    }

    public void PlayBackgroundMusic(bool status) {
        backgroundMusic.mute = false;
        gamePaused = status ? false : true;
        if (status && !musicToggledOff) {
            backgroundMusic.UnPause();
        } else {
            backgroundMusic.Pause();
        }
    }

    public void ToggleBackgroundMusic() {
        backgroundMusic.mute = false;
        if (gamePaused) {
            return;
        }
        musicToggledOff = !musicToggledOff;
        if (!musicToggledOff) {
            backgroundMusic.UnPause();
            iconToggleMusic.ToggleIcon(true);
        } else {
            backgroundMusic.Pause();
            iconToggleMusic.ToggleIcon(false);
        }
    }

    public void MuteBackGroundMusic(bool status) {
        backgroundMusic.mute = status;
        iconToggleMusic.ToggleIcon(!status);
        musicToggledOff = status;
    }
}
