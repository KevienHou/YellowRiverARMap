using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using easyar;
using SpatialMap_SparseSpatialMap;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EditeMapController : MonoBehaviour
{
    public SparseSpatialMapController mapTemp;

    private SparseSpatialMapWorkerFrameFilter mapWorker;
    private MapSession mapSession;
    private ARSession session;

    public Dragger PropDragger;

    [SerializeField]
    private MapSession.MapData mapData;


    private void Awake()
    {
#if UNITY_EDITOR
        GameObject.Find("EasyAR_SparseSpatialMapWorker").SetActive(false);
#endif

        PropDragger.CreateObject += (gameObj) =>
        {
            if (gameObj)
            {
                gameObj.transform.parent = mapData.Controller.transform;
                mapData.Props.Add(gameObj);
            }
        };
        PropDragger.DeleteObject += (gameObj) =>
        {
            if (gameObj)
            {
                mapData.Props.Remove(gameObj);
            }
        };
    }


    private void Start()
    {
        session = FindObjectOfType<ARSession>();
        mapWorker = FindObjectOfType<SparseSpatialMapWorkerFrameFilter>();

        mapSession = new MapSession(mapWorker, MapMetaManager.LoadAll());
        mapSession.LoadMapMeta(mapTemp, true);
        mapSession.currentMapData = (mapData) => { this.mapData = mapData; };

#if UNITY_EDITOR


        DebugObj();

#endif
        PropDragger.SetMapSession(mapSession);
    }

#if UNITY_EDITOR
    public int index = 0;
#endif
#if UNITY_EDITOR
    private void Update()
    {
        if (index < mapSession.Maps.Count && Input.GetKeyDown(KeyCode.P))
        {
            DebugObj();
        }
    }


    public void DebugObj()
    {
        mapData = mapSession.Maps[index];
        GameObject controller = GameObject.Find("ObjParents");
        if (controller == null)
        {
            controller = new GameObject("ObjParents");
        }
        foreach (var propInfo in mapData.Meta.Props)
        {
            GameObject prop = null;
            foreach (var templet in PropCollection.Instance.Templets)
            {
                if (templet.Object.name == propInfo.Name)
                {
                    prop = UnityEngine.Object.Instantiate(templet.Object);
                    break;
                }
            }
            if (!prop)
            {
                Debug.LogError("Missing prop templet: " + propInfo.Name);
                continue;
            }
            prop.transform.parent = controller.transform;
            prop.transform.localPosition = new UnityEngine.Vector3(propInfo.Position[0], propInfo.Position[1], propInfo.Position[2]);
            prop.transform.localRotation = new Quaternion(propInfo.Rotation[0], propInfo.Rotation[1], propInfo.Rotation[2], propInfo.Rotation[3]);
            prop.transform.localScale = new UnityEngine.Vector3(propInfo.Scale[0], propInfo.Scale[1], propInfo.Scale[2]);
            prop.name = propInfo.Name;
            mapData.Props.Add(prop);
        }
    }

#endif



    public void Save()
    {
        if (mapData == null)
        {
            return;
        }

        var propInfos = new List<MapMeta.PropInfo>();

        foreach (var prop in mapData.Props)
        {
            var position = prop.transform.localPosition;
            var rotation = prop.transform.localRotation;
            var scale = prop.transform.localScale;

            propInfos.Add(new MapMeta.PropInfo()
            {
                Name = prop.name,
                Position = new float[3] { position.x, position.y, position.z },
                Rotation = new float[4] { rotation.x, rotation.y, rotation.z, rotation.w },
                Scale = new float[3] { scale.x, scale.y, scale.z }
            });
        }
        mapData.Meta.Props = propInfos;

        MapMetaManager.Save(mapData.Meta, MapMetaManager.FileNameType.Name);
        Debug.Log("保存成功");
    }

    public void BackMain()
    {
        SceneManager.LoadScene("MainMapScene");
    }




    private void DestroySession()
    {
        if (mapSession != null)
        {
            mapSession.Dispose();
            mapSession = null;
        }
    }


    private void OnDestroy()
    {
        DestroySession();
    }


}
