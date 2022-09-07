using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using Utils;
using MagicaCloth;

public class SetColliders : MonoBehaviour
{
    public GameObject Model;

    public MagicaCloth.MagicaBoneCloth[] magicaCloths;

    [HideInInspector]
    public List<string> colliderBoneNames = new List<string>{
        "J_Bip_C_Head",
        "J_Bip_C_UpperChest",
        "J_Bip_C_Chest",
        "J_Bip_C_Spine",
        "J_Bip_L_UpperArm",
        "J_Bip_R_UpperArm",
        "J_Bip_L_LowerArm",
        "J_Bip_R_LowerArm",
        "J_Bip_L_Hand",
        "J_Bip_R_Hand",
        "J_Bip_L_UpperLeg",
        "J_Bip_R_UpperLeg",
        "J_Bip_L_LowerLeg",
        "J_Bip_R_LowerLeg",
        "J_Bip_L_Shoulder",
        "J_Bip_R_Shoulder"
    };
    

    [HideInInspector]
    public List<Vector3> colliderBoneVecs = new List<Vector3>(){};
    [HideInInspector]
    public List<MagicaCapsuleCollider.Axis> colliderAxis = new List<MagicaCapsuleCollider.Axis>();
    [HideInInspector]
    public List<float> colliderLength = new List<float>();
    [HideInInspector]
    public List<float> colliderStartRadius = new List<float>();
    [HideInInspector]
    public List<float> colliderEndRadius = new List<float>();

}
