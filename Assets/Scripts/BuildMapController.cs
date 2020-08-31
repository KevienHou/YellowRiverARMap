using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using easyar;
using System;
using SpatialMap_SparseSpatialMap;
using UnityEngine.SceneManagement;

public class BuildMapController : MonoBehaviour
{
    //稀疏空间地图相关对象
    private ARSession session;
    private SparseSpatialMapWorkerFrameFilter mapWorker;
    private SparseSpatialMapController map;
    /// <summary>
    /// 保存按钮
    /// </summary>
    private Button openSave;
    private Button btnSave;
    private GameObject SavePanel;
    /// <summary>
    /// 显示文本
    /// </summary>
    private Text text;

    private InputField inputField;




    void Start()
    {
        //稀疏空间地图初始
        session = FindObjectOfType<ARSession>();
        mapWorker = FindObjectOfType<SparseSpatialMapWorkerFrameFilter>();
        map = FindObjectOfType<SparseSpatialMapController>();


        SavePanel = GameObject.Find("SavePanel");
        SavePanel.SetActive(false);

        inputField = SavePanel.transform.Find("Panel/InputField").GetComponent<InputField>();
        btnSave = SavePanel.transform.Find("Panel/Button").GetComponent<Button>();

        //注册追踪状态变化事件
        session.WorldRootController.TrackingStatusChanged += OnTrackingStatusChanged;
        //初始化保存按钮
        openSave = GameObject.Find("/Canvas/Button").GetComponent<Button>();
        openSave.onClick.AddListener(OpenSavePanel);
        btnSave.onClick.AddListener(Save);
        openSave.interactable = false;
        if (session.WorldRootController.TrackingStatus == MotionTrackingStatus.Tracking)
        {
            openSave.interactable = true;
        }
        else
        {
            openSave.interactable = false;
        }
        //初始化显示文本
        text = GameObject.Find("/Canvas/Panel/Text").GetComponent<Text>();
    }

    /// <summary>
    /// 保存地图方法
    /// </summary>
    private void OpenSavePanel()
    {
        openSave.interactable = false;
        SavePanel.SetActive(true);
    }

    string mapName = "";
    private void Save()
    {
        //注册地图保存结果反馈事件
        mapWorker.BuilderMapController.MapHost += SaveMapHostBack;
        //mapWorker.BuilderMapController.MapLoad += LoadMapHostBack;
        //保存地图
        try
        {
            if (!inputField.text.Equals(string.Empty))
            {
                mapName = "Map" + inputField.text;
            }
            else
            {
                mapName = "Map" + DateTime.Now.ToString("YYYY-MM-DD-HH-mm-ss");
            }
            //保存地图
            mapWorker.BuilderMapController.Host(mapName, null);
            text.text = "开始保存地图，请稍等。";
        }
        catch (Exception ex)
        {
            openSave.interactable = true;
            text.text = "保存出错：" + ex.Message;
        }
    }

    /// <summary>
    /// 保存地图反馈
    /// </summary>
    /// <param name="mapInfo">地图信息</param>
    /// <param name="isSuccess">成功标识</param>
    /// <param name="error">错误信息</param>
    private void SaveMapHostBack(SparseSpatialMapController.SparseSpatialMapInfo mapInfo, bool isSuccess, string error)
    {
        if (isSuccess)
        {
            SavePanel.SetActive(false);

            PlayerPrefs.SetString("MapID", mapInfo.ID);
            PlayerPrefs.SetString("MapName", mapInfo.Name);
            text.text = "地图保存成功。\r\nMapID：" + mapInfo.ID + "\r\nMapName：" + mapInfo.Name;
            MapMetaManager.Save(new MapMeta(mapInfo, new List<MapMeta.PropInfo>()));
            Invoke("BackMain", 3);
        }
        else
        {
            openSave.interactable = true;
            text.text = "地图保存出错：" + error;
        }
    }

    public void BackMain()
    {
        SceneManager.LoadScene("MainMapScene");
    }




    /// <summary>
    /// 摄像机状态变化
    /// </summary>
    /// <param name="status">状态</param>
    private void OnTrackingStatusChanged(MotionTrackingStatus status)
    {
        if (status == MotionTrackingStatus.Tracking)
        {
            openSave.interactable = true;
            text.text = "进入跟踪状态。";
        }
        else
        {
            openSave.interactable = false;
            text.text = "退出跟踪状态。" + status.ToString();
        }
    }
}

