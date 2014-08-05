using System;
using System.Collections.Generic;

using UnityEngine;
using Network.Packets;

public class AddCharmScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public int attr = -1;
	public int level = -1;
	public void useCharm()
    {
		if(attr == -1 || level == -1)
		{
			return;
		}
		string talbeString = "90" + level.ToString() + "0" + attr.ToString() + "001";
		int needIdTable = int.Parse(talbeString);
		for( int i = 0; i < GAMEDEFINE.MAX_BAG_SIZE; i++ )
        {
			CObject_Item objectItem = CDataPool.Instance.UserBag_GetItem(i);
            if( objectItem != null && ( objectItem is CObject_Item_Symbol ) )//如果是符印
            {
				CObject_Item_Symbol charmItem = (CObject_Item_Symbol)objectItem;
				int idTable = charmItem.GetIdTable();
				if( needIdTable == idTable )
				{
					CGUseSymbol useSymbolMsg = new CGUseSymbol();
                	useSymbolMsg.BagIndex = (byte)objectItem.PosIndex;
               		NetManager.GetNetManager().SendPacket(useSymbolMsg);
					//吃一个就返回
					return ;
				}
            }
         }
    }
}
