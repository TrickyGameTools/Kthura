# A few things needed to do before you try anything

TeddyBear was written in Visual Studio 2017, version 15.9.9, for the .NET framework 4.6

Now the best approach is first to make sure to create a folder dedicated to my work. I myself used a portable hard drive. For easiness I'll pretend you use device T:\
Make sure PowerShell is configured to run ps1 scripts (which by default it is not).

Next make sure that MonoGame has been installed, since the editor heavily relies on it.

Then go to PowerShell and type the following

~~~PowerShell
# First to my needed folder:
T:
mkdir TrickyProjects
cd TrickyProjects

# Now to make sure all dependencies are installed
git clone https://github.com/Tricky1975/trickyunits_csharp.git TrickyUnits
git clone https://github.com/jcr6/JCR6_CSharp.git JCR6
git clone https://github.com/TrickyGameTools/TQMG
git clone https://github.com/TrickyGameTools/Kthura --recurse-submodules

# Let's get into the Kthura directory
cd Kthura/Source
~~~

Now this repository does contain links to, but not the package itself. NLua (and its dependency KeraLua). It has been used in the editor.
Now I did it the easy way, with opening T:\TrickyProjects\Kthura\Source\KthuraEdit\KthuraEdit.sln in Visual Studio and I used NuGet through VS to import it in, but I guess there are also other ways to do the trick.

When everything is in order
~~~PowerShell
# Make sure you are in T:\TrickyUnits\Kthura\Source
KthBuild
# This should, if everything has properly been set up, compile everything and put it all in T:\TrickyProjects\Kthura\Releases.
# If you do the following the Kthura launcher should start up
cd ../Releases
Kthura
~~~
