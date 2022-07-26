# Dark HumanizR
A utility for humanizing drum MIDI.

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

### Parameters
| Name | Required | Description | 
| --- | --- | --- | 
| `-s` or `--source` | Yes | Source drum MIDI file | 
| `-t` or `--target` | Yes | Target drum MIDI file, will create a new file or override an existing one | 
| `-b` or `--bpm` | No | Static BPM to be used instead of reading tempo from MIDI tempo changes |

## Help
```powershell
humanizr --help
```