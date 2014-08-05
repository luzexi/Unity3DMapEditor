using System;
using System.Collections.Generic;

using Network.Packets;

namespace Network.Handlers
{
    public class GCBankMoneyHandler : HandlerBase
    {
        public override NET_RESULT_DEFINE.PACKET_EXE Execute(PacketBase pPacket, ref Peer pPlayer)
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            {
                GCBankMoney packet = pPacket as GCBankMoney;
                byte IsSave = packet.SaveType;
                int Amount = packet.Amount;
                int AmountRMB = packet.AmountRMB;
                int CurMoney = CDataPool.Instance.UserBank_GetBankMoney();
                int CurRMB = CDataPool.Instance.UserBank_GetBankRMB();

                string szMsg = "";
                if ((IsSave & (byte)CGBankMoney.OPtype.SAVE_MONEY) != 0)
                {
                    CurMoney += Amount;
                    //通知玩家交易成功

                    szMsg = "存入金钱成功";
                    CDataPool.Instance.UserBank_SetBankMoney(CurMoney);
                    //update 界面
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_BANK);
                    //提示信息
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, szMsg);

                }
                else if ((IsSave & (byte)CGBankMoney.OPtype.PUTOUT_MONEY) != 0)
                {
                    CurMoney -= Amount;
                    //通知玩家交易成功		

                    szMsg = "取出金钱成功";

                    CDataPool.Instance.UserBank_SetBankMoney(CurMoney);
                    //update 界面
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_BANK);
                    //提示信息
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, szMsg);

                }
                else if ((IsSave & (byte)CGBankMoney.OPtype.UPDATE_MONEY) != 0)
                {//进入场景时由服务器刷过来，银行中的金钱数
                    CurMoney = Amount;

                    CDataPool.Instance.UserBank_SetBankMoney(CurMoney);
                    //update 界面
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_BANK);
                }

                if ((IsSave & (byte)CGBankMoney.OPtype.SAVE_RMB) != 0)
                {
                    CurRMB += AmountRMB;
                    //通知玩家交易成功

                    szMsg = "存入元宝成功";
                    CDataPool.Instance.UserBank_SetBankRMB(CurRMB);
                    //update 界面
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_BANK);
                    //提示信息
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, szMsg);
                }
                else if ((IsSave & (byte)CGBankMoney.OPtype.PUTOUT_RMB) != 0)
                {
                    CurRMB -= AmountRMB;
                    //通知玩家交易成功		

                    szMsg = "取出元宝成功";

                    CDataPool.Instance.UserBank_SetBankRMB(CurRMB);
                    //update 界面
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_BANK);
                    //提示信息
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, szMsg);
                }
                else if ((IsSave & (byte)CGBankMoney.OPtype.UPDATA_RMB) != 0)
                {
                    CurRMB = AmountRMB;

                    CDataPool.Instance.UserBank_SetBankRMB(CurRMB);
                    //update 界面
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UPDATE_BANK);
                }
            }

            return NET_RESULT_DEFINE.PACKET_EXE.PACKET_EXE_CONTINUE;

        }

        public override int GetPacketID()
        {
            return (int)PACKET_DEFINE.PACKET_GC_BANKMONEY;
        }
    }
}