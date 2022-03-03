using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public static class Globals
    {
        public static string GetEnumAsName<EnumType>(EnumType value)
        {
            return Enum.GetName(typeof(EnumType), value);
        }

        public static int GetPlayerId(PlayerTags player)
        {
            return (int)player;
        }

        public static int GetPlayerId(String token)
        {
            if (false == token.Contains('_'))
                throw new ArgumentException($"Param '{nameof(token)}' is not in <string>_<int> format! '{nameof(token)}' is: {token}");

            return Convert.ToInt32(token.Split('_')[1]);
        }
    }
}
