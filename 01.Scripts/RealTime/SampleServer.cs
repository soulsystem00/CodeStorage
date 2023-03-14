using System;
using Google.Protobuf;
using Google.Protobuf.GameProtocol;
using RealTime;
using RealTime.Server;
using UnityEngine;

public class SampleServer : RealTimeServer
{
    public void Init()
    {
        ServerEventHandler handler = new ServerEventHandler();
        handler.RegisterNet<C_G_Start>((UInt16)GamePacketId.CGStart, CGStart);
        handler.RegisterNet<C_G_End>((UInt16)GamePacketId.CGEnd, CGEnd);
        handler.RegisterNet<C_G_Score>((UInt16)GamePacketId.CGScore, CGScore);
        var result = base.Init("Survive", typeof(CustomRoom), handler);
        if (!result)
        {
            Debug.Log("Server Init Error");
        }
    }

    private void CGScore(ulong arg1, CustomRoom _custom_room, IMessage _packet)
    {
        Debug.Log("Get Score");
        var player = (C_G_Score)_packet;

        G_C_Score response = new G_C_Score();
        response.SessionID = player.SessionID;
        response.Score = player.Score;

        _custom_room.Broadcast((UInt16)GamePacketId.GCScore, response);
    }

    public static void CGStart(ulong arg1, CustomRoom _custom_room, IMessage _packet)
    {
        G_C_Start response = new G_C_Start();
        _custom_room.Broadcast((UInt16)GamePacketId.GCStart, response);

    }

    public static void CGEnd(ulong arg1, CustomRoom _custom_room, IMessage _packet)
    {
        //Debug.Log("Get End Message from client");
        G_C_End response = new G_C_End();
        _custom_room.Broadcast((UInt16)GamePacketId.GCEnd, response);
    }
}