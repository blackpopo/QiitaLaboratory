using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Utils;
using System;
using System.IO;

[System.Serializable]
public class MagicaCollider
{
    public string boneName;
    public Vector3 center;
    public MagicaCloth.MagicaCapsuleCollider.Axis axis;

    public float length;

    public float startRadius;

    public float endRadius;
}

[CustomEditor(typeof(SetColliders))]
[CanEditMultipleObjects]
public class SetCollidersEditor : Editor
{
    SetColliders setColliders;

    public override void OnInspectorGUI()
    {

        setColliders = target as SetColliders;
        base.OnInspectorGUI();
        serializedObject.Update ();

        EditorGUILayout.Space();

        string fileName = "MagicaRecord";

        EditorGUILayout.LabelField("Recording Collider json file name");
        fileName = EditorGUILayout.TextField(fileName);
        EditorGUILayout.Space();

        // if(GUILayout.Button("コライダーセットする！")){
        //     if(setColliders.Model == null){
        //         Debug.Log("モデルがセットされていません！");
        //         return;
        //     }
        //     for(int i = 0 ; i< setColliders.colliderBoneNames.Count; i++){
        //         if(setColliders.colliderStartRadius[i] < 0.01f && setColliders.colliderEndRadius[i] < 0.01f){
        //             continue;
        //         }
        //         var obj = Utils.FindTransform.FindDeep(setColliders.Model, setColliders.colliderBoneNames[i]);
        //         var collider = obj.gameObject.AddComponent<MagicaCloth.MagicaCapsuleCollider>();
        //         collider.Center = setColliders.colliderBoneVecs[i];
        //         collider.AxisMode = setColliders.colliderAxis[i];
        //         collider.Length = setColliders.colliderLength[i];
        //         collider.StartRadius = setColliders.colliderStartRadius[i];
        //         collider.EndRadius = setColliders.colliderEndRadius[i];
        //         foreach(var boneCloth in setColliders.magicaCloths){
        //             boneCloth.AddCollider(collider);
        //         }
        //     }
        // }

        if(GUILayout.Button("コライダーをすべて削除")){
            if(setColliders.Model == null){
                Debug.Log("モデルがセットされていません！");
                return;
            }
            for(int i = 0 ; i< setColliders.colliderBoneNames.Count; i++){
                var obj = Utils.FindTransform.FindDeep(setColliders.Model, setColliders.colliderBoneNames[i]);
                var collider = obj.gameObject.GetComponent<MagicaCloth.MagicaCapsuleCollider>();
                if(collider != null){
                    foreach(var boneCloth in setColliders.magicaCloths){
                        boneCloth.RemoveCollider(collider);
                    }
                    DestroyImmediate(collider);
                }
            }
            Debug.Log("コライダーをすべて削除しました！");
        }

        if(GUILayout.Button("コライダーをjsonに記憶")){
            if(setColliders.Model == null){
                Debug.Log("モデルがセットされていません！");
                return;
            }
            List<string> recordList = new List<string>();
            for(int i=0; i<setColliders.colliderBoneNames.Count; i++){
                var obj = Utils.FindTransform.FindDeep(setColliders.Model, setColliders.colliderBoneNames[i]);
                var collider = obj.gameObject.GetComponent<MagicaCloth.MagicaCapsuleCollider>();
                if(collider != null){
                    setColliders.colliderBoneVecs[i] = collider.Center;
                    setColliders.colliderAxis[i] = collider.AxisMode;
                    setColliders.colliderLength[i] = collider.Length;
                    setColliders.colliderStartRadius[i] = collider.StartRadius;
                    setColliders.colliderEndRadius[i] = collider.EndRadius;
                    
                    var magicaCollier = new MagicaCollider();
                    magicaCollier.center = collider.Center;
                    magicaCollier.boneName = setColliders.colliderBoneNames[i];
                    magicaCollier.length = collider.Length;
                    magicaCollier.axis = collider.AxisMode;
                    magicaCollier.startRadius = collider.StartRadius;
                    magicaCollier.endRadius = collider.EndRadius;

                    recordList.Add(JsonUtility.ToJson(magicaCollier));
                }
            }
            if(recordList.Count > 0){
                string path = Application.dataPath + "/" + fileName + ".json";
                using(StreamWriter sw = new StreamWriter(path)){
                    foreach(var record in recordList){
                        sw.WriteLine(record);
                    }
                }
                Debug.Log("コライダーをすべて記憶しました！");
            }else{
                Debug.Log("コライダーが存在しませんでした.");
            }

        }

        if(GUILayout.Button("コライダーをjsonから読みこみ")){
            if(setColliders.Model == null){
                Debug.Log("モデルがセットされていません！");
                return;
            }

            List<string> recordList = new List<string>();

            string path = Application.dataPath + "/"+ fileName + ".json";
            using(StreamReader sr = new StreamReader(path)){
                while(sr.Peek() != -1){
                    var line = sr.ReadLine();
                    recordList.Add(line);
                }

            }

            foreach(var record in recordList){
                var jsonCollider = JsonUtility.FromJson<MagicaCollider>(record);
                var obj = Utils.FindTransform.FindDeep(setColliders.Model, jsonCollider.boneName);
                if(obj.gameObject.GetComponent<MagicaCloth.MagicaCapsuleCollider>() == null){
                    var collider = obj.gameObject.AddComponent<MagicaCloth.MagicaCapsuleCollider>();
                    collider.Center = jsonCollider.center;
                    collider.AxisMode = jsonCollider.axis;
                    collider.Length = jsonCollider.length;
                    collider.EndRadius = jsonCollider.endRadius;
                    collider.StartRadius = jsonCollider.startRadius;
                    foreach(var boneCloth in setColliders.magicaCloths){
                        boneCloth.TeamData.ColliderList.Add(collider);
                    }
                }

            }
            Debug.Log("コライダーをすべてjsonから読み取りました！");
        }

       EditorGUILayout.Space();

        for(int i = 0; i< setColliders.colliderBoneNames.Count; i++){
            setColliders.colliderEndRadius.Add(0.0f);
            setColliders.colliderStartRadius.Add(0.0f);
            setColliders.colliderLength.Add(0.0f);
            setColliders.colliderBoneVecs.Add(Vector3.zero);
            setColliders.colliderAxis.Add(MagicaCloth.MagicaCapsuleCollider.Axis.X);
            // EditorGUILayout.LabelField(setColliders.colliderBoneNames[i]);

            EditorGUILayout.Space();
        
        }


    //         EditorGUILayout.BeginVertical();
    //         setColliders.colliderBoneVecs[i]= EditorGUILayout.Vector3Field("Center", setColliders.colliderBoneVecs[i]);
    //         EditorGUILayout.Space();
    //         setColliders.colliderAxis[i] = (MagicaCloth.MagicaCapsuleCollider.Axis)EditorGUILayout.EnumPopup((Enum)setColliders.colliderAxis[i]);
    //         EditorGUILayout.Space();
    //         setColliders.colliderLength[i] = EditorGUILayout.FloatField("Length", setColliders.colliderLength[i]);
    //         EditorGUILayout.Space();
    //         setColliders.colliderStartRadius[i] = EditorGUILayout.FloatField("Start Radius", setColliders.colliderStartRadius[i]);
    //         EditorGUILayout.Space();
    //         setColliders.colliderEndRadius[i] = EditorGUILayout.FloatField("End Radius", setColliders.colliderEndRadius[i]);
    //         EditorGUILayout.EndVertical();

    //         EditorGUILayout.Space();
    //         EditorGUILayout.Space();
    //         EditorGUILayout.Space();

    //     }
    }
}
