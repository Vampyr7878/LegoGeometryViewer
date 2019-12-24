# LegoGeometryViewer
Tools and libraries to view an export Lego Digial Designer models.

This solution inclues 3 dll libarires:

* GXLib - handles loading individual g and gx files.
* LXFLib - handles loading LXF model files.
* OBLExporter - handles exporing loaded model into a obl file.

There are 2 appliactions:

* LegoGeometryViewer - uses SharpGL to render opened model as a prview and allows exporting it.
* LegoGeometryExporter - console appliction that can be used to export multiple lxf files.

It's made in Visual Studio 2013 because SharpGL project templates are only available up to this version. It might not compile in newer one.

In order for all of this to work you need to extract db.lif file in your %AppData%\LEGO Company\LEGO Digital Designer\ folder. In order to do that you can use LIF Extractor tool: https://github.com/JrMasterModelBuilder/LIF-Extractor/releases

You also need to change the path in the Brick.cs to your exporterd folder + \Primiteves\LOD0\. This file is inside LXFLib project.

You might also want to change InitialDirectory for openFileDialot inside SharpGLForm.cs from LegoGeometryViewer. This step is not necessary but it will be easier for you to open individual models.

OBL is modified OBJ format that I made in order to store some additional information about the Lego pieces i the model.
