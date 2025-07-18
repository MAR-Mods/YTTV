# YTTV - A Lethal Company Mod
This mod allows you to play YouTube videos on the TV, by specifying a link. You can enter various commands for the mod into chat.

A fork of TheByteNinja's [Television Controller Fix](https://thunderstore.io/c/lethal-company/p/TheByteNinja/Television_Controller_Fix/), which itself is a fork of KoderTech's [Television Controller](https://thunderstore.io/c/lethal-company/p/KoderTeh/Television_Controller/) mod. Both of these mods are outdated, so I took it upon myself to make a working version.

`/treset` seems to help when the mod becomes unresponsive to commands.

Supports English and Russian, just as the original did. You can set the language in a config file (`television_controller.cfg`).

## Commands
<ul>
<li>/thelp - View all commands</li>
<li>/tplay [LINK] - Play video from this link</li>
<li>/ttime [TIMESTAMP] - Set video to this timestamp</li>
<li>/treset - Restart video</li>
<li>/tposition [TRUE/FALSE] - Toggle if video resets on TV on/off</li>
<li>/tvolume [0-100] - Set TV volume to this percentage</li>
</ul>

## How did you do it?
I used Visual Studio. For the project, I picked Class Library (.NET Framework), and set the framework to .NET Framework 4.8. Essentially all I really did (besides fixing TheByteNinja's decompiled code) was change the link that downloads yt-dlp.exe to be the latest version (in the original mods, these are outdated, hence why they don't work anymore).
<br> <br>
This is the relevant line of code: `DownloadFiles(new Uri("https://github.com/yt-dlp/yt-dlp/releases/download/2025.06.30/yt-dlp.exe"), "YTTV\\other\\yt-dlp.exe");`
<br> <br>
Here is the link to the [GitHub repository with the source code](https://github.com/MAR-Mods/YTTV).

## Credits
<em> Television Controller </em>
<ul>
<li> KoderTech (listed on Thunderstore as KoderTeh) </li>
<li> 7-8 Arctiqan </li>
<li> Iluminati </li>
<li> Dan4ik </li>
<li> Durnanu </li>
<li> Larte </li>
</ul>
<em> Television Controller Fix </em>
<ul>
<li> TheByteNinja </li>
</ul>
