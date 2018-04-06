using Intel.RealSense;
using System.Collections.Generic;
using UnityEngine;

using OpenCVForUnity;

namespace OpenCVForUnity
{
    public class MeshDrawer : MonoBehaviour
    {
        [SerializeField] bool showMesh = true;
        [SerializeField] bool showBox = false;

        public void draw(List<Mat> corners, Points.Vertex[] vertices, ref Matrix4x4 transformationM, int width, float markerLength)
        {
            for (int i = 0; i < corners.Count; i++)
            {
                Mat points = corners[i];

                Vector3[] _verts = new Vector3[4];
                int[] triangles = { 0, 1, 2, 0, 2, 3 };

                for (int j = 0; j < points.total(); j++)
                {
                    //pixel coordinate (x, y) in (width, height)
                    double[] pointCoord = points.get(0, j);

                    Points.Vertex vert = vertices[width * (int)pointCoord[1] + (int)pointCoord[0]];

                    Vector4 camCoord = new Vector4(vert.x, vert.y * -1, vert.z, 1.0f);

                    camCoord = transformationM * camCoord;

                    Vector3 worldCoord = camCoord / camCoord.w;

                    _verts[j] = worldCoord;

                }

                Mesh mesh = new Mesh();
                mesh.vertices = _verts;
                mesh.triangles = triangles;
                mesh.RecalculateNormals();

                if (showMesh)
                {
                    MeshFilter meshFilter = GetComponent<MeshFilter>();
                    meshFilter.mesh = mesh;
                }


                if (showBox)
                {
                    GameObject coordObj = Resources.Load<GameObject>("Box");
                    coordObj.transform.localScale = Vector3.one * markerLength;
                    Vector3 norm = mesh.normals[0].normalized;
                    Vector3 center = mesh.bounds.center + (norm * markerLength / 2);
                    Quaternion q = Quaternion.FromToRotation(Vector3.up, norm);
					Instantiate(coordObj, center, q, this.gameObject.transform);
                }
            }
        }
    }
}

