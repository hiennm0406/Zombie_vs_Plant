using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using PlayFab.Json;

public static class PF_GameDatas
{     /*
    public static Dictionary<string, int> CharacterLevelExp = new Dictionary<string, int>();
    


    // intfo all character of game
    public static Dictionary<string, HeroesDetail> HeroDetail = new Dictionary<string, HeroesDetail>();

    // intfo all Item of game
    public static Dictionary<string, ItemDetail> ItemDetail = new Dictionary<string, ItemDetail>();


    // this is a sorted, collated structure built from playerInventory. By default, this will only grab items that are in the primary catalog
    public static readonly Dictionary<string, ItemInstance> HeroInInventory = new Dictionary<string, ItemInstance>();
    public static readonly Dictionary<string, List<ItemInstance>> ItemInInventory = new Dictionary<string, List<ItemInstance>>();
    public static readonly Dictionary<string, int> virtualCurrency = new Dictionary<string, int>();
    public static readonly List<ItemInstance> playerInventory = new List<ItemInstance>();
    public static readonly Dictionary<string, int> userStatistics = new Dictionary<string, int>();
    public static readonly Dictionary<string, string> userStage = new Dictionary<string, string>();
    



    #region TitleData
    public static void GetTitleData()
    {
        var request = new GetTitleDataRequest { Keys = GlobalStrings.InitTitleKeys };
        //DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetTitleData_General);
        PlayFabClientAPI.GetTitleData(request, OnGetTitleDataSuccess, PF_Bridge.PlayFabErrorCallback);
    }

    private static void ExtractJsonTitleData<T>(Dictionary<string, string> resultData, string titleKey, ref T output)
    {
        string json;
        if (!resultData.TryGetValue(titleKey, out json))
            Debug.LogError("Failed to load titleData: " + titleKey);
        try
        {
            output = JsonWrapper.DeserializeObject<T>(resultData[titleKey]);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load titleData: " + titleKey);
            Debug.LogException(e);
        }
    }

    private static void OnGetTitleDataSuccess(GetTitleDataResult result)
    {
        Debug.Log("Start get Title");
        ExtractJsonTitleData(result.Data, "CharacterLevelExp", ref CharacterLevelExp);
        ExtractJsonTitleData(result.Data, "CharacterLevelTime", ref CharacterLevelTime);        
        ExtractJsonTitleData(result.Data, "Skills", ref Skills);
        ExtractJsonTitleData(result.Data, "Passive", ref Passive);
        ExtractJsonTitleData(result.Data, "EncounterStage", ref EncounterStage);
        ExtractJsonTitleData(result.Data, "Dialog", ref Dialog);
        ExtractJsonTitleData(result.Data, "ListStage", ref ListStage);

        Debug.Log("Done get Title");

        GameController.RequestRefeshInventory();
        PF_Bridge.RaiseCallbackSuccess("Title Data Loaded", PlayFabAPIMethods.GetTitleData_General, MessageDisplayStyle.none);
    }

    public static void GetCharacterInfo(Action callback = null)
    {
        Debug.Log("Start get Base Character Data");
        GetCatalogItemsRequest request = new GetCatalogItemsRequest();
        request.CatalogVersion = "Character";
        PlayFabClientAPI.GetCatalogItems(request, result =>
        {
            List<CatalogItem> catalog = result.Catalog;
            foreach (CatalogItem i in catalog)
            {                                
                HeroDetail.Add(i.ItemId, JsonUtility.FromJson<HeroesDetail>(i.CustomData));
            }
            Debug.Log("End get Base Character Data");
            GameController.RequestRefeshInventory();
            if (callback != null)
                callback();
        }, PF_Bridge.PlayFabErrorCallback);
    }

    public static void GetItemInfo(Action callback = null)
    {
        Debug.Log("Start get Item Data");
        GetCatalogItemsRequest request = new GetCatalogItemsRequest();
        request.CatalogVersion = "Items";
        PlayFabClientAPI.GetCatalogItems(request, result =>
        {
            List<CatalogItem> catalog = result.Catalog;
            foreach (CatalogItem i in catalog)
            {
                if(i.ItemId != "Gold" || i.ItemId != "RuneFrag")
                    ItemDetail.Add(i.ItemId, JsonUtility.FromJson<ItemDetail>(i.CustomData));
            }
            Debug.Log("End get Item Data");
            GameController.RequestRefeshInventory();
            if (callback != null)
                callback();
        }, PF_Bridge.PlayFabErrorCallback);
    }

    #endregion

    #region PlayerData
    public static void GetUserInventory(Action callback = null)
    {
        Debug.Log("Start get User Inventory");
        //DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetUserInventory);
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), (GetUserInventoryResult result) =>
        {
            virtualCurrency.Clear();
            playerInventory.Clear();            
            foreach (var pair in result.VirtualCurrency)
            {
                virtualCurrency.Add(pair.Key, pair.Value);                
            }
         
            foreach (var eachItem in result.Inventory)
            {
                playerInventory.Add(eachItem);
            
            }
            HeroInInventory.Clear();            
            ItemInInventory.Clear();            


            foreach (var item in playerInventory)
            {
                if (item.ItemClass != "Hero")
                {
                    if (!ItemInInventory.ContainsKey(item.CatalogVersion))
                    {
                        List<ItemInstance> cat = new List<ItemInstance>();
                        cat.Add(item);
                        ItemInInventory.Add(item.CatalogVersion, cat);
                    }
                    else
                    {
                        ItemInInventory[item.CatalogVersion].Add(item);
                    }
                }
                else
                {
                    HeroInInventory.Add(item.ItemId,item);
                }
            }

           

            if (callback != null)
                callback();
            Debug.Log("Done get Inventory");            
            PF_Bridge.RaiseCallbackSuccess("", PlayFabAPIMethods.GetUserInventory, MessageDisplayStyle.none);
            PlayerData.getCharacter();
        }, PF_Bridge.PlayFabErrorCallback);
    }
    #endregion

    #region PlayerTitle
    public static void GetUserReadOnlyData(Action callback = null)
    {
        Debug.Log("Start get Stage");
        PlayFabClientAPI.GetUserReadOnlyData(new GetUserDataRequest(),
        result => {
           foreach(KeyValuePair<string, UserDataRecord> data in result.Data)
            {
                userStage.Add(data.Key, data.Value.Value);
            }


            stageDone = JsonWrapper.DeserializeObject<StageDone>(PF_GameDatas.userStage["Stage"]);
            if (callback != null)
                callback();
        },
        error => {
            PF_Bridge.RaiseCallbackError(error.ErrorMessage, PlayFabAPIMethods.GetUserReadOnlyData, MessageDisplayStyle.error);
        });
    }
    #endregion

    #region User Statistics
    public static void GetUserStatistics()
    {
        GetPlayerStatisticsRequest request = new GetPlayerStatisticsRequest();
        PlayFabClientAPI.GetPlayerStatistics(request, OnGetUserStatisticsSuccess, OnGetUserStatisticsError);
    }

    private static void OnGetUserStatisticsSuccess(GetPlayerStatisticsResult result)
    {
        //TODO update to use new 

        PF_Bridge.RaiseCallbackSuccess("", PlayFabAPIMethods.GetUserStatistics, MessageDisplayStyle.none);
        foreach (var each in result.Statistics)
            userStatistics[each.StatisticName] = each.Value;
    }

    private static void OnGetUserStatisticsError(PlayFabError error)
    {
        PF_Bridge.RaiseCallbackError(error.ErrorMessage, PlayFabAPIMethods.GetUserStatistics, MessageDisplayStyle.error);
    }

    public static void UpdateUserStatistics(Dictionary<string, int> updates)
    {
        var request = new UpdatePlayerStatisticsRequest();
        request.Statistics = new List<StatisticUpdate>();

        foreach (var eachUpdate in updates) // Copy the stats from the inputs to the request
        {
            int eachStat;
            userStatistics.TryGetValue(eachUpdate.Key, out eachStat);
            request.Statistics.Add(new StatisticUpdate { StatisticName = eachUpdate.Key, Value = eachUpdate.Value }); // Send the value to the server
            userStatistics[eachUpdate.Key] = eachStat + eachUpdate.Value; // Update the local cache so that future updates are using correct values
        }

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnUpdateUserStatisticsSuccess, OnUpdateUserStatisticsError);
    }

    private static void OnUpdateUserStatisticsSuccess(UpdatePlayerStatisticsResult result)
    {
        PF_Bridge.RaiseCallbackSuccess("User Statistics Loaded", PlayFabAPIMethods.UpdateUserStatistics, MessageDisplayStyle.none);        
    }

    private static void OnUpdateUserStatisticsError(PlayFabError error)
    {
        PF_Bridge.RaiseCallbackError(error.ErrorMessage, PlayFabAPIMethods.UpdateUserStatistics, MessageDisplayStyle.error);
    }
    #endregion

    */
}
