using UnityEditor;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ComponentModel;

public class ExcelConverter : EditorWindow
{
    private static string excelPath = "Assets/Excel/CardData.xlsx";
    private static string csvPath = "Assets/Excel/CardData.csv";
    private static string saveSOPath = "Assets/ScriptableObject/Data";
    private static string cardDataAsset = "Assets/ScriptableObject/Data/SOCardData.asset";

    /// <summary>
    /// Excel을 csv로 변환해주는 Menu
    /// </summary>
    [MenuItem("Tools/Card Data/1. Excel To CSV File")]
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

    /// <summary>
    /// CSV을 ScritableObject로 변환해주는 Menu
    /// </summary>
    [MenuItem("Tools/Card Data/2. CSV To ScritableObject")]
    public static void CSVToSO()
    {
        TextAsset cardCSVFile = AssetDatabase.LoadAssetAtPath<TextAsset>(csvPath);
        if(cardCSVFile == null)
        {
            UnityEngine.Debug.LogError("Card CSV 파일을 찾을 수 없다. csv 파일 변환을 먼저 눌러주세요.");
            return;
        }

        // 폴더 없으면 생성
        if (!Directory.Exists(saveSOPath))
            Directory.CreateDirectory(saveSOPath);

        ScritableCardData cardDataList = AssetDatabase.LoadAssetAtPath<ScritableCardData>(cardDataAsset);

        if(cardDataList == null)
        {
            cardDataList =  ScriptableObject.CreateInstance<ScritableCardData>();
            AssetDatabase.CreateAsset(cardDataList, cardDataAsset);
        }

        cardDataList.cardDatas.Clear();

        // 파싱과 SO 생성을 한다.
        ParseCardCSV(cardDataList, cardCSVFile.text);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void ParseCardCSV(ScritableCardData soCardData, string data)
    {
        string[] lines = Regex.Split(data, @"\r\n(?=(?:[^""]*""[^""]*"")*[^""]*$)");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] row = Regex.Split(lines[i], ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            if (string.IsNullOrEmpty(row[0]))
                continue;

            CardData card = new CardData();

            card.cardId = SafetyParser<uint>(row[0].Trim());
            card.cardName = row[1].Trim();
            card.cost = SafetyParser<int>(row[2].Trim());
            card.attack = SafetyParser<int>(row[3].Trim());
            card.health = SafetyParser<int>(row[4].Trim());
            card.description = row[5].Trim().Replace("\\r\\n", "\n").Replace("\\n", "\n").Replace("\r\n", "\n");
            card.spriteName = row[6].Trim();
            card.gem = row[7].Trim();

            // 변경 예정
            card.isMinion = row[8].Trim() == "Minion" ? true : false;
            card.cardCategory = row[8].Trim();

            card.jobType = row[9].Trim();
            card.packgeType = row[10].Trim();

            if (row[8].Trim() == "Minion")
            {
                card.cardType = row[11].Trim();

                if (!string.IsNullOrWhiteSpace(row[11]))
                {
                    string typeWords = row[11].Replace("\"", "").Trim();
                    string[] typeIds = typeWords.Split(',');
                    card.cardTypes = new string[typeIds.Length];

                    for (int j = 0; j < typeIds.Length; j++)
                    {
                        card.cardTypes[j] = typeIds[j];
                    }
                }
            }
            else if (row[8].Trim() == "Magic")
            {
                card.cardType = row[12].Trim();

                if (!string.IsNullOrWhiteSpace(row[12]))
                {
                    string typeWords = row[12].Replace("\"", "").Trim();
                    string[] typeIds = typeWords.Split(',');
                    card.cardTypes = new string[typeIds.Length];

                    for (int j = 0; j < typeIds.Length; j++)
                    {
                        card.cardTypes[j] = typeIds[j];
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(row[13]))
            {
                string spawnNumberWord = row[13].Replace("\"", "").Trim();
                string[] spawnIds = spawnNumberWord.Split(',');
                card.spawn = new uint[spawnIds.Length];

                for (int j = 0; j < spawnIds.Length; j++)
                {
                    card.spawn[j] = SafetyParser<uint>(spawnIds[j]);
                }
            }
            else
                card.spawn = new uint[0];

            card.isCollector = row[14] == "TRUE" ? true : false;
            card.posX = SafetyParser<float>(row[15]);
            card.posY = SafetyParser<float>(row[16]);
            card.rotation = SafetyParser<float>(row[17]);

            soCardData.cardDatas.Add(card);
        }

        // Data 저장
        EditorUtility.SetDirty(soCardData);
        AssetDatabase.SaveAssets();
    }

    
    private static bool ConverterExcelToCSV(string inputPath, string outputPath)
    {
        UnityEngine.Debug.Log("PowerShell을 이용해 Excel Converter start (UTF-8)");

        // 스크립트 수정: CSV로 저장 후, 해당 파일을 읽어 UTF8로 다시 저장
        // 근데 이거는 잘 모르겠다. 진짜 처음봐서 하나도 모른다.
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


    private static T SafetyParser<T>(string number) where T : struct
    {
        // 1. 빈 값 처리
        if (string.IsNullOrWhiteSpace(number)) return default(T);

        // 2. 따옴표 및 공백 제거
        string cleanValue = number.Replace("\"", "").Trim();

        try
        {
            // 3. TypeDescriptor를 이용한 변환 (int, float, uint, bool 등 대부분 지원)
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null)
            {
                return (T)converter.ConvertFromString(cleanValue);
            }
        }
        catch
        {
            // 변환 실패 시 해당 타입의 기본값(0, 0f 등) 반환
            return default(T);
        }

        return default(T);
    }
}
