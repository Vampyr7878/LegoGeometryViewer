using System;
using LXFLib;

namespace LegoGeometryExporter
{
    class Program
    {
        /// <summary>
        /// Console based exporter, you can give it one or more files via command line,
        /// each file should be seperated by space.
        /// </summary>
        /// <param name="args">Command line arguments, here the files.</param>
        static void Main(string[] args)
        {
            // If no files are given then don't do anything.
            if (args.Length == 0)
            {
                Console.WriteLine("No file");
                return;
            }
            // Extract each lxf file into obl file.
            for (int i = 0; i < args.Length; i++)
            {
                if(!args[i].EndsWith(".lxf"))
                {
                    continue;
                }
                LXF model = new LXF(args[i]);
                OBLExporter.OBLExporter.Export(model, args[i].Replace(".lxf", ".obl"));
                Console.WriteLine(args[i] + " done");
            }
        }
    }
}
