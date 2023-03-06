using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proto.Promises;

namespace XX
{
    /// <summary>
    /// 游戏内数据使用对象
    /// </summary>
    public class GameDataObj
    {
        public List<List<int>> sudokuArray = new List<List<int>>();
        public List<int> numCondition = new List<int>();
        public List<int> rowCondition = new List<int>();
        public List<int> colunmCondition = new List<int>();
        public List<int> areaCondition = new List<int>();
    }

    public class GameDataUtil : Singleton<GameDataUtil>
    {
        public Promise Init() {
            return Promise.New((defeered) => {
                defeered.Resolve();
            });
        }
    }
}
