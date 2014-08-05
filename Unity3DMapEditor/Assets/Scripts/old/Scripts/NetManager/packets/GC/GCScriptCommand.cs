using Network;
using Network.Handlers;
using System;

namespace Network.Packets
{
    public class GCScriptCommand : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_SCRIPTCOMMAND;
        }

        public override int getSize()
        {
            int iSize = sizeof(int);
            switch (m_nCmdID)
            {
                case ENUM_SCRIPT_COMMAND.SCRIPT_COMMAND_EVENT_LIST_RESPONSE:
                    iSize += (int)TempScriptParam_EventList.GetBufSize();
                    break;
                case ENUM_SCRIPT_COMMAND.SCRIPT_COMMAND_MISSION_RESPONSE:
                    iSize += (int)TempScriptParam_MissionInfo.GetBufSize();
                    break;
                case ENUM_SCRIPT_COMMAND.SCRIPT_COMMAND_MISSION_REGIE: // 漕运任务查询返回
                    iSize += (int)TempScriptParam_MissionRegie.GetBufSize();
                    break;
                case ENUM_SCRIPT_COMMAND.SCRIPT_COMMAND_MISSION_DEMAND_RESPONSE:
                    iSize += (int)TempScriptParam_MissionDemandInfo.GetBufSize();
                    break;
                case ENUM_SCRIPT_COMMAND.SCRIPT_COMMAND_MISSION_CONTINUE_RESPONSE:
                    iSize += (int)TempScriptParam_MissionContinueInfo.GetBufSize();
                    break;
                case ENUM_SCRIPT_COMMAND.SCRIPT_COMMAND_MISSION_TIPS:
                    iSize += (int)TempScriptParam_MissionTips.GetBufSize();
                    break;
                case ENUM_SCRIPT_COMMAND.SCRIPT_COMMAND_TRADE:
                    iSize += (int)TempScriptParam_Trade.GetBufSize();
                    break;
                case ENUM_SCRIPT_COMMAND.SCRIPT_COMMAND_SKILL_STUDY:
                    iSize += (int)TempScriptParam_SkillStudy.GetBufSize();
                    break;
                default:
                    break;
            }
            return iSize;
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            int n = MacroDefine.INVALID_ID;
            if (buff.ReadInt(ref n) != sizeof(int))
            {
                return false;
            }
            else
            {
                m_nCmdID = (ENUM_SCRIPT_COMMAND)n;
            }

            switch (m_nCmdID)
            {
                case ENUM_SCRIPT_COMMAND.SCRIPT_COMMAND_EVENT_LIST_RESPONSE:
                    {
                        ScriptParam = TempScriptParam_EventList;
                        if (!TempScriptParam_EventList.Read(ref buff))
                            return false;
                        break;
                    }
                    
                case ENUM_SCRIPT_COMMAND.SCRIPT_COMMAND_MISSION_RESPONSE:
                    {
                        ScriptParam = TempScriptParam_MissionInfo;
                        if (!TempScriptParam_MissionInfo.Read(ref buff))
                            return false;
                        break;
                    }
                case ENUM_SCRIPT_COMMAND.SCRIPT_COMMAND_MISSION_REGIE:
                    {
                        ScriptParam = TempScriptParam_MissionRegie;
                        if (!TempScriptParam_MissionRegie.Read(ref buff))
                            return false;
                        break;
                    }
                case ENUM_SCRIPT_COMMAND.SCRIPT_COMMAND_MISSION_DEMAND_RESPONSE:
                    {
                        ScriptParam = TempScriptParam_MissionDemandInfo;
                        if (!TempScriptParam_MissionDemandInfo.Read(ref buff))
                            return false;
                        break;
                    }
                case ENUM_SCRIPT_COMMAND.SCRIPT_COMMAND_MISSION_CONTINUE_RESPONSE:
                    {
                        ScriptParam = TempScriptParam_MissionContinueInfo;
                        if (!TempScriptParam_MissionContinueInfo.Read(ref buff))
                            return false;
                        break;
                    }
                case ENUM_SCRIPT_COMMAND.SCRIPT_COMMAND_MISSION_TIPS:
                    {
                        ScriptParam = TempScriptParam_MissionTips;
                        if (!TempScriptParam_MissionTips.Read(ref buff))
                            return false;
                        break;
                    }
                case ENUM_SCRIPT_COMMAND.SCRIPT_COMMAND_TRADE:
                    {
                        ScriptParam = TempScriptParam_Trade;
                        if (!TempScriptParam_Trade.Read(ref buff))
                            return false;
                        break;
                    }
                case ENUM_SCRIPT_COMMAND.SCRIPT_COMMAND_SKILL_STUDY:
                    {
                        ScriptParam = TempScriptParam_SkillStudy;
                        if (!TempScriptParam_SkillStudy.Read(ref buff))
                            return false;
                        break;
                    }
                default:
                    break;
            }
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            return getSize();
        }

        public static int MAX_SCRIPT_CMD_BUF_LEN = 2560;

        private ScriptParam_EventList TempScriptParam_EventList = new ScriptParam_EventList();
        private ScriptParam_MissionInfo TempScriptParam_MissionInfo = new ScriptParam_MissionInfo();
        private ScriptParam_EventList TempScriptParam_MissionRegie = new ScriptParam_EventList();
        private ScriptParam_MissionDemandInfo TempScriptParam_MissionDemandInfo = new ScriptParam_MissionDemandInfo();
        private ScriptParam_MissionContinueInfo TempScriptParam_MissionContinueInfo = new ScriptParam_MissionContinueInfo();
        private ScriptParam_MissionTips TempScriptParam_MissionTips = new ScriptParam_MissionTips();
        private ScriptParam_Trade TempScriptParam_Trade = new ScriptParam_Trade();
        private ScriptParam_SkillStudy TempScriptParam_SkillStudy = new ScriptParam_SkillStudy();

        private ENUM_SCRIPT_COMMAND m_nCmdID;
        private object ScriptParam;

        public ENUM_SCRIPT_COMMAND GetCmdID() { return m_nCmdID; }
        public object getBuf() { return ScriptParam; }
    }

    public class GCScriptCommandFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCScriptCommand(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_GC_SCRIPTCOMMAND; }
        public override int GetPacketMaxSize() { return sizeof(int) + GCScriptCommand.MAX_SCRIPT_CMD_BUF_LEN; }
    }
}