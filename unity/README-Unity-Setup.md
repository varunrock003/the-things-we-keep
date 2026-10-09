# Unity setup

This folder is where the Unity project will go. Right now there is only this note, so the folder doesn't disappear from git.

We are using Unity 6.3 LTS (6000.3.25f1) with the 3D (URP) template, same setup we used in Lab 02, so nothing new to install.

Steps I (Varunteja) followed for the project setup:

1. In Unity Hub, create the project inside this `unity/` folder. If Hub makes a subfolder with the project name, that's fine, that subfolder is the project.
2. Open the default scene once, let it finish importing, save, and close Unity before committing. Committing while Unity is still importing makes a mess of the Library changes.
3. Check `git status` before committing. `Library/`, `Temp/` and the rest are ignored from the root `.gitignore`, so the commit should only have `Assets/`, `Packages/` and `ProjectSettings/`.

When the project is in here, this note can be deleted.
