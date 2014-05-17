using System;
using System.Collections.Generic;


namespace UNM.Parser
{
    public interface INamelistSource
    {
        UnmData LoadData();
    }
}
