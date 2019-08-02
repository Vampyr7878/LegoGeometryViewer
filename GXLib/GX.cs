using System.IO;
using System.Numerics;

namespace GXLib
{
    /// <summary>
    /// Subpart geometry data.
    /// </summary>
    public class GX
    {
        /// <summary>
        /// Coordinates of vertices.
        /// </summary>
        public Vector3[] Vertices { get; set; }

        /// <summary>
        /// Coordinates of normals.
        /// </summary>
        public Vector3[] Normals { get; set; }

        /// <summary>
        /// Coordinates for appling a decoration.
        /// It's not used in the exporter.
        /// </summary>
        public Vector2[] Textures { get; set; }

        /// <summary>
        /// Triangle indices.
        /// </summary>
        public int[] Indices { get; set; }

        /// <summary>
        /// Material assigned for this subpart.
        /// </summary>
        public int Material { get; set; }

        /// <summary>
        /// Load subpart.
        /// </summary>
        /// <param name="name">Subpart file.</param>
        public GX(string name)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(name, FileMode.Open)))
            {
                int vertex;
                int index;
                int flags;
                reader.ReadInt32();
                vertex = reader.ReadInt32();
                index = reader.ReadInt32();
                flags = reader.ReadInt32();
                // Load verticees.
                Vertices = new Vector3[vertex];
                for (int i = 0; i < vertex; i++)
                {
                    Vertices[i] = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                }
                // Load normals.
                Normals = new Vector3[vertex];
                for (int i = 0; i < vertex; i++)
                {
                    Normals[i] = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                }
                // If file contains texture coordinates then load them.
                if ((flags & 1) != 0)
                {
                    Textures = new Vector2[vertex];
                    for (int i = 0; i < vertex; i++)
                    {
                        Textures[i] = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                    }
                }
                // Load triangle indices.
                Indices = new int[index];
                for (int i = 0; i < index; i++)
                {
                    Indices[i] = reader.ReadInt32();
                }
            }
        }
    }
}
