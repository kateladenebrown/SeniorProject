using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameEF;

namespace TurnBasedGameAPI.ViewModels
{
    public class GameDetails
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public List<SimpleGameUser> Users { get; set; }

        public GameDetails(Game game)
        {
            Id = game.ID;
            Status = game.Status;
            Start = game.Start.ToShortDateString();
            End = (game.End != null) ? ((DateTime)game.End).ToShortDateString() : null;
            Users = game.GameUsers.ToList().Select(x => new SimpleGameUser(x)).ToList();
        }
    }

    public class SimpleGameUser
    {
        public string UserName { get; set; }
        public int Status { get; set; }

        public SimpleGameUser(GameUser gu)
        {
            UserName = gu.AspNetUser.UserName;
            Status = gu.Status;
        }
    }
}