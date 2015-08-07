# OPC Demo
This is an project to read data from PLC with java and C#
We got a middleware named Kepware that support PLC hardware.
To use its library, we package the library dlls as COM component since Kepware is based on C#
Then we use 3-rd java library (JACOB) to use COM component in java project.
Technology Architecture:
        Kepware Library + C#  + COM + JACOB + Java
