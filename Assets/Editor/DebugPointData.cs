using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

public class DebugPointData : EditorWindow
{
    [MenuItem("ThursdayFrame/Tools/调试场景点云")]
    static void AddWindow_DebugPointData()
    {
        //EditorWindow window = new EditorWindow();
        EditorWindow.GetWindow<DebugPointData>(false, "调试场景点云");
        EditeMapController = GameObject.FindObjectOfType<EditeMapController>();
        EditeMapController.InitPointData();
        EnumField = new EnumField("111");
        for (int i = 0; i < EditeMapController.pointDatas.Count; i++)
        {
            //EnumField.Add(new VisualElement() { name = EditeMapController.pointDatas[i].mapName })
        }

    }

    static EditeMapController EditeMapController;
    static DropdownMenuAction DropdownMenuAction;
    static int index = 0;
    static GUILayoutOption[] gUILayoutOptions;

    static EnumField EnumField;

    private void OnGUI()
    {
        //DropdownMenuAction = new DropdownMenuAction("Data", (c) => { }, (a) => { return DropdownMenuAction.Status.Checked; });
        if (GUILayout.Button("搜索", GUILayout.Width(200)))
        {
           
        }


        if (GUILayout.Button("关闭窗口", GUILayout.Width(200)))
        {
            //关闭窗口
            this.Close();
        }


    }



}
