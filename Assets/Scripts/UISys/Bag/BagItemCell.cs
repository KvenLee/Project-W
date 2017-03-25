using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Author:Lee
/// Date:2017-3-12
/// Func:背包Cell

public class BagItemCell : ScrollViewCell
{
    protected override void ConfigureCellData()
    {
        if (dataObject == null)
            return;
        Data _data = dataObject as Data;
    }
}
