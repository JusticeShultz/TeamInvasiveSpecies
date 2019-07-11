using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

//I got lazy with the name and said fuck it.

/// <summary>
/// This script is a simple combine mesh tool to simply save on batching in Unity.
/// </summary>
public class JusticesMagicMeshCombiner : MonoBehaviour
{
    [MenuItem("Justice/MagicBatchButton")]
    static void MagicButton()
    {
        Transform[] selection = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);

        if (selection.Length == 0)
        {
            EditorUtility.DisplayDialog("No source object selected!", "Please select two or more target objects", "uwu");
            return;
        }

        if (selection.Length < 2)
        {
            EditorUtility.DisplayDialog("Not enough objects selected...", "Please select two or more target objects", "uwu");
            return;
        }

        /*Mesh newMesh = new Mesh();

        foreach (Transform trans in selection)
        {
            MeshFilter rendererObj = trans.gameObject.GetComponent<MeshFilter>();

            if (rendererObj)
            {
                List<Vector3> newVerts = new List<Vector3>();

                //Get worldspace vertices.
                for (int i = 0; i < rendererObj.sharedMesh.vertices.Length; i++)
                    newVerts.Add(rendererObj.sharedMesh.vertices[i] + rendererObj.transform.position);

                //Concat the vertices together.
                newMesh.vertices = newMesh.vertices.Concat(newVerts.ToArray()).ToArray();

                List<int> triangles = new List<int>();

                for(int i = 0; i < rendererObj.sharedMesh.triangles.Length; i++)
                {
                    triangles.Add(rendererObj.sharedMesh.triangles[i]); //+ //newMesh.triangles.Length);
                }

                //Concat the triangles together.
                newMesh.triangles = newMesh.triangles.Concat(triangles.ToArray()).ToArray();
            }
        }*/

        Material material = null;
        List<MeshFilter> meshFiltersList = new List<MeshFilter>();

        foreach (Transform trans in selection)
        {
            MeshFilter rendererObj = trans.gameObject.GetComponent<MeshFilter>();

            if (rendererObj)
                meshFiltersList.Add(rendererObj);
        }

        MeshFilter[] meshFilters = meshFiltersList.ToArray();

        // figure out array sizes
        int vertCount = 0;
        int normCount = 0;
        int triCount = 0;
        int uvCount = 0;
        

        foreach (MeshFilter mf in meshFilters)
        {
            vertCount += mf.sharedMesh.vertices.Length;
            normCount += mf.sharedMesh.normals.Length;
            triCount += mf.sharedMesh.triangles.Length;
            uvCount += mf.sharedMesh.uv.Length;
            if (material == null)
                material = mf.gameObject.GetComponent<Renderer>().material;
        }

        // allocate arrays
        Vector3[] verts = new Vector3[vertCount];
        Vector3[] norms = new Vector3[normCount];
        Transform[] aBones = new Transform[meshFilters.Length];
        Matrix4x4[] bindPoses = new Matrix4x4[meshFilters.Length];
        BoneWeight[] weights = new BoneWeight[vertCount];
        int[] tris = new int[triCount];
        Vector2[] uvs = new Vector2[uvCount];

        int vertOffset = 0;
        int normOffset = 0;
        int triOffset = 0;
        int uvOffset = 0;
        int meshOffset = 0;

        // merge the meshes and set up bones
        foreach (MeshFilter mf in meshFilters)
        {
            foreach (int i in mf.sharedMesh.triangles)
                tris[triOffset++] = i + vertOffset;

            aBones[meshOffset] = mf.transform;
            bindPoses[meshOffset] = Matrix4x4.identity;

            foreach (Vector3 v in mf.sharedMesh.vertices)
            {
                weights[vertOffset].weight0 = 1.0f;
                weights[vertOffset].boneIndex0 = meshOffset;
                verts[vertOffset++] = mf.gameObject.transform.TransformPoint(v);
            }

            foreach (Vector3 n in mf.sharedMesh.normals)
                norms[normOffset++] = mf.gameObject.transform.TransformDirection(n);

            foreach (Vector2 uv in mf.sharedMesh.uv)
                uvs[uvOffset++] = uv;

            meshOffset++;

            /*MeshRenderer mr =
              mf.gameObject.GetComponent(typeof(MeshRenderer))
              as MeshRenderer;

            if (mr)
                mr.enabled = false;*/
        }

        // hook up the mesh
        Mesh me = new Mesh();
        me.name = "MagicMesh";
        me.vertices = verts;
        me.normals = norms;
        me.boneWeights = weights;
        me.uv = uvs;
        me.triangles = tris;
        me.bindposes = bindPoses;
        me.RecalculateNormals();
        me.RecalculateTangents();
        me.RecalculateBounds();

        string filePath = EditorUtility.SaveFilePanelInProject("Save Procedural Mesh", "MagicMesh", "asset", "");

        if (filePath == "")
        {
            EditorUtility.DisplayDialog("Invalid file path!", "Please select a valid file path and try again...", "uwu");
            return;
        }

        AssetDatabase.CreateAsset(me, filePath);
    }
}