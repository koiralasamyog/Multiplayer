//using AtomicConsole.Engine;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;


public class TestLobby : MonoBehaviour
{

    private Lobby hostLobby;
    private float heartbeatTimer;
    private async void Start()
    {
        await UnityServices.InitializeAsync();


        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in : " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update()
    {
        HandleLobbyHeartBeat();
    }

    private async void HandleLobbyHeartBeat()
    {
        if (hostLobby!=null)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15;
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    [AtomicConsole.AtomicCommand]
    private async void CreateLobby()
    {
        try
        {
            string lobbyName = "MyLobby";
            int maxPlayers = 4;

            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = true,
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);

            hostLobby = lobby;

            Debug.Log("Created Lobby : " + lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Id + " " + lobby.LobbyCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [AtomicConsole.AtomicCommand]
    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.MaxPlayers, "5", QueryFilter.OpOptions.EQ)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };
            

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            Debug.Log("Lobbies Found: " + queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log($"{lobby.Name} {lobby.MaxPlayers}");
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [AtomicConsole.AtomicCommand]
    private async void JoinLobby(string lobbyCode)
    {
        try
        {
            await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            Debug.Log("Joined lobby with lobby code" + lobbyCode);
        }

        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [AtomicConsole.AtomicCommand]
    private async void QuickJoinLobby()
    {
        await LobbyService.Instance.QuickJoinLobbyAsync();
    }

    [AtomicConsole.AtomicCommand]
    public void HelloWorld()
    {
        Debug.Log("Hello World");
    }
}

