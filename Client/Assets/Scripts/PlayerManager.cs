using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    MyPlayer _myPlayer;
    Dictionary<int, Player> _players = new Dictionary<int, Player>();
    public static PlayerManager Instance { get; } = new PlayerManager();

    public void Add(S_PlayerList packet)
    {
        Object obj = Resources.Load("Player");

        foreach(S_PlayerList.Player p in packet.players)
        {
            GameObject go = Object.Instantiate(obj) as GameObject;

            if (p.isSelf)
            {
                MyPlayer myPlayer = go.AddComponent<MyPlayer>();
                myPlayer.PlayerID = p.playerID;
                myPlayer.transform.position = new Vector3(p.posX, p.posY, p.posZ);
                _myPlayer = myPlayer;
            }
            else
            {
                Player player = go.AddComponent<Player>();
                player.PlayerID = p.playerID;
                player.transform.position = new Vector3(p.posX, p.posY, p.posZ);
                _players.Add(p.playerID, player);
            }
        }
    }

    public void EnterGame(S_BroadCastEnterGame packet)
    {
        if (packet.playerID == _myPlayer.PlayerID)
            return;
        Object obj = Resources.Load("Player");
        GameObject go = Object.Instantiate(obj) as GameObject;

        Player player = go.AddComponent<Player>();
        player.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
        _players.Add(packet.playerID, player);
    }

    public void LeaveGame(S_BroadCastLeaveGame packet)
    {
        if(_myPlayer.PlayerID == packet.playerID)
        {
            GameObject.Destroy(_myPlayer.gameObject);
            _myPlayer = null;
        }
        else
        {
            Player player = null;
            if(_players.TryGetValue(packet.playerID, out player))
            {
                GameObject.Destroy(player.gameObject);
                _players.Remove(packet.playerID);
            }
        }
    }

    public void Move(S_BroadCastMove packet)
    {
        if (_myPlayer.PlayerID == packet.playerID)
        {
            _myPlayer.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
        }
        else
        {
            Player player = null;
            if (_players.TryGetValue(packet.playerID, out player))
            {
                player.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
            }
        }
    }



}
