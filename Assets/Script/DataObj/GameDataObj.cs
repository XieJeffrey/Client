using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proto.Promises;

namespace XX
{
    /// <summary>
    /// ��Ϸ������ʹ�ö���
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
