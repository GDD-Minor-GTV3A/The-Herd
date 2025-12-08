using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

/// <summary>
/// This tool can be used to export all the scripts in the project in txt file for whoever needs that
/// </summary>
public class ScriptExporter
{
    [MenuItem("Tools/Export All Scripts to Text")]
    public static void ExportScripts()
    {
        // Get all .cs files inside Assets
        string[] scripts = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== FULL UNITY PROJECT SCRIPT EXPORT ===\n\n");

        foreach (string path in scripts)
        {
            string fileName = Path.GetFileName(path);

            sb.AppendLine("===============================================");
            sb.AppendLine("FILE: " + fileName);
            sb.AppendLine("PATH: " + path.Replace(Application.dataPath, "Assets"));
            sb.AppendLine("===============================================\n");
            sb.AppendLine(File.ReadAllText(path));
            sb.AppendLine("\n\n\n");
        }

        // Path is your desktop
        string outputPath = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop),
            "AllScriptsExport.txt"
        );
        File.WriteAllText(outputPath, sb.ToString());

        EditorUtility.DisplayDialog("Export Complete", "Scripts saved to:\n" + outputPath, "OK");
    }
}
