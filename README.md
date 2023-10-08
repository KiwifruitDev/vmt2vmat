# Vmt2Vmat

Requires [.NET 7.0](https://dotnet.microsoft.com/download/dotnet/7.0) or higher.

```
Usage: Vmt2Vmat --output <output vmat folder> --shader <shader name> --aotex <ambient occlusion texture> <input vmat folder>
```

## Example

```bash
vmt2vmat.exe --deleteexisting --output "C:\Program Files (x86)\Steam\steamapps\common\Counter-Strike Global Offensive\content\csgo_addons\sfm\materials" "C:\Program Files (x86)\Steam\steamapps\common\SourceFilmmaker\game\kiwano\materials"
```

Default shader is `csgo_complex`.