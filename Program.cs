// Expects the following parameters:
// --output <output vmat folder> --shader <shader name> --aotex <ambient occlusion texture> <input vmat folder>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TGASharpLib;

// Convert an entire folder of VMTs and VTFs to VMATs and TGA files

namespace Vmt2Vmat
{
    class Program
    {
        public static List<string> bumpmaps = new List<string>();
        public static void Main(string[] args)
        {
            // Complain if output is missing, shader defaults to vr_complex and aotex defaults to materials/default/default_ao.tga
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: Vmt2Vmat --deleteexisting --output <output vmat folder> --shader <shader name> --aotex <ambient occlusion texture> <input vmat folder>");
                return;
            }

            // If .\format.vmat is present, override the default format
            string formatPath = Path.Combine(Environment.CurrentDirectory, "format.vmat");
            if (File.Exists(formatPath))
            {
                Vmat.Format = File.ReadAllText(formatPath);
            }

            // Parse arguments
            bool overrideFiles = false;
            string output = null;
            string shader = Vmat.ShaderDefault;
            string aotex = Vmat.TextureAmbientOcclusionDefault;
            string input = null;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--deleteexisting")
                {
                    overrideFiles = true;
                }
                else if (args[i] == "--output")
                {
                    output = args[++i];
                }
                else if (args[i] == "--shader")
                {
                    shader = args[++i];
                }
                else if (args[i] == "--aotex")
                {
                    aotex = args[++i];
                }
                else
                {
                    input = args[i];
                }
            }

            // Complain if output is missing
            if (output == null)
            {
                Console.WriteLine("Usage: Vmt2Vmat --output <output vmat folder> --shader <shader name> --aotex <ambient occlusion texture> <input vmat folder>");
                return;
            }

            // Header
            Console.WriteLine("Vmt2Vmat by KiwifruitDev - Version {0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

            // Create output folder if it doesn't exist
            if (!Directory.Exists(output))
            {
                Directory.CreateDirectory(output);
            }

            // Get all VMTs in the input folder and retain their relative paths
            Dictionary<string, string> vmts = new Dictionary<string, string>();
            foreach (string file in Directory.GetFiles(input, "*.vmt", SearchOption.AllDirectories))
            {
                string relative = file.Substring(input.Length + 1);
                vmts[relative] = file;
            }

            // Get all VTFs in the input folder and retain their relative paths
            Dictionary<string, string> vtfs = new Dictionary<string, string>();
            foreach (string file in Directory.GetFiles(input, "*.vtf", SearchOption.AllDirectories))
            {
                string relative = file.Substring(input.Length + 1);
                vtfs[relative] = file;
            }

            // Convert each VMT to a VMAT
            foreach (KeyValuePair<string, string> vmt in vmts)
            {
                string vmatPath = Path.Combine(output, vmt.Key.Replace(".vmt", ".vmat"));
                // Delete file if override is enabled
                if (overrideFiles)
                {
                    if (File.Exists(vmatPath))
                    {
                        File.Delete(vmatPath);
                    }
                }
                // Read the VMT and replace tabs with spaces
                string vmtText = File.ReadAllText(vmt.Value).Replace("\t", "    ");
                List<string> vmtLines = new List<string>(vmtText.Split('\n'));
                // First line is the shader, remove quotation marks to get shader name
                string shaderName = vmtLines[0].Replace("\"", "");
                // Report warning
                for(int shaderIndex = 0; shaderIndex < Shaders.ShadersList.Count; shaderIndex++)
                {
                    if (shaderName == Shaders.ShadersList[shaderIndex])
                    {
                        if (shaderIndex > Shaders.ShaderCountValid)
                        {
                            Console.WriteLine("Warning: Shader {0} on material {1} may not be supported", shaderName, vmt.Key);
                        }
                    }
                }
                // Report error
                if (shaderName == "LightmappedGeneric")
                {
                    Console.WriteLine("Error: Shader {0} on material {1} is unknown", shaderName, vmt.Key);
                    continue;
                }
                // Get texture names
                string textureAmbientOcclusion = Vmat.TextureAmbientOcclusionDefault;
                string textureColor = Vmat.TextureColorDefault;
                string textureNormal = Vmat.TextureNormalDefault;
                // $basetexture is required, find it
                foreach (string line in vmtLines)
                {
                    if (line.Contains("$basetexture"))
                    {
                        // Whatever is after $basetexture is the texture name
                        textureColor = line.Substring(line.IndexOf("$basetexture") + "$basetexture".Length + 1).Replace("\"", "").Replace("\r", "");
                        // Remove leading and trailing spaces
                        textureColor = "materials/" + textureColor.Trim() + ".tga";
                        break;
                    }
                }
                // $bumpmap is optional, find it
                foreach (string line in vmtLines)
                {
                    if (line.Contains("$bumpmap"))
                    {
                        // Whatever is after $bumpmap is the texture name
                        textureNormal = line.Substring(line.IndexOf("$bumpmap") + "$bumpmap".Length + 1).Replace("\"", "").Replace("\r", "");
                        // Remove leading and trailing spaces
                        textureNormal = "materials/" + textureNormal.Trim() + ".tga";
                        // Add to bumpmap list
                        bumpmaps.Add(Path.Combine(output, textureNormal.Replace(".vtf", "") + ".tga"));
                        break;
                    }
                }
                // No ambient occlusion texture
                // If $basetexture is not found, skip this VMT
                if (textureColor == Vmat.TextureColorDefault)
                {
                    Console.WriteLine("Error: Material {0} has no $basetexture", vmt.Key);
                    continue;
                }
                // Save the VMAT
                Vmat vmat = new Vmat()
                {
                    Shader = shader,
                    TextureAmbientOcclusion = aotex,
                    TextureColor = textureColor,
                    TextureNormal = textureNormal,
                };
                // Create output folder if it doesn't exist
                if (!Directory.Exists(Path.GetDirectoryName(vmatPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(vmatPath));
                }
                File.WriteAllText(vmatPath, vmat.ToString());
            }

            // Convert each VTF to a TGA
            foreach (KeyValuePair<string, string> vtf in vtfs)
            {
                // vtfcmd.exe -file <file> -output <output> -exportformat <tga>
                string vtfCmd = Path.Combine(Environment.CurrentDirectory, "library", "vtf", "vtfcmd.exe");
                string outputCombined = Path.Combine(output, Path.GetDirectoryName(vtf.Key));
                string vtfCmdArgs = string.Format("-file \"{0}\" -output \"{1}\" -exportformat tga", vtf.Value, outputCombined);
                // Delete file if override is enabled
                if (overrideFiles)
                {
                    string existing = Path.Combine(output, vtf.Key.Replace(".vtf", ".tga"));
                    if (File.Exists(existing))
                    {
                        File.Delete(existing);
                    }
                }
                ProcessStartInfo vtfCmdInfo = new ProcessStartInfo(vtfCmd, vtfCmdArgs)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                Process vtfCmdProcess = new Process();
                vtfCmdProcess.StartInfo = vtfCmdInfo;
                vtfCmdProcess.OutputDataReceived += new DataReceivedEventHandler((sender, e) => { Console.WriteLine(e.Data); });
                vtfCmdProcess.ErrorDataReceived += new DataReceivedEventHandler((sender, e) => { Console.WriteLine(e.Data); });
                vtfCmdProcess.Start();
                vtfCmdProcess.BeginOutputReadLine();
                vtfCmdProcess.BeginErrorReadLine();
                vtfCmdProcess.WaitForExit();
                // If the VTF is a bumpmap, invert the green channel
                string tgaPath = Path.Combine(output, vtf.Key.Replace(".vtf", ".tga"));
                if (bumpmaps.Contains(tgaPath))
                {
                    Console.WriteLine("Inverting green channel of {0}", tgaPath);
                    // Load as bitmap
                    TGA tgaParsed = new TGA(tgaPath);
                    System.Drawing.Bitmap bitmap = tgaParsed.ToBitmap();
                    // Invert green channel
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        for (int y = 0; y < bitmap.Height; y++)
                        {
                            System.Drawing.Color color = bitmap.GetPixel(x, y);
                            bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(color.A, color.R, 255 - color.G, color.B));
                        }
                    }
                    // Save as TGA
                    tgaParsed = TGA.FromBitmap(bitmap);
                    tgaParsed.Save(tgaPath);
                }
            }
        }
    }
}
            