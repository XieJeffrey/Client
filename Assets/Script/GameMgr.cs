using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameMgr
{
    public void InitSudoku() {
        //��ʼ����������
        for (int i = 0; i < 9; i++) {
            App.Data.GameData.sudokuArray.Add(new List<int>());
            for (int j = 0; j < 9; j++) {
                App.Data.GameData.sudokuArray[i].Add(0);
            }
        }
        //���λ������N�����λ��
        RandomSudokuList(Random.Range(5,10));
        //���ɾ�ȷ������������
        int[,] danceLinkData = GenDanceLinkArray();

        //���
        var dlx = new DlxLib.Dlx();
        IEnumerable<DlxLib.Solution> result = dlx.Solve(danceLinkData).Take(1);
        //��һ���ɲ��˵Ļ���ִ��һ���������
        if (result.Count() == 0) {
            this.InitSudoku();
            return;
        }
        foreach (DlxLib.Solution s in result) {
            int[] answer = s.RowIndexes.ToArray();
            string str = "";
            for (int i = 0; i < answer.Length; i++)
                str += answer[i] + " ";
            //������ԭ����������
            ConvertAnswerToSudoku(danceLinkData, answer);
        }

    }

    /// <summary>
    /// �����λ�����������{num}������
    /// </summary>
    /// <param name="num"></param>
    public void RandomSudokuList(int num) {
        //��ʼ����������
        int length = 9 * 9 * 4;

        //��ʼ���������
        List<int> rowArray = new List<int>();
        List<int> columnArray = new List<int>();
        List<int> numArray = new List<int>();
        for (int i = 0; i < 9; i++) {
            rowArray.Add(i);
            columnArray.Add(i);
            numArray.Add(i + 1);
        }
        //�����У��У���������
        rowArray.Sort((int a, int b) => { return Random.Range(0.0f, 1.0f) < 0.5f ? -1 : 1; });
        columnArray.Sort((int a, int b) => { return Random.Range(0.0f, 1.0f) < 0.5f ? -1 : 1; });
        numArray.Sort((int a, int b) => { return Random.Range(0.0f, 1.0f) < 0.5f ? -1 : 1; });

        //��ʼ�����{num}�����ֳ���
        int genNum = num;
        int[] tmp = new int[length];
        while (genNum > 0) {
            int i = rowArray[Random.Range(0, 9)];
            int j = columnArray[Random.Range(0, 9)];
            int k = numArray[Random.Range(0, 9)];

            int a = i * 9 + j;//�ڣ�i*9+j����λ������û������
            int b = i * 9 + k + 80;//��{i}������û������k
            int c = j * 9 + k + 161;//��{j}������û������k
            int d = (Mathf.FloorToInt(i / 3) * 3 + Mathf.FloorToInt(j / 3)) * 9 + k + 242;//��{x}���Ź�����û������k

            //�����㾫ȷ���������ģ����·�������
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
    /// ����dancelink��ȷ���Ǿ���
    /// </summary>
    /// <returns></returns>
    public int[,] GenDanceLinkArray() {
        int length = 9 * 9 * 4;
        List<int[]> tmpList = new List<int[]>();
        for (int i = 0; i < 9; i++) {
            for (int j = 0; j < 9; j++) {
                //��λ�õ�û�����ֵģ������е�����г������ȥ
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
                    //�Ѿ���д�����ֵģ����ϸ����ֵĸ�����������
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

        //ת��
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
    /// ��dancelink�Ĵ�ת������������
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
