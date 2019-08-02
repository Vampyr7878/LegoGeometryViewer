using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Windows.Forms;
using LXFLib;
using GXLib;
using OBLExporter;

namespace LegoGeometryViewer
{
    /// <summary>
    /// The main form class.
    /// </summary>
    public partial class SharpGLForm : Form
    {
        /// <summary>
        /// The current rotation.
        /// </summary>
        private float rotation = 0.0f;

        /// <summary>
        /// Lego model.
        /// </summary>
        LXF model;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharpGLForm"/> class.
        /// </summary>
        public SharpGLForm()
        {
            InitializeComponent();
            //Change this path to your Lego Models folder;
            openFileDialog.InitialDirectory = @"C:\Users\wojte\Documents\LEGO Creations\Models";
        }

        /// <summary>
        /// Open Lego model.
        /// </summary>
        /// <param name="name"></param>
        private void OpenFile(string name)
        {
            model = new LXF(name);
        }

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RenderEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, RenderEventArgs e)
        {
            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  Clear the color and depth buffer.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            //  Load the identity matrix.
            gl.LoadIdentity();

            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.Enable(OpenGL.GL_DEPTH_BUFFER);

            //  Rotate around the Y axis.
            gl.Rotate(rotation, 0.0f, 1.0f, 0.0f);
            //  Draw a coloured pyramid.
            if (model != null)
            {
                foreach (Brick brick in model.Bricks)
                {
                    gl.PushMatrix();
                    gl.MultMatrix(brick.Transformation);
                    gl.Begin(OpenGL.GL_TRIANGLES);
                    foreach (GX part in brick.Models)
                    {
                        foreach (int index in part.Indices)
                        {
                            gl.Vertex(part.Vertices[index].X, part.Vertices[index].Y, part.Vertices[index].Z);
                            gl.Normal(part.Normals[index].X, part.Normals[index].Y, part.Normals[index].Z);
                        }
                    }
                    gl.End();
                    gl.PopMatrix();
                }
            }

            //  Nudge the rotation.
            rotation += 3.0f;
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, EventArgs e)
        {
            //  TODO: Initialise OpenGL here.

            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  Set the clear color.
            gl.ClearColor(0, 0, 0, 0);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, EventArgs e)
        {
            //  TODO: Set the projection matrix here.

            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            //  Load the identity.
            gl.LoadIdentity();

            //  Create a perspective transformation.
            gl.Perspective(60.0f, (double)Width / (double)Height, 0.01, 100.0);

            //  Use the 'look at' helper function to position and aim the camera.
            gl.LookAt(-30, 30, -30, 0, 0, 0, 0, 1, 0);

            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        /// <summary>
        /// Open model file.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void openButton_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != "")
            {
                OpenFile(openFileDialog.FileName);
            }
        }

        /// <summary>
        /// Export loaded model.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void exportButton_Click(object sender, EventArgs e)
        {
            if (model != null)
            {
                OBLExporter.OBLExporter.Export(model, openFileDialog.FileName.Replace(".lxf", ".obl"));
                MessageBox.Show("Done");
            }
        }
    }
}
