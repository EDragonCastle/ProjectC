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
    private static string csvCardPath = "Assets/Excel/CardData.csv";
    private static string csvAbilityPath = "Assets/Excel/AbilityData.csv";
    private static string saveSOPath = "Assets/ScriptableObject/Data";
    private static string cardDataAsset = "Assets/ScriptableObject/Data/SOCardData.asset";
    private static string abilityDataAsset = "Assets/ScriptableObject/Data/SOAbilityData.asset";

    /// <summary>
    /// Excel을 csv로 변환해주는 Menu
    /// </summary>
    [MenuItem("Tools/Card Data/1. Excel To CSV File")]
    public static void ExcelToCSV()
    {
        string fullExcelPath = Path.GetFullPath(excelPath);
        string fullCardCSV = Path.GetFullPath(csvCardPath);
        string fullAbilityCSV = Path.GetFullPath(csvAbilityPath);

        if (!File.Exists(fullExcelPath))
        {
            UnityEngine.Debug.LogError($"액셀 파일을 찾을 수 없습니다: {fullExcelPath}");
            return;
        }

        if (ConvertSheetToCSV(fullExcelPath, fullCardCSV, 1) &&
            ConvertSheetToCSV(fullExcelPath, fullAbilityCSV, 2))
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
        TextAsset cardCSVFile = AssetDatabase.LoadAssetAtPath<TextAsset>(csvCardPath);
        if(cardCSVFile == null)
        {
            UnityEngine.Debug.LogError("Card CSV 파일을 찾을 수 없다. csv 파일 변환을 먼저 눌러주세요.");
            return;
        }

        TextAsset abilityCSV = AssetDatabase.LoadAssetAtPath<TextAsset>(csvAbilityPath);
        if (abilityCSV == null)
        {
            UnityEngine.Debug.LogError("AbilityData CSV를 찾을 수 없습니다.");
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

        ScriptableAbilityData abilityDataList = AssetDatabase.LoadAssetAtPath<ScriptableAbilityData>(abilityDataAsset);

        if(abilityDataList == null)
        {
            abilityDataList = ScriptableObject.CreateInstance<ScriptableAbilityData>();
            AssetDatabase.CreateAsset(abilityDataList, abilityDataAsset);
        }
        abilityDataList.abilityData.Clear();
        ParserAbilityCSV(abilityDataList, abilityCSV.text);

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

    private static void ParserAbilityCSV(ScriptableAbilityData soAbilityData, string data)
    {
        string[] lines = Regex.Split(data, @"\r\n(?=(?:[^""]*""[^""]*"")*[^""]*$)");


        for(int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] row = Regex.Split(lines[i], ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            if (string.IsNullOrEmpty(row[0])) continue;

            AbilityData ability = new AbilityData();
            ability.cardId = SafetyParser<uint>(row[0].Trim());
            ability.actionTrigger = row[1].Trim();
            ability.action = row[2].Trim();
            ability.target = row[3].Trim();
            ability.value = SafetyParser<int>(row[4].Trim());
            ability.spawnID = SafetyParser<uint>(row[5].Trim());
            ability.condition = row[6].Trim();
            ability.conditionValue = SafetyParser<int>(row[7].Trim());
            ability.conditionMinionType = row[8].Trim();
            ability.isTargetting = row[9] == "TRUE" ? true : false;
            ability.isTempory = row[10] == "TRUE" ? true : false;
            ability.conditionStat = row[11].Trim();
            soAbilityData.abilityData.Add(ability);
        }

        // Data 저장
        EditorUtility.SetDirty(soAbilityData);
        AssetDatabase.SaveAssets();
    }

    private static bool ConvertSheetToCSV(string inputPath, string outputPath, int sheetIndex)
    {
        UnityEngine.Debug.Log($"Sheet {sheetIndex} → CSV 변환 시작");

        string command = $@"
        $excel = New-Object -ComObject Excel.Application;
        $excel.DisplayAlerts = $false;
        $wb = $excel.Workbooks.Open('{inputPath}');
        
        # 시트 인덱스로 선택 (1부터 시작)
        $ws = $wb.Sheets.Item({sheetIndex});
        $ws.Activate();

        $wb.SaveAs('{outputPath}', 6);
        $wb.Close($false);
        $excel.Quit();
        [System.Runtime.Interopservices.Marshal]::ReleaseComObject($excel) | Out-Null;

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
                UnityEngine.Debug.LogError($"PowerShell Error (Sheet {sheetIndex}): " + errors);
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
