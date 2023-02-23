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
