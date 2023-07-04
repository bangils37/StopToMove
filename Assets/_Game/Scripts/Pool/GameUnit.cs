using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameUnit : MonoBehaviour
{
    private Transform tf;
    public PoolType poolType;

    public Transform TF
    {
        get
        {
            tf = tf ?? gameObject.transform;
            return tf;
        }
    }

    public abstract void OnInit();
    public abstract void OnInit(CharacterController t);
    public abstract void OnInit(Vector3 spawnPosition, Vector3 targetEnemy);
    public abstract void OnDespawn();

}