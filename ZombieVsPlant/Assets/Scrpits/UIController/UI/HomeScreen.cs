using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeScreen : AnimatedPanel
{
    

    public void GoMap()
    {
        GameGUI.instance.PushPanel("MapScreen");
    }

    public void GoExtras()
    {
        GameGUI.instance.PushPanel("ExtrasScreen");
    }
    public void GoLeaderboard()
    {
        GameGUI.instance.PushPanel("LeaderboardScreen");
    }
    public void GoShop()
    {
        GameGUI.instance.PushPanel("ShopScreen");
    }
    public void GoArchiment()
    {
        GameGUI.instance.PushPanel("ArchimentScreen");
    }

}
