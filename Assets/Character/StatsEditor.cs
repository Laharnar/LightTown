using UnityEngine;
using UnityEditor;

namespace Fantasy
{
    [CustomEditor(typeof(Stats))]
    public class StatsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Stats myTarget = (Stats)target;

            Stats.CharStatus[] temp = (Stats.CharStatus[])System.Enum.GetValues(typeof(Stats.CharStatus));
            if (myTarget.stats == null)
            {
                myTarget.stats = new float[temp.Length];
            }
            else if (myTarget.stats.Length < temp.Length)
            {
                float[] res = new float[temp.Length];
                for (int i = 0; i < myTarget.stats.Length; i++)
                {
                    res[i] = myTarget.stats[i];
                }
                myTarget.stats = new float[temp.Length];
                for (int i = 0; i < res.Length; i++)
                {
                    myTarget.stats[i] = res[i];
                }
            }
            else if (myTarget.stats.Length > temp.Length)
            {
                float[] res = new float[temp.Length];
                for (int i = 0; i < res.Length; i++)
                {
                    res[i] = myTarget.stats[i];
                }
                myTarget.stats = new float[temp.Length];
                for (int i = 0; i < res.Length; i++)
                {
                    myTarget.stats[i] = res[i];
                }
            }

            for (int i = 0; i < myTarget.stats.Length; i++)
            {
                myTarget.stats[i] = EditorGUILayout.FloatField(temp[i].ToString(), myTarget.stats[i], GUILayout.MaxWidth(1500));
            }

            EditorUtility.SetDirty(target);
        }
    }
}
