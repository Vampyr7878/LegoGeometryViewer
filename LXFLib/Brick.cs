using GXLib;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace LXFLib
{
    /// <summary>
    /// Brick data.
    /// </summary>
    public class Brick
    {
        /// <summary>
        /// Path to the extracted db.lif.
        /// Change this value to the path where you extracted your db.lif,
        /// choose a \Primites\LOD0\ folder inside.
        /// </summary>
        private string path = @"C:\Users\wojte\AppData\Roaming\LEGO Company\LEGO Digital Designer\db\Primitives\LOD0\";

        /// <summary>
        /// Part number.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of all the subparts.
        /// </summary>
        public List<GX> Models { get; set; }

        /// <summary>
        /// Transformation matrix.
        /// </summary>
        public float[] Transformation { get; set; }

        /// <summary>
        /// Part's bones.
        /// </summary>
        public List<string> Bones { get; set; }

        /// <summary>
        /// Bone of the parent part.
        /// </summary>
        public string Parent { get; set; }
        
        /// <summary>
        /// Load empty brick data.
        /// </summary>
        public Brick()
        {
            Name = "";
            Parent = "-1";
            Models = new List<GX>();
        }

        /// <summary>
        /// Load a brick data.
        /// </summary>
        /// <param name="name">Part's number.</param>
        /// <param name="materials">List of the part materials.</param>
        public Brick(string name, string materials)
        {
            Name = name;
            Parent = "-1";
            Models = new List<GX>();
            Models.Add(new GX(path + name + ".g"));
            int i = 1;
            while (File.Exists(path + name + ".g" + i))
            {
                Models.Add(new GX(path + name + ".g" + i));
                i++;
            }
            string[] m = materials.Split(',');
            for (i = 0; i < Models.Count; i++)
            {
                Models[i].Material = int.Parse(m[i]);
                Models[i].Material = Models[i].Material == 0 ? Models[i - 1].Material : Models[i].Material;
            }
        }
    }
}
