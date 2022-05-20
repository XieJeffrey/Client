using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XX {
    public interface IProcedure {
        // Start is called before the first frame update
        void Init();

        void InitFinish();

        void UpdateProgress(float value);
    }
}
