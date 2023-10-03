using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FindPlayer : MonoBehaviour
{

    CinemachineVirtualCamera _virtualCamera;

    public void SetActivePlayer(GameObject PlayerToFollow)
    {
        _virtualCamera.m_Follow = PlayerToFollow.transform;
    }

    private void OnEnable()
    {
        
    }

}
