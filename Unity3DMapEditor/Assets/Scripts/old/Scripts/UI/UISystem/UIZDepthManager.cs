using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class UIZDepthManager
{
    static float nearestZ = 10;
    public static float NearWinZ
    {
        get { return nearestZ; }
    }

    static float tooltipZ = 1;
    public static float TooltipZ
    {
        get { return tooltipZ; }
    }

    static float alwaysShowWin = 98;
    static float nameZ = 100;
    public static float NameZ
    {
        get { return nameZ; }
    }
    static float createId = 98;
    public static float CreateId
    {
        get 
        {
            if ((createId -= 2f) < 0)
                createId = 0;
            return createId; 
        }
    }

    public static float GetIdByDemiseType(int demiseType)
    {
        if (demiseType == 0)
            return alwaysShowWin;
        else
            return CreateId;
    }
}
