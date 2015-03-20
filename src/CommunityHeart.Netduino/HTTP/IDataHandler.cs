using System;
using Microsoft.SPOT;

namespace CommunityHeart.Netduino.HTTP
{
    public interface IDataHandler
    {
        bool Initialize();
        string Data
        {
            get;
        }
    }
}
