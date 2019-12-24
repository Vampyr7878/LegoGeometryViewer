using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace LXFLib
{
    /// <summary>
    /// Main Class for the Lego Model.
    /// </summary>
    public class LXF
    {
        /// <summary>
        /// List of all the bricks in the model.
        /// </summary>
        public List<Brick> Bricks { get; set; }

        /// <summary>
        /// Load the model from specified path.
        /// </summary>
        /// <param name="path">Path to the model.</param>
        public LXF(string path)
        {
            Bricks = new List<Brick>();
            string name = "";
            using (StreamReader reader = new StreamReader(path))
            {
                using (ZipArchive zip = new ZipArchive(reader.BaseStream, ZipArchiveMode.Read))
                {
                    foreach (ZipArchiveEntry entry in zip.Entries)
                    {
                        // Extract the LXFML file that contains actual model.
                        if (entry.Name.ToLower().Contains("lxfml"))
                        {
                            name = entry.Name;
                            entry.ExtractToFile(name);
                        }
                    }
                }
            }
            try
            {
                LXFML file;
                XmlSerializer serializer = new XmlSerializer(typeof(LXFML));
                using (StreamReader reader = new StreamReader(name))
                {
                    file = (LXFML)serializer.Deserialize(reader);
                }
                Brick part;
                Brick p = new Brick();
                string[] transform;
                float[] values;
                // Load all the bricks and parts.
                foreach (LXFMLBricksBrick brick in file.Bricks[0].Brick)
                {
                    for (int j = 0; j < brick.Part.Length; j++)
                    {
                        part = new Brick(brick.Part[j].designID, brick.Part[j].materials);
                        transform = brick.Part[j].Bone[brick.Part[j].Bone.Length - 1].transformation.Split(',');
                        values = new float[transform.Length];
                        CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                        culture.NumberFormat.CurrencyDecimalSeparator = ".";
                        for (int i = 0; i < values.Length; i++)
                        {
                            values[i] = float.Parse(transform[i], NumberStyles.Any, culture);
                        }
                        part.Transformation = new float[]{ values[0],  values[1],  values[2], 0f,
                                                   values[3],  values[4],  values[5], 0f,
                                                   values[6],  values[7],  values[8], 0f,
                                                   values[9], values[10], values[11], 1f };
                        part.Bones = new List<string>();
                        foreach (LXFMLBricksBrickPartBone bone in brick.Part[j].Bone)
                        {
                            part.Bones.Add(bone.refID);
                        }
                        Bricks.Add(part);
                    }
                }
                string boneString;
                string[] bones;
                string[] others;
                // Recreate the model structure by assigning part parents.
                foreach (LXFMLRigidSystemsRigidSystemJoint joint in file.RigidSystems[0].Joint)
                {
                    boneString = file.RigidSystems[0].Rigid[int.Parse(joint.RigidRef[0].rigidRef)].boneRefs;
                    bones = boneString.Split(',');
                    others = file.RigidSystems[0].Rigid[int.Parse(joint.RigidRef[1].rigidRef)].boneRefs.Split(',');
                    foreach (string bone in bones)
                    {
                        int index = -1;
                        int parent = -1;
                        for (int j = 0; j < Bricks.Count; j++)
                        {
                            if (index == -1)
                            {
                                index = Bricks[j].Bones.FindIndex(x => x == bone) == -1 ? -1 : j;
                            }
                            if (parent == -1)
                            {
                                parent = Bricks[j].Bones.FindIndex(x => x == others[0]) == -1 ? -1 : j;
                            }
                        }
                        if (Bricks[index].Parent == "-1")
                        {
                            Bricks[index].Parent = Bricks[parent].Bones[0];
                        }
                    }
                    for (int i = 1; i < others.Length; i++)
                    {
                        int index = -1;
                        int parent = -1;
                        for (int j = 0; j < Bricks.Count; j++)
                        {
                            if (index == -1)
                            {
                                index = Bricks[j].Bones.FindIndex(x => x == others[i]) == -1 ? -1 : j;
                            }
                            if (parent == -1)
                            {
                                parent = Bricks[j].Bones.FindIndex(x => x == others[0]) == -1 ? -1 : j;
                            }
                        }
                        if (Bricks[index].Parent == "-1")
                        {
                            Bricks[index].Parent = Bricks[parent].Bones[0];
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "There was an error");
            }
            finally
            {
                // Delete extracted file when you're done.
                File.Delete(name);
            }
        }
    }
}
