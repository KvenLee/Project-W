using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Author:Lee
/// Date:2017-3-14
/// Func:主角英雄Cell

public class UIPlayerCell : ScrollViewCell
{
    protected override void ConfigureCellData()
    {
        if (dataObject == null)
            return;
        Data _data = dataObject as Data;
    }
}
