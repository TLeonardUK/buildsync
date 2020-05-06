```
___.         .__.__       .___                           
\_ |__  __ __|__|  |    __| _/_________.__. ____   ____  
 | __ \|  |  \  |  |   / __ |/  ___<   |  |/    \_/ ___\ 
 | \_\ \  |  /  |  |__/ /_/ |\___ \ \___  |   |  \  \___ 
 |___  /____/|__|____/\____ /____  >/ ____|___|  /\___  >
     \/                    \/    \/ \/         \/     \/ 
```

## About buildsync
Buildsync is a simple tool for distributing package builds, binaries and tool across internal LAN networks. It's primarily focused on the needs of game developers, and is active use on various large projects.

In principle it works similarily to peer-to-peer torrenting but with the benefit of having a central server which can maintain a build registry and permit the coordination of admin tasks (modifying network routing, user permissions and so forth). It also supports C# scriptable actions for each build, to configure installation, launching and so forth.

## Features
* Centralized server model allowing global build registry, access control and network management.
* Distributed P2P download model to increased maximum throughput across your network and reduce bandwith saturation of any specific peers.
* Granular usergroup based permissions to restrict access to builds, eg. by project or platform.
* C# Scriptable installation of builds. Permits doing things such as auto-installing builds to devkits, setting up symlinks, decompressing files, apply patches, etc.
* C# Scriptable launching of builds. Allows the user to be given launch options (boot to specific maps, to specific kits, etc), as well as configuring how the build is launched (from a devkit, on the local machine, etc).
* Automatic distribution of updates.
* Automatic updating and installation of builds as they become available, or ability to install indiividual builds.
* Flexible tagging support used to control which builds to download and which to exclude. Allows tagging builds for useful states such as "Latest-QA, Broken, etc".
* Automatic management of storage space. Set the maximum amount of space you can afford to use for builds, along with what builds you would prefer to prioritize keeping (eg. builds tagged as archived) or deleting (eg. builds tagged as broken). After that the tool will dynamically manage builds to try to keep within space limits.
* Dynamic management of network routing. Permits the explicit definitions of what client tags can communicate with each other, and what bandwidth limits are enforced. Using this you can do things like segregating offices, setting up spatial proxies, setting up replica servers, changing to a central download model rather than P2P, etc.
* Validation of builds (using hardware accelerated hashing and checksuming) to ensure transfers were intact.
* Re-use of files from previous builds to reduce download sizes.
* Transport level SNAPPY compression to reduce data transfered.
* Source control integration (currently for Perforce and GIT). Can be used to filter which builds are downloaded (eg. only downloading builds at or below the current head revision of a workspace).
* Automatic mass replication of builds. Allowing machines to be setup to act as replicas or proxies. 

## Building
The source code comes as a visual studio 2019 solution. A few additional extensions are required to fully build install and help files.

+ IsWIX and its associated dependencies (Wix Extension, Windows Installed XML, etc) (https://github.com/iswix-llc/iswix-tutorials)
+ Sandcastle Help File Builder (https://ewsoftware.github.io/SHFB/html/8c0c97d0-c968-4c15-9fe9-e8f3a443c50a.htm)

## Basic Usage
Provided with the tool is a help file that you can access from the clients Help->View Help menu item. This gives a general description on setting it up and general usage.

## Screenshots
<html>
<tr>
<td><a href="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/downloads.png?raw=true"><img src="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/downloads.png?raw=true" width="350"></a></td>
<td><a href="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/launch_build.png?raw=true"><img src="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/launch_build.png?raw=true" width="350"></a></td>
 </tr>
<tr>
<td><a href="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/build_manager.png?raw=true"><img src="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/build_manager.png?raw=true" width="350"></a></td>
<td><a href="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/server_manager.png?raw=true"><img src="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/server_manager.png?raw=true" width="350"></a></td>
 </tr>
 <tr>
<td><a href="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/users.png?raw=true"><img src="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/users.png?raw=true" width="350"></a></td>
<td><a href="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/manifests.png?raw=true"><img src="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/manifests.png?raw=true" width="350"></a></td>
 </tr>
 <tr>
<td><a href="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/routes.png?raw=true"><img src="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/routes.png?raw=true" width="350"></a></td>
<td><a href="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/statistics.png?raw=true"><img src="https://github.com/TLeonardUK/buildsync/blob/master/Resources/GitHub/statistics.png?raw=true" width="350"></a></td>
 </tr>
</table>

## FAQ
TODO

## Contact Details
Any questions you are welcome to send me an email;

Tim Leonard
me@timleonard.uk

## Credits
Climax Studios for being the guinea pig for my tool.
