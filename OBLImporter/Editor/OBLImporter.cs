using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

[ScriptedImporter(1, "obl")]
public class OBLImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        GameObject temp = new GameObject();
        GameObject gameObject;
        GameObject mesh;
        GameObject brick = temp;
        List<GameObject> meshes = new List<GameObject>();
        List<GameObject> parts = new List<GameObject>();
        List<Mesh> bricks = new List<Mesh>();
        List<string> names = new List<string>();
        List<Material> materials = new List<Material>();
        List<short> files = new List<short>();
        List<short> parents = new List<short>();
        string name;
        short material;
        Vector3[] vertices;
        Vector3[] normals;
        int[] indices;
        Vector2[] uv;
        Matrix4x4 matrix;
        using (BinaryReader reader = new BinaryReader(File.Open(ctx.assetPath, FileMode.Open)))
        {
            name = reader.ReadString();
            gameObject = new GameObject(name.Replace(".obl", ""));
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                name = reader.ReadString();
                mesh = new GameObject(name + "_" + reader.ReadInt16());
                if (name.EndsWith("x0"))
                {
                    parents.Add(reader.ReadInt16());
                    parts.Add(mesh);
                    mesh.transform.parent = gameObject.transform;
                    matrix = new Matrix4x4(new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                                           new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                                           new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                                           new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
                    mesh.transform.position = matrix.GetColumn(3);
                    mesh.transform.rotation = matrix.rotation;
                    brick = mesh;
                }
                else
                {
                    mesh.transform.parent = brick.transform;
                    mesh.transform.position = brick.transform.position;
                    mesh.transform.rotation = brick.transform.rotation;
                }
                material = reader.ReadInt16();
                if (!files.Contains(material))
                {
                    Material mat = Resources.Load<Material>("Materials/" + material);
                    materials.Add(mat);
                    files.Add(material);
                }
                vertices = new Vector3[reader.ReadInt32()];
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                }
                normals = new Vector3[reader.ReadInt32()];
                for (int i = 0; i < normals.Length; i++)
                {
                    normals[i] = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                }
                indices = new int[reader.ReadInt32()];
                for (int i = 0; i < indices.Length; i++)
                {
                    indices[i] = reader.ReadInt16();
                }
                uv = new Vector2[vertices.Length];
                for (int i = 0; i < uv.Length; i++)
                {
                    Vector3 v = vertices[i];
                    uv[i] = new Vector2((v.x + v.y) / 2, (v.y + v.z) / 2);
                }
                if (!names.Contains(name))
                {
                    Mesh m = new Mesh
                    {
                        vertices = vertices,
                        normals = normals,
                        triangles = indices,
                        uv = uv
                    };
                    bricks.Add(m);
                    names.Add(name);
                }
                mesh.AddComponent<MeshFilter>();
                mesh.GetComponent<MeshFilter>().mesh = bricks[names.IndexOf(name)];
                mesh.AddComponent<MeshRenderer>();
                mesh.GetComponent<MeshRenderer>().materials = new Material[] { materials[files.IndexOf(material)] };
                meshes.Add(mesh);
            }
        }
        for (int i = 0; i < parts.Count; i++)
        {
            int index = parts.FindIndex(x => x.name.EndsWith(parents[i].ToString()));
            if (parents[i] != -1)
            {
                parts[i].transform.parent = parts[index].transform;
            }
        }
        gameObject.transform.localScale = new Vector3(0.5f, 0.5f, -0.5f);
        ctx.AddObjectToAsset(gameObject.name, gameObject);
        ctx.SetMainObject(gameObject);
        for (int i = 0; i < bricks.Count; i++)
        {
            ctx.AddObjectToAsset(names[i], bricks[i]);
        }
        for (int i = 0; i < materials.Count; i++)
        {
            ctx.AddObjectToAsset(files[i].ToString(), materials[i]);
        }
        DestroyImmediate(temp);
    }
}
