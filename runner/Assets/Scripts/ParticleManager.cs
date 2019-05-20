using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PEffect
{
    public string name;
    public ParticleSystem pEffect;

    public void Create(Vector3 pos, Quaternion rot)
    {
        GameObject.Instantiate(pEffect, pos, rot);
    }
}

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager instance;

    [SerializeField]
    PEffect[] effects;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Particle Manager: more than one Particle Manager in the Scene.");
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        for (int i = 0; i < effects.Length; i++)
        {
            GameObject _go = new GameObject("Particle effect_" + i + "_" + effects[i].name);
            _go.transform.SetParent(this.transform);
        }
    }

    public void PlayEffect(string name, Vector3 position, Quaternion rotation)
    {
        for (int i = 0; i < effects.Length; i++)
        {
            if (effects[i].name == name)
            {
                effects[i].Create(position, rotation);
                return;
            }
        }
        Debug.LogError("Particle Manager: No effect found with name " + name);
    }

    public void PlayEffect(string name, Vector3 position)
    {
        for (int i = 0; i < effects.Length; i++)
        {
            if (effects[i].name == name)
            {
                effects[i].Create(position, effects[i].pEffect.transform.rotation);
                return;
            }
        }
        Debug.LogError("Particle Manager: No effect found with name " + name);
    }

}
