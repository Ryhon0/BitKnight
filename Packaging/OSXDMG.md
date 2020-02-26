# .dmg installer for MacOS

Open Disk Utility

![](https://raw.githubusercontent.com/Ryhon0/BitKnight/master/Packaging/OSX%20Screenshots/1.png)
Create a new blank image
![](https://raw.githubusercontent.com/Ryhon0/BitKnight/master/Packaging/OSX%20Screenshots/2.png)
Name the image BitKnight Installer
![](https://raw.githubusercontent.com/Ryhon0/BitKnight/master/Packaging/OSX%20Screenshots/3.png)
Mount the image and open folder view options
![](https://raw.githubusercontent.com/Ryhon0/BitKnight/master/Packaging/OSX%20Screenshots/4.png)
Show hidden files (Command+Shift+Dot)
Copy the files from Packaging/Packages/OSX
Set the background image to .background.png

If you exported the game on another OS it will create a .zip file instead of a .app folder. Make sure to export the .zip into a folder ending with .app

![](https://raw.githubusercontent.com/Ryhon0/BitKnight/master/Packaging/OSX%20Screenshots/5.png)

### Create a symlink to /Applications
Open a terminal and enter these commands:
`cd /Volumes/BitKnight\ Installer`
`ln -s /Applications Applications`

Change the icon size to 128x128 and adjust the file placement
![](https://raw.githubusercontent.com/Ryhon0/BitKnight/master/Packaging/OSX%20Screenshots/6.png)

### Compress the image
Open a terminal and change the directory with the .dmg file
Make sure you've unmounted the image before entering this command:
`hdiutil convert BitKnight\ Installer.dmg -format UDBZ -o BitKnight.dmg` 