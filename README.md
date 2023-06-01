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


