using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.GameProtocol;
using Google.Protobuf.ProtoObject;
using RealTime;
using RealTime.Common;
using RealTime.Server;
using UnityEngine;
public class SampleServer : RealTimeServer
{
    public void Init()
    {
        ServerEventHandler handler = new ServerEventHandler();
        handler.RegisterNet<C_G_Ready>((UInt16)GamePacketId.CGReady, CGReady);

        handler.RegisterNet<C_G_Start>((UInt16)GamePacketId.CGStart, CGStart);
        handler.RegisterNet<C_G_End>((UInt16)GamePacketId.CGEnd, CGEnd);
        handler.RegisterNet<C_G_Battle_Start>((UInt16)GamePacketId.CGBattleStart, CGBattleStart);
        handler.RegisterNet<C_G_Battle_End>((UInt16)GamePacketId.CGBattleEnd, CGBattleEnd);

        handler.RegisterNet<C_G_Ship_Spawn>((UInt16)GamePacketId.CGShipSpawn, CGShipSpawn);
        handler.RegisterNet<C_G_Stat_Sync>((UInt16)GamePacketId.CGStatSync, CGStatSync);
        handler.RegisterNet<C_G_Player_Die>((UInt16)GamePacketId.CGPlayerDie, CGPlayerDie);

        handler.RegisterNet<C_G_Island_Spawn>((UInt16)GamePacketId.CGIslandSpawn, CGIslandSpawn);
        handler.RegisterNet<C_G_Island_Hp_Changed>((UInt16)GamePacketId.CGIslandHpChanged, CGIslandHpChanged);
        handler.RegisterNet<C_G_Island_Attack>((UInt16)GamePacketId.CGIslandAttack, CGIslandAttack);
        handler.RegisterNet<C_G_Dokdo_Stat>((UInt16)GamePacketId.CGDokdoStat, CGDokdoStat);

        handler.RegisterNet<C_G_Rejoin>((UInt16)GamePacketId.CGRejoin, CGRejoin);
        handler.RegisterNet<C_G_Rejoin_Obj>((UInt16)GamePacketId.CGRejoinObj, CGRejoinObj);

        handler.RegisterNet<C_G_Time_Sync>((UInt16)GamePacketId.CGTimeSync, CGTimeSync);
        handler.RegisterNet<C_G_Redzone_Sync>((UInt16)GamePacketId.CGRedzoneSync, CGRedZoneSync);

        handler.RegisterNet<C_G_Supply_Spawn>((UInt16)GamePacketId.CGSupplySpawn, CGSupplySpawn);
        handler.RegisterNet<C_G_Supply_Rejoin>((UInt16)GamePacketId.CGSupplyRejoin, CGSupplyRejoin);
        handler.RegisterNet<C_G_Supply_Destroy>((UInt16)GamePacketId.CGSupplyDestroy, CGSupplyDestory);

        var result = base.Init("Cookie_SYD_Second", typeof(SampleRoom), handler);
        if (!result)
        {
            Debug.Log("Server Init Error");
        }
    }

    // server packet function
    private void CGReady(ulong _session_id, CustomRoom _custom_room, IMessage _packet)
    {
        SampleRoom room = (SampleRoom)_custom_room;
        room.PlayerReady(_session_id);
    }

    private void CGStart(ulong _session_id, CustomRoom _custom_room, IMessage _packet)
    {
        SampleRoom room = (SampleRoom)_custom_room;
        room.CheckReady();
    }

    private void CGEnd(ulong _session_id, CustomRoom _custom_room, IMessage _packet)
    {
        throw new NotImplementedException();
    }

    private void CGBattleStart(ulong _session_id, CustomRoom _custom_room, IMessage _packet)
    {
        // Debug.Log("Server Battle Packet");
        _custom_room.Broadcast((UInt16)GamePacketId.GCBattleStart, new G_C_Battle_Start());
    }

    private void CGBattleEnd(ulong _session_id, CustomRoom _custom_room, IMessage _packet)
    {
        C_G_Battle_End packet = (C_G_Battle_End)_packet;
        G_C_Battle_End response = new G_C_Battle_End();
        response.WinningTeam = packet.WinningTeam;

        _custom_room.Broadcast((UInt16)GamePacketId.GCBattleEnd, response);
    }

    private void CGShipSpawn(ulong _session_id, CustomRoom _custom_room, IMessage _packet)
    {
        C_G_Ship_Spawn packet = (C_G_Ship_Spawn)_packet;
        G_C_Ship_Spawn response = new G_C_Ship_Spawn();

        response.SessionID = packet.SessionID;
        response.Index = packet.Index;
        // Debug.Log(response.Index);
        response.Team = packet.Team;

        _custom_room.Broadcast((UInt16)GamePacketId.GCShipSpawn, response);
    }

    private void CGStatSync(ulong _session_id, CustomRoom _custom_room, IMessage _packet)
    {
        C_G_Stat_Sync packet = (C_G_Stat_Sync)_packet;
        G_C_Stat_Sync response = new G_C_Stat_Sync();
        response.SessionID = packet.SessionID;
        response.ShipStat = packet.ShipStat;

        _custom_room.Broadcast((UInt16)GamePacketId.GCStatSync, response);
    }

    private void CGPlayerDie(ulong _session_id, CustomRoom _custom_room, IMessage _packet)
    {
        C_G_Player_Die packet = (C_G_Player_Die)_packet;
        G_C_Player_Die response = new G_C_Player_Die();
        response.SessionID = packet.SessionID;

        _custom_room.Broadcast((UInt16)GamePacketId.GCPlayerDie, response);
    }

    private void CGIslandSpawn(ulong _session_id, CustomRoom _custom_room, IMessage _packet)
    {
        C_G_Island_Spawn packet = (C_G_Island_Spawn)_packet;
        G_C_Island_Spawn response = new G_C_Island_Spawn();

        response.Position = new Position();
        response.IsRejoin = packet.IsRejoin;
        response.IslandBaseID = packet.IslandBaseID;
        response.IslandKey = packet.IslandKey;
        response.Hp = packet.Hp;
        response.Position = packet.Position;
        response.SessionID = packet.SessionID;
        response.OwnerID = packet.OwnerID;

        if (!packet.IsRejoin)
            _custom_room.Broadcast((UInt16)GamePacketId.GCIslandSpawn, response);
        else
            _custom_room.SendTarget((UInt16)GamePacketId.GCIslandSpawn, response, packet.SessionID);
    }

    private void CGIslandHpChanged(ulong _session_id, CustomRoom _custom_room, IMessage _packet)
    {
        C_G_Island_Hp_Changed packet = (C_G_Island_Hp_Changed)_packet;
        G_C_Island_Hp_Changed response = new G_C_Island_Hp_Changed();

        response.Index = packet.Index;
        response.Damage = packet.Damage;
        response.SessionID = packet.SessionID;
        response.IsDokdo = packet.IsDokdo;

        _custom_room.Broadcast((UInt16)GamePacketId.GCIslandHpChanged, response);
    }

    private void CGIslandAttack(ulong _session_id, CustomRoom _custom_room, IMessage _packet)
    {
        C_G_Island_Attack packet = (C_G_Island_Attack)_packet;
        G_C_Island_Attack response = new G_C_Island_Attack();
        response.Degree = packet.Degree;
        response.Index = packet.Index;
        response.Position = packet.Position;
        response.IsDokdo = packet.IsDokdo;

        _custom_room.Broadcast((UInt16)GamePacketId.GCIslandAttack, response);
    }

    private void CGDokdoStat(ulong _session_id, CustomRoom _custom_room, IMessage _packet)
    {
        C_G_Dokdo_Stat packet = (C_G_Dokdo_Stat)_packet;
        G_C_Dokdo_Stat response = new G_C_Dokdo_Stat();

        response.Hp = packet.Hp;
        response.OwnerID = packet.OwnerID;
        response.SessionID = packet.SessionID;

        _custom_room.SendTarget((UInt16)GamePacketId.GCDokdoStat, response, packet.SessionID);
    }

    private void CGRejoin(ulong _session_id, CustomRoom _custom_room, IMessage _packet)
    {
        C_G_Rejoin packet = (C_G_Rejoin)_packet;
        G_C_Rejoin response = new G_C_Rejoin();
        response.SessionID = packet.SessionID;
        _custom_room.SendTarget((UInt16)GamePacketId.GCRejoin, response, packet.SessionID);
    }

    private void CGRejoinObj(ulong _session_id, CustomRoom _custom_room, IMessage _packet)
    {
        C_G_Rejoin_Obj packet = (C_G_Rejoin_Obj)_packet;
        G_C_Rejoin_Obj response = new G_C_Rejoin_Obj();
        response.SessionID = packet.SessionID;
        response.Value.Add(packet.Value);
        response.ShipStat = packet.ShipStat;

        _custom_room.SendTarget((UInt16)GamePacketId.GCRejoinObj, response, packet.SessionID);
    }

    private void CGTimeSync(ulong _session_id, CustomRoom _custom_room, IMessage _packet)
    {
        // Debug.Log("Send Time Packet");
        C_G_Time_Sync packet = (C_G_Time_Sync)_packet;
        G_C_Time_Sync response = new G_C_Time_Sync();
        response.Time = packet.Time;

        _custom_room.Broadcast((UInt16)GamePacketId.GCTimeSync, response);
    }

    private void CGRedZoneSync(ulong _session_id, CustomRoom _custom_room, IMessage _packet)
    {
        C_G_Redzone_Sync packet = (C_G_Redzone_Sync)_packet;
        G_C_Redzone_Sync response = new G_C_Redzone_Sync();
        response.Size = packet.Size;

        _custom_room.Broadcast((UInt16)GamePacketId.GCRedzoneSync, response);
    }

    private void CGSupplySpawn(ulong _session_id, CustomRoom _custom_room, IMessage _packet)
    {
        C_G_Supply_Spawn packet = (C_G_Supply_Spawn)_packet;
        G_C_Supply_Spawn response = new G_C_Supply_Spawn();
        response.Position = packet.Position;
        response.Time = DateTime.Now.Ticks;
        _custom_room.Broadcast((UInt16)GamePacketId.GCSupplySpawn, response);
    }

    private void CGSupplyRejoin(ulong _session_id, CustomRoom _custom_room, IMessage _packet)
    {
        C_G_Supply_Rejoin packet = (C_G_Supply_Rejoin)_packet;
        G_C_Supply_Rejoin response = new G_C_Supply_Rejoin();
        response.Position = packet.Position;
        response.SessionID = packet.SessionID;

        _custom_room.SendTarget((UInt16)GamePacketId.GCSupplyRejoin, response, packet.SessionID);
    }

    private void CGSupplyDestory(ulong _session_id, CustomRoom _custom_room, IMessage _packet)
    {
        C_G_Supply_Destroy packet = (C_G_Supply_Destroy)_packet;
        G_C_Supply_Destroy response = new G_C_Supply_Destroy();
        response.Position = packet.Position;

        _custom_room.Broadcast((UInt16)GamePacketId.GCSupplyDestroy, response);
    }
}


class SampleRoom : CustomRoom
{
    List<ulong> ReadyPlayerCount = new List<ulong>();
    int livePlayerCount = 0;

    public void CheckReady()
    {
        if (ReadyPlayerCount.Count == PlayerCount() && PlayerCount() % 2 == 0)
        {
            Broadcast((UInt16)GamePacketId.GCStart, new G_C_Start());
        }
    }

    public void PlayerReady(ulong _session_id)
    {
        if (!ReadyPlayerCount.Contains(_session_id))
        {
            ReadyPlayerCount.Add(_session_id);
        }
    }

    public override void OnCreateRoom(CoreDefine.RT_C_G_Create_Room _packet)
    {
        livePlayerCount++;
    }

    public override void OnJoinRoom(CoreDefine.RT_C_G_Join_Room _packet)
    {
        livePlayerCount++;
    }

    public override void OnOutPlayer(ulong _session_id, bool _is_rejoin, int _rejoin_time)
    {
        livePlayerCount--;
        if (ReadyPlayerCount.Contains(_session_id))
        {
            ReadyPlayerCount.Remove(_session_id);
        }
        if (livePlayerCount == 0)
        {
            EndRoom(null);
        }
    }
}