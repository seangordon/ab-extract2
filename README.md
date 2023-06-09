# ab-extract2
Command line interface for [AAXClean](https://github.com/Mbucari/AAXClean)

Simplified version to suit my use case, which is downloaded aax files which I need to decode in a consistent manner - e.g. same parameters every time, so modified to use app.comfig for all settings since these are mostly static.

## Currently Supports:
- Aax and Aaxc files
- Parameters passed via app.config file - command line is input filename only
- Auto split on chapters
- Input Aax(c) files
  - From local file system
- Output
  - Single m4b file to local file system
  - Multiple m4b files split on chapter and filed into Artist/Title folder structure
  - folder.jpg - cover art

## Configuration
A sample `app.config` file is located in `.\src` folder named `SAMPLEapp.config`. 
- This should be renamed to `app.config`
- The Activate Bytes (8 hex chars) should be added to this file     

## Running in a Dev Container
VS Code with the Dev Containers extension will detect the `.devcontainer\devcontainer.json` file and offer to run the solution in a .NET DevContainer
The Dev Conatiner config does the following additional steps to enable the solution 
- NuGets the required AAXClean package 
- Adds a file mount that expects to find <home>/tmp directory on the host machine for use when accessing audio files.
- Modify the source or target locations specified in the mount to match your local setup     

# Anti-Piracy Notice

Note that this software does not ‘crack’ the DRM or circumvent it in any other way. The application applies account and book specific keys, retrieved directly from the Audible server via the user’s personal account, to decrypt the audiobook in the same manner as the official audiobook playing software does. Please only use this application for gaining full access to your own audiobooks for archiving/conversion/convenience. De-DRMed audiobooks must not be uploaded to open servers, torrents, or other methods of mass distribution. No help will be given to people doing such things. Authors, retailers and publishers all need to make a living, so that they can continue to produce audiobooks for us to listen to and enjoy.

(This blurb is borrowed from https://github.com/KrumpetPirate/AAXtoMP3 and https://apprenticealf.wordpress.com/).
