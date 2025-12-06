using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class ScriptExporter
{
    [MenuItem("Tools/Export All Scripts to Text (Assets Only)")]
    public static void ExportScripts()
    {
        string assetsRoot = Application.dataPath;                         // ex: C:/Project/Assets
        string packagesRoot = Path.GetFullPath(Path.Combine(assetsRoot, "../Packages"));

        // Get ALL .cs files inside Assets folder physically.
        string[] scripts = Directory.GetFiles(assetsRoot, "*.cs", SearchOption.AllDirectories);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== UNITY PROJECT SCRIPT EXPORT (ASSETS ONLY) ===\n\n");

        foreach (string path in scripts)
        {
            // Safety check — skip if the script path falls inside Packages folder
            string fullPath = Path.GetFullPath(path);

            if (fullPath.StartsWith(packagesRoot)) 
                continue; // Ignore any Packages scripts

            string fileName = Path.GetFileName(path);

            sb.AppendLine("===============================================");
            sb.AppendLine("FILE: " + fileName);
            sb.AppendLine("PATH: " + fullPath.Replace(assetsRoot, "Assets"));
            sb.AppendLine("===============================================\n");
            sb.AppendLine(File.ReadAllText(fullPath));
            sb.AppendLine("\n\n\n");
        }

        // Output to Desktop
        string outputPath = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop),
            "AllScriptsExport_AssetsOnly.txt"
        );

        File.WriteAllText(outputPath, sb.ToString());
        EditorUtility.DisplayDialog("Export Complete", "Scripts saved to:\n" + outputPath, "OK");
    }
}