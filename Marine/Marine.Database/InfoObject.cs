using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marine.Database
{
    public delegate string UpdateObjectInfo(string infoString);
    public class InfoObject
    {
        public event UpdateObjectInfo OnUpdateObjInfo;

        public void UpdateInfo(string infoString)
        {
            if (OnUpdateObjInfo != null)
                OnUpdateObjInfo(infoString);
        }
    }
}
