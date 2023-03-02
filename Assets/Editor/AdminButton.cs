using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AdminStaff))]
public class AdminButton : Editor
{
    bool active = false;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AdminStaff adminStaff = (AdminStaff)target;
        active = GUILayout.Toggle(active,("Activate Admin"));
        if (active)
        {
            adminStaff.ActivateAdmin(true);
        }
        else
        {
            adminStaff.ActivateAdmin(false);
        }
    }

}
