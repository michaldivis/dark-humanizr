# Dark HumanizR
A utility for humanizing drum MIDI.

Before:

![image](https://user-images.githubusercontent.com/45868547/181001103-27a027b4-c378-4092-97c2-f0cc3037fe7c.png)

Listen here: <a href="assets/example_before.mp3">example_before.mp3</a>

After:

![image](https://user-images.githubusercontent.com/45868547/181001162-97caae93-c1fd-4727-b7e6-8e38ddc3ba48.png)

Listen here: <a href="assets/example_after.mp3">example_after.mp3</a>

# Getting Started
1. Download the latest release from https://github.com/michaldivis/dark-humanizr/releases
2. Extract the dowloaded zip file, the CLI tool is called humanizr.exe

## Humanize a MIDI file

If your MIDI file has a tempo track included:
```powershell
humanizr -s "path/to/source.mid" -t "path/to/target.mid"
```

If your MIDI file does NOT have a tempo track included, you also need to provide the tempo manually:
```powershell
humanizr -s "path/to/source.mid" -t "path/to/target.mid" -b 128
```

## Help
```powershell
humanizr --help
```

![image](https://user-images.githubusercontent.com/45868547/181000195-03328708-e695-4d22-9a9d-c1c70e718976.png)
