using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flashlight : MonoBehaviour, IProp
{
    [SerializeField] private Transform m_armsRoot;

    private bool m_holster = true;

    public void SetHolster(bool holster)
    {
        m_holster = holster;
    }

    public void StateUpdate()
    {

    }

    public void Fire()
    {

    }

    public void Reload() {}
    public void CancelReload() {}

    public bool IsReloading()
    {
        return false;
    }

    public bool IsHolstered()
    {
        return m_holster;
        //return m_anim.IsHostered();
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public Transform GetArmsRoot()
    {
        return m_armsRoot;
    }
}
