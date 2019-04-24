# Kthura
![](https://raw.githubusercontent.com/TrickyGameTools/KthuraTextEditor/master/Properties/Kthura.png)

Kthura is an Object based Map system for 2D games. Due to Kthura's object oriented approach, Kthura can be more complicated than a regular TileMap editor to understand, but in return Kthura can give you many tricks and treats in return a TileMap editor cannot offer. 

Kthura has been written in C#, and the classes you need to make it run in any .NET engine are all zlib licensed (only the editor, and the tools you can use to work with Kthura are GPL3).

By default Kthura uses its own native file format, but if you plan to create your own engine, and you don't wish to use a JCR6 based file format, you can export Kthura files into JSON, Lua and XML respectively. Alternatively (although both languages use the JSON syntax, so you may not fully need it), Kthura can even export into JavaScript file or a Python script.





# License notes:

The editor as an application has been licensed under the terms of the GPL3.
The classes set up to implement Kthura into your own programs have been licensed under the zlib license, so you can use these even in close sourced programs. 
The maps created with Kthura are merely considered as "the data" and is up to you to license any way you see fit!
