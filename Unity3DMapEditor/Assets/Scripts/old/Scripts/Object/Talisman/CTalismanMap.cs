using System.Collections.Generic;
public class CTalismanMap
{
    private List<CTalisman_Item> items_;
    private Dictionary<int,bool> mapsStats_;
    private int timeCounts_;
    
    public CTalismanMap()
    {
        items_      = new List<CTalisman_Item>();
        mapsStats_ = new Dictionary<int, bool>();
    }

    public void SetItem(int index, CTalisman_Item item)
    {

    }

    public CTalisman_Item GetItem(int index)
    {
        if (index < 0 || index > items_.Count)
        {
            return null;
        }
        return items_[index];
    }

    public bool isMapActive(int id)
    {
        if (mapsStats_.ContainsKey(id))
        {
            return mapsStats_[id];
        }
        return false;
    }

}
