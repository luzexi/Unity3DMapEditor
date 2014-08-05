using System;
using System.Collections.Generic;
using System.Text;

namespace Interface
{
    public class InterfaceTarget
    {
        static readonly InterfaceTarget instance = new InterfaceTarget();
        public static InterfaceTarget Instance { get { return instance; } }

        public string GetDialogNpcName()
        {
            int dialogNpc = CUIDataPool.Instance.GetCurDialogNpcId();
            CObject_PlayerNPC npc = (CObject_PlayerNPC)CObjectManager.Instance.FindServerObject(dialogNpc);

            if (npc != null)
            {
                return npc.GetCharacterData().Get_Name();
            }
            else
            {
                return Character.Instance.GetName();
            }
        }

        public int GetDialogNpcID()
        {
            int dialogNpc = CUIDataPool.Instance.GetCurDialogNpcId();
            CObject_PlayerNPC npc = (CObject_PlayerNPC)CObjectManager.Instance.FindServerObject(dialogNpc);

            if (npc != null)
            {
                return npc.ID;
            }
            else
            {
                return -1;
            }
        }
    }
}
