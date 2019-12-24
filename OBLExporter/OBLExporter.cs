using GXLib;
using LXFLib;
using System.IO;
using System.Numerics;

namespace OBLExporter
{
    /// <summary>
    /// Exporter class.
    /// </summary>
    public class OBLExporter
    {
        /// <summary>
        /// Export the model.
        /// </summary>
        /// <param name="model">Model to be exported.</param>
        /// <param name="file">Output file.</param>
        public static void Export(LXF model, string file)
        {
            // The output file is a slightly modified obj file to allow for lego materials
            // and model structure so you know which parts are attached to each other.
            using (StreamWriter writer = new StreamWriter(file))
            {
                int a, b, c;
                // File name.
                writer.WriteLine("# " + Path.GetFileName(file));
                foreach (Brick brick in model.Bricks)
                {
                    int i = 0;
                    // Make a group for each part.
                    foreach (GX part in brick.Models)
                    {
                        writer.WriteLine();
                        // Name of the group is the part number plus a subpart number.
                        writer.WriteLine("g " + brick.Name + "x" + i);
                        writer.WriteLine();
                        // Make bone for the piece, used to attach them.
                        writer.WriteLine("b " + brick.Bones[0]);
                        writer.WriteLine();
                        if (i == 0)
                        {
                            // If it's the main subpart then include a parent bone.
                            writer.WriteLine("p " + brick.Parent);
                            writer.WriteLine();
                            // If it's the main subpart then include transformation matrix.
                            writer.Write("t");
                            for (int j = 0; j < 16; j++)
                            {
                                writer.Write(" " + brick.Transformation[j]);
                            }
                            writer.WriteLine();
                            writer.WriteLine();
                        }
                        // Index of a Lego Material.
                        writer.WriteLine("m " + part.Material);
                        writer.WriteLine();
                        // Number of vertices and their coordinates.
                        writer.WriteLine(part.Vertices.Length);
                        foreach (Vector3 v in part.Vertices)
                        {
                            writer.WriteLine("v " + v.X + " " + v.Y + " " + v.Z);
                        }
                        // Number of normals and their coordinates.
                        writer.WriteLine();
                        writer.WriteLine(part.Normals.Length);
                        foreach (Vector3 v in part.Normals)
                        {
                            writer.WriteLine("vn " + v.X + " " + v.Y + " " + v.Z);
                        }
                        // Number of vertex indices and their order for each face.
                        writer.WriteLine();
                        writer.WriteLine(part.Indices.Length);
                        for (int j = 0; j < part.Indices.Length; j++)
                        {
                            a = part.Indices[j++];
                            b = part.Indices[j++];
                            c = part.Indices[j];
                            writer.WriteLine("f " + a + " " + b + " " + c);
                        }
                        i++;
                    }
                }
            }
        }

        /// <summary>
        /// Export the model.
        /// </summary>
        /// <param name="model">Model to be exported.</param>
        /// <param name="file">Output file.</param>
        public static void ExportBinary(LXF model, string file)
        {
            // The output file is a slightly modified obj file to allow for lego materials
            // and model structure so you know which parts are attached to each other.
            using (BinaryWriter writer = new BinaryWriter(File.Create(file)))
            {
                // File name.
                writer.Write(Path.GetFileName(file));
                foreach (Brick brick in model.Bricks)
                {
                    int i = 0;
                    // Make a group for each part.
                    foreach (GX part in brick.Models)
                    {
                        // Name of the group is the part number plus a subpart number.
                        writer.Write(brick.Name + "x" + i);
                        // Make bone for the piece, used to attach them.
                        writer.Write(short.Parse(brick.Bones[0]));
                        if (i == 0)
                        {
                            // If it's the main subpart then include a parent bone.
                            writer.Write(short.Parse(brick.Parent));
                            // If it's the main subpart then include transformation matrix.
                            for (int j = 0; j < 16; j++)
                            {
                                writer.Write(brick.Transformation[j]);
                            }
                        }
                        // Index of a Lego Material.
                        writer.Write(part.Material);
                        // Number of vertices and their coordinates.
                        writer.Write(part.Vertices.Length);
                        foreach (Vector3 v in part.Vertices)
                        {
                            writer.Write(v.X);
                            writer.Write(v.Y);
                            writer.Write(v.Z);
                        }
                        // Number of normals and their coordinates.
                        writer.Write(part.Normals.Length);
                        foreach (Vector3 v in part.Normals)
                        {
                            writer.Write(v.X);
                            writer.Write(v.Y);
                            writer.Write(v.Z);
                        }
                        // Number of vertex indices and their order for each face.
                        writer.Write(part.Indices.Length);
                        for (int j = 0; j < part.Indices.Length; j++)
                        {
                            writer.Write((short)part.Indices[j++]);
                            writer.Write((short)part.Indices[j++]);
                            writer.Write((short)part.Indices[j]);
                        }
                        i++;
                    }
                }
            }
        }
    }
}
