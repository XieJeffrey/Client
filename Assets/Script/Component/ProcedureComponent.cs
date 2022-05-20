using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XX {
    public class ProcedureComponent : MonoBehaviour {
        public float progress;
        public List<IProcedure> procedureObj = new List<IProcedure>();

        public void StartProcedure() { 
            
        }

        public void UpdateProcedure(float value) { }

        public void FinishProcedure() { 
        
        }
    }
}
