using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOracle2.UserContent
{
    public class GameItem
    {
        public GameItem()
        {

        }

        public int GameItemId { get; set; }
        public string Name { get; set; }
        public ICollection<User> SubscribedUsers { get; set; } = new List<User>();
    }
}
