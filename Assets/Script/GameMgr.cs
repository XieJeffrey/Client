using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameMgr
{
    public void InitSudoku() {
        //初始化数独数组
        for (int i = 0; i < 9; i++) {
            App.Data.GameData.sudokuArray.Add(new List<int>());
            for (int j = 0; j < 9; j++) {
                App.Data.GameData.sudokuArray[i].Add(0);
            }
        }
        //随机位置填上N个随机位置
        RandomSudokuList(Random.Range(5,10));
        //生成精确覆盖条件矩阵
        int[,] danceLinkData = GenDanceLinkArray();

        //求解
        var dlx = new DlxLib.Dlx();
        IEnumerable<DlxLib.Solution> result = dlx.Solve(danceLinkData).Take(1);
        //万一生成不了的话再执行一次随机生成
        if (result.Count() == 0) {
            this.InitSudoku();
            return;
        }
        foreach (DlxLib.Solution s in result) {
            int[] answer = s.RowIndexes.ToArray();
            string str = "";
            for (int i = 0; i < answer.Length; i++)
                str += answer[i] + " ";
            //将矩阵还原成数独数据
            ConvertAnswerToSudoku(danceLinkData, answer);
        }

    }

    /// <summary>
    /// 在随机位置上随机生成{num}个数字
    /// </summary>
    /// <param name="num"></param>
    public void RandomSudokuList(int num) {
        //初始化条件数组
        int length = 9 * 9 * 4;

        //初始化随机数组
        List<int> rowArray = new List<int>();
        List<int> columnArray = new List<int>();
        List<int> numArray = new List<int>();
        for (int i = 0; i < 9; i++) {
            rowArray.Add(i);
            columnArray.Add(i);
            numArray.Add(i + 1);
        }
        //乱序行，列，数字数组
        rowArray.Sort((int a, int b) => { return Random.Range(0.0f, 1.0f) < 0.5f ? -1 : 1; });
        columnArray.Sort((int a, int b) => { return Random.Range(0.0f, 1.0f) < 0.5f ? -1 : 1; });
        numArray.Sort((int a, int b) => { return Random.Range(0.0f, 1.0f) < 0.5f ? -1 : 1; });

        //开始先随机{num}个数字出来
        int genNum = num;
        int[] tmp = new int[length];
        while (genNum > 0) {
            int i = rowArray[Random.Range(0, 9)];
            int j = columnArray[Random.Range(0, 9)];
            int k = numArray[Random.Range(0, 9)];

            int a = i * 9 + j;//第（i*9+j）的位置上有没有数字
            int b = i * 9 + k + 80;//第{i}行上有没有数字k
            int c = j * 9 + k + 161;//第{j}列上有没有数字k
            int d = (Mathf.FloorToInt(i / 3) * 3 + Mathf.FloorToInt(j / 3)) * 9 + k + 242;//第{x}个九宫里有没有数字k

            //不满足精确覆盖条件的，重新返回生成
            if (tmp[a] == 1)
                continue;
            if (tmp[b] == 1)
                continue;
            if (tmp[c] == 1)
                continue;
            if (tmp[d] == 1)
                continue;
            tmp[a] = 1;
            tmp[b] = 1;
            tmp[c] = 1;
            tmp[d] = 1;

            App.Data.GameData.sudokuArray[i][j] = k;
            genNum--;
        }
    }

    /// <summary>
    /// 生成dancelink精确覆盖矩阵
    /// </summary>
    /// <returns></returns>
    public int[,] GenDanceLinkArray() {
        int length = 9 * 9 * 4;
        List<int[]> tmpList = new List<int[]>();
        for (int i = 0; i < 9; i++) {
            for (int j = 0; j < 9; j++) {
                //该位置的没有数字的，把所有的情况列出来填进去
                if (App.Data.GameData.sudokuArray[i][j] == 0) {
                    for (int k = 1; k <= 9; k++) {
                        int[] tmp = new int[length];
                        int a = i * 9 + j;
                        int b = i * 9 + k + 80;
                        int c = j * 9 + k + 161;
                        int d = (Mathf.FloorToInt(i / 3) * 3 + Mathf.FloorToInt(j / 3)) * 9 + k + 242;

                        tmp[a] = 1;
                        tmp[b] = 1;
                        tmp[c] = 1;
                        tmp[d] = 1;
                        tmpList.Add(tmp);
                    }

                }
                else {
                    //已经填写了数字的，加上该数字的覆盖条件即可
                    int[] tmp = new int[length];
                    int k = App.Data.GameData.sudokuArray[i][j];
                    int a = i * 9 + j;
                    int b = i * 9 + k + 80;
                    int c = j * 9 + k + 161;
                    int d = (Mathf.FloorToInt(i / 3) * 3 + Mathf.FloorToInt(j / 3)) * 9 + k + 242;
                    tmp[a] = 1;
                    tmp[b] = 1;
                    tmp[c] = 1;
                    tmp[d] = 1;

                    tmpList.Add(tmp);
                }

            }
        }

        //转化
        int[][] result = tmpList.ToArray();
        int[,] output = new int[result.Length, result[0].Length];
        for (int i = 0; i < result.Length; i++) {
            for (int j = 0; j < result[i].Length; j++) {
                output[i, j] = result[i][j];
            }
        }
        return output;
    }

    /// <summary>
    /// 把dancelink的答案转化成数独数组
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="answer"></param>
    public void ConvertAnswerToSudoku(int[,] condition, int[] answer) {
        List<int[]> selection = new List<int[]>();
        for (int i = 0; i < answer.Length; i++) {
            int idx = answer[i];
            int r = 0;
            int c = 0;
            int num = 0;
            for (int j = 0; j < 81; j++) {
                if (condition[idx, j] == 1) {
                    r = Mathf.FloorToInt(j / 9);
                    c = j % 9;
                    break;
                }
            }

            for (int j = 81; j < 162; j++) {
                if (condition[idx, j] == 1) {
                    num = (j - 81) - (r * 9);
                    break;
                }
            }

            App.Data.GameData.sudokuArray[r][c] = num + 1;
        }
        LogSudoku();
    }

    public void LogSudoku() {
        string tmp = "";
        for (int i = 0; i < 9; i++) {
            for (int j = 0; j < 9; j++) {
                tmp += App.Data.GameData.sudokuArray[i][j] + " ";
            }
            tmp += "\n";
        }
        Debug.Log(tmp);
    }
}
