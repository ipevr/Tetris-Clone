using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour {

    [SerializeField] ParticleSystem[] subParticles = new ParticleSystem[] { };

    public void SetSubParticleColor(int index, Color color) {
        ParticleSystem particleSystem = subParticles[index].GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particleSystem.main;
        main.startColor = color;
    }

}
