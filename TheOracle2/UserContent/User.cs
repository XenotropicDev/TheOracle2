using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOracle2.UserContent
{
    public class User
    {
        public static User GetUser(ulong userId, EFContext context)
        {
            var user = context.Users.Find(userId);
            if (user != null) return user;

            user = new User() {UserId = userId };
            context.Users.Add(user);
            return user;
        }
        public ulong UserId { get; private set; }

        public ICollection<GameItem> GameItems { get; set; } = new List<GameItem>();
        public ulong UserID { get; }
    }
}
