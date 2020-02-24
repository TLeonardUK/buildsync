```
___.         .__.__       .___                           
\_ |__  __ __|__|  |    __| _/_________.__. ____   ____  
 | __ \|  |  \  |  |   / __ |/  ___<   |  |/    \_/ ___\ 
 | \_\ \  |  /  |  |__/ /_/ |\___ \ \___  |   |  \  \___ 
 |___  /____/|__|____/\____ /____  >/ ____|___|  /\___  >
     \/                    \/    \/ \/         \/     \/ 
```

## About buildsync
Buildsync is a simple tool for distributing package builds, binaries and tool across internal LAN networks. It's primarily focused on the needs of game developers, and is active use on various projects.

The tool works similarly to peer-to-peer torrenting, with each peer both downloading parts of a build as well as uploading to other peers. The major change from torrenting is that the behaviour is controlled by a central server. Having this central server allows having a central registry of all available builds, central control over bandwidth limitations (to avoid network saturation), and user-level permissions.

It also supports scriptable support for installation and user-defined parametric launching of builds. This allows things like automatically installing console packages to devkits, or giving the user different options for launching a build - to different levels, in windowed mode, etc.

## Building
The source code comes as a visual studio 2019 solution. A few additional extensions are required to fully build install and help files.

+ IsWIX and its associated dependencies (Wix Extension, Windows Installed XML, etc) (https://github.com/iswix-llc/iswix-tutorials)
+ Sandcastle Help File Builder (https://ewsoftware.github.io/SHFB/html/8c0c97d0-c968-4c15-9fe9-e8f3a443c50a.htm)

## Basic Usage
Provided with the tool is a help file that you can access from the clients Help->View Help menu item. This gives a general description on setting it up and general usage.

## Speeds
The tool should be able to staurate a 1gbps connection if downloading/uploading from an SSD. Higher speeds are theoretical possible, but untested, and will probably run into other limits.

## Screenshots
<a href="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/downloads.png?raw=true"><img src="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/downloads.png?raw=true" width="150"></a>

<a href="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/launch_build.png?raw=true"><img src="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/launch_build.png?raw=true" width="150"></a>

<a href="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/build_manager.png?raw=true"><img src="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/build_manager.png?raw=true" width="150"></a>

<a href="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/server_manager.png?raw=true"><img src="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/server_manager.png?raw=true" width="150"></a>

<a href="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/users.png?raw=true"><img src="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/users.png?raw=true" width="150"></a>

## Contact Details
Any questions you are welcome to send me an email;

Tim Leonard
me@timleonard.uk

## Credits
Climax Studios for being the guinea pig for my tool.
