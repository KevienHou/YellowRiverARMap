using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

    private VideoCameraDevice videoCamera;


    [SerializeField]
    private MapSession.MapData mapData;
    private PointData pointData;
    private List<PointData> pointDatas = new List<PointData>();

    private void Awake()
    {

        session = FindObjectOfType<ARSession>();
        mapWorker = FindObjectOfType<SparseSpatialMapWorkerFrameFilter>();
        videoCamera = session.GetComponentInChildren<VideoCameraDevice>();
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

        mapSession = new MapSession(mapWorker, MapMetaManager.LoadAll());
        mapSession.LoadMapMeta(mapTemp, true);
        mapSession.CurrentMapLoad = (mapData) =>
        {
            pointDatas.Add(new PointData() { mapName = mapData.Meta.Map.Name, PointCloud = mapData.Controller.PointCloud });
        };
        mapSession.CurrentMapLocalized = (mapData) =>
        {
            this.mapData = mapData;
        };
        //mapSession.CurrentMapStopLocalized = (mapData) => { };
        videoCamera.DeviceOpened += () =>
        {
            if (videoCamera == null)
            {
                return;
            }
            videoCamera.FocusMode = CameraDeviceFocusMode.Continousauto;
        };


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
        pointDatas = MapMetaManager.Load_PointCloud<PointData>();
        UpdatePointCloud(GetCurrentPointData);

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

    public ParticleSystem PointCloudParticleSystem;
    SparseSpatialMapController.ParticleParameter pointCloudParticleParameter = new SparseSpatialMapController.ParticleParameter() { StartSize = 0.02f };


    private void UpdatePointCloud(PointData PointData)
    {
        if (string.IsNullOrEmpty(PointData.mapName))
        {
            PointCloudParticleSystem.Clear();
            return;
        }

        Debug.Log(PointData.PointCloud.Count);

        if (!PointCloudParticleSystem)
        {
            return;
        }

        var particles = PointData.PointCloud.Select(p =>
        {
            var particle = new ParticleSystem.Particle();
            particle.position = p;
            particle.startLifetime = pointCloudParticleParameter.StartLifetime;
            particle.remainingLifetime = pointCloudParticleParameter.RemainingLifetime;
            particle.startSize = pointCloudParticleParameter.StartSize;
            particle.startColor = pointCloudParticleParameter.StartColor;
            return particle;
        }).ToArray();
        PointCloudParticleSystem.SetParticles(particles, particles.Length);
    }

    private PointData GetCurrentPointData
    {
        get
        {
            return pointDatas.Find(a => a.mapName == mapData.Meta.Map.Name);
        }
    }




#endif

    public void SavePoint()
    {
        Debug.Log(mapData.Controller.PointCloud.Count);
        MapMetaManager.Save(mapData.Controller.PointCloud, mapData.Meta, MapMetaManager.FileNameType.Name);
    }




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


public struct PointData
{
    public string mapName;
    public List<Vector3> PointCloud;
}

