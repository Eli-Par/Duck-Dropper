using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(Static_Queue))]
public class Static_Queue_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        Static_Queue queue = (Static_Queue)target;

        if (GUILayout.Button("Order Sections"))
        {
            queue.sectionIndices = new int[queue.sectionsList.Length];

            //Make a copy of the current sections array
            GameObject[] oldSections = new GameObject[queue.sectionsList.Length];
            for(int i = 0; i < queue.sectionsList.Length; i++)
            {
                oldSections[i] = queue.sectionsList[i];
            }

            //Order the sections array
            for(int i = 0; i < queue.sectionsList.Length; i += queue.width)
            {
                SortWidth(queue, i);
            }

            //Make the array of old to new indexes
            for(int i = 0; i < oldSections.Length; i++)
            {
                for(int k = 0; k < queue.sectionsList.Length; k++)
                {
                    if(oldSections[i] == queue.sectionsList[k])
                    {
                        queue.sectionIndices[i] = k;
                    }
                }
            }
        }

        if (GUILayout.Button("Completely Order Sections"))
        {
            queue.sectionIndices = new int[queue.sectionsList.Length];

            //Make a copy of the current sections array
            GameObject[] oldSections = new GameObject[queue.sectionsList.Length];
            for (int i = 0; i < queue.sectionsList.Length; i++)
            {
                oldSections[i] = queue.sectionsList[i];
            }

            //Sort all sections by distance
            float[] distances = new float[queue.sectionsList.Length];
            for (int i = 0; i < distances.Length; i++)
            {
                distances[i] = Vector3.Distance(Camera.main.transform.position, queue.sectionsList[i].transform.position);
            }
            Array.Sort(distances, queue.sectionsList);

            //Make the array of old to new indexes
            for (int i = 0; i < oldSections.Length; i++)
            {
                for (int k = 0; k < queue.sectionsList.Length; k++)
                {
                    if (oldSections[i] == queue.sectionsList[k])
                    {
                        queue.sectionIndices[i] = k;
                    }
                }
            }
        }

        base.OnInspectorGUI();
    }

    //Sort an individual row of sections by distance
    void SortWidth(Static_Queue queue, int startIndex)
    {
        float[] distances = new float[queue.width];
        GameObject[] objs = new GameObject[queue.width];

        for(int i = 0; i < distances.Length; i++)
        {
            distances[i] = Vector3.Distance(Camera.main.transform.position, queue.sectionsList[i + startIndex].transform.position);
            objs[i] = queue.sectionsList[i + startIndex];
        }

        Array.Sort(distances, objs);

        for(int i = 0; i < objs.Length; i++)
        {
            queue.sectionsList[i + startIndex] = objs[i];
        }
    }
}
