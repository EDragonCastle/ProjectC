using UnityEditor;
using UnityEngine;
using System.IO;
using System.Diagnostics;

public class ExcelConverter : EditorWindow
{
    private static string excelPath = "Assets/Excel/CardData.xlsx";
    private static string csvPath = "Assets/Excel/CardData.csv";
    private static string savePath = "Assets/Excel/";

    [MenuItem("Tools/Card Data/Excel To CSV File")]
    public static void ExcelToCSV()
    {
        string fullExcelPath = Path.GetFullPath(excelPath);
        string fullCSVPath = Path.GetFullPath(csvPath);

        if(!File.Exists(fullExcelPath))
        {
            UnityEngine.Debug.LogError($"액셀 파일을 찾을 수 없습니다. 경로를 확인해주세요: {fullExcelPath}");
            return;
        }

        if(ConverterExcelToCSV(fullExcelPath, fullCSVPath))
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    private static bool ConverterExcelToCSV(string inputPath, string outputPath)
    {
        UnityEngine.Debug.Log("PowerShell을 이용해 Excel Converter start (UTF-8)");

        // 스크립트 수정: CSV로 저장 후, 해당 파일을 읽어 UTF8로 다시 저장
        // 근데 이거는 잘 모르겠네 진짜 처음봐서 하나도 모른다.
        string command = $@"
        $excel = New-Object -ComObject Excel.Application;
        $excel.DisplayAlerts = $false; # 경고창 끄기
        $wb = $excel.Workbooks.Open('{inputPath}');
        
        # 일단 임시 CSV로 저장 (6 = xlCSV)
        $wb.SaveAs('{outputPath}', 6); 
        $wb.Close($false);
        $excel.Quit();
        [System.Runtime.Interopservices.Marshal]::ReleaseComObject($excel) | Out-Null;

        # 중요: 생성된 CSV 파일을 읽어서 UTF-8로 다시 인코딩하여 저장
        # -Encoding utf8 은 BOM이 포함된 UTF-8을 만듭니다. (유니티에서 가장 잘 인식함)
        $content = Get-Content '{outputPath}';
        $content | Set-Content '{outputPath}' -Encoding utf8;
        ";

        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = "powershell.exe",
            Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true
        };

        using (Process process = Process.Start(startInfo))
        {
            string errors = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(errors))
            {
                UnityEngine.Debug.LogError("PowerShell Error: " + errors);
                return false;
            }
        }
        return true;
    }


}
