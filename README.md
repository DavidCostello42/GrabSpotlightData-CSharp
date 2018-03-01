# GrabSpotlightData-CSharp

This is a simple C# Application that will retrieve content from the Microsoft Spotlight profile directory for the currently logged in Windows User and copy all content to the currently logged in user's Pictures directory.

Content is copied to Pictures\Spotlight and broken down into some relevant sub-directories.

- Spotlight - root directory where images go if they can't be allocated somewhere.
- Spotlight\Landscapes - Landscape wallpapers (JPG's over 100KB in size where width is greater than height).
- Spotlight\Portraits - Portrait wallpapers (JPG's over 100KB where height is greater than width)
- Spotlight\PNGs - Any PNG images. Often icons for Start Menu applications.

A typical use-case for this program is to filter the quite beautiful Spotlight wallpapers into folders where the Windows Desktop Wallpaper slide show tool can read from.

This is a run-once application. Once compiled the application needs to be run regularly (through means of a Windows Scheduled Task, or alternate synchronisation method) in order to regularly copy the latest changes to the Spotlight directory.

There is no file-copying memory implemented in this code. When the tool runs, it will only copy files to the Pictures\Spotlight directories if they do not already exist. If you remove files from the Pictures\Spotlight directory they may be restored on subsequent executions.
