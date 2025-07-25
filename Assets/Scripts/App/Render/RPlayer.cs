using System.Collections.Generic;
using System.Linq;
using App.Data;
using App.Data.Definition;
using App.Render.WorldObjects;
using Core;
using UnityEngine;

namespace App.Render
{
    public class RPlayer : IRenderAgent
    {
        Dictionary<string, WOPlayer> worldObjectPlayers = new Dictionary<string, WOPlayer>();

        public void Setup()
        {
            Player[] players = DataTable.FromCollection(DataType.Players).GetAll<Player>();
            foreach (Player p in players)
            {
                worldObjectPlayers.Add(p.Id, new WOPlayer(p.Id));
                worldObjectPlayers[p.Id].SetColor(p.color);
            }
        }

        public void Draw()
        {

            if (worldObjectPlayers == null)
                return;

            Player[] players = DataTable.FromCollection(DataType.Players).GetAll<Player>();

            string[] removedIds = worldObjectPlayers.Keys.Except(players.Select(p => p.Id)).ToArray();

            foreach (string removedId in removedIds)
            {
                worldObjectPlayers[removedId].Destroy();
                worldObjectPlayers.Remove(removedId);
            }

            foreach (Player p in players)
            {
                worldObjectPlayers[p.Id].Position = p.position;
                worldObjectPlayers[p.Id].Rotation = Quaternion.Euler(0, 0, p.rotation);
            }
        }
    }
}