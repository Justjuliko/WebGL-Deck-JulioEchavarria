using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class HttpGet : MonoBehaviour
{
    public int UserId = 2;
    public string url = "https://my-json-server.typicode.com/justjuliko/jsonDB";
    public string apiRickAndMorty = "https://rickandmortyapi.com/api/character";

    [SerializeField]
    private TMP_Text UsernameLabel;
    [SerializeField]
    private RawImage[] MyDeck;
    [SerializeField]
    private TMP_Text[] MyDeckLabes;
    [SerializeField]
    private TMP_Text[] DeckNumber;
    private User MyUser;

    public void SendRequest()
    {
        UserId = UnityEngine.Random.Range(1, 5);
        StartCoroutine(GetUser());
    }
    IEnumerator GetUser()
    {
        UnityWebRequest request = UnityWebRequest.Get(url+"/users/"+UserId);
        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debug.Log("NETWORK ERROR:" + request.error);
        }
        else
        {

            if (request.responseCode == 200)
            {
                MyUser = JsonUtility.FromJson<User>(request.downloadHandler.text);

                UsernameLabel.text = MyUser.username;



                for(int i = 0; i < MyUser.deck.Length; i++)
                {
                    StartCoroutine(GetCharacter(i));
                    StartCoroutine(GetDeck(i));
                }

            }
            else
            {
                Debug.Log(request.error);
            }
        }

        IEnumerator GetCharacter(int index)
        {
            int characterID = MyUser.deck[index];
            UnityWebRequest request = UnityWebRequest.Get(apiRickAndMorty + "/" + characterID);
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                Debug.Log(request.error);
            }
            else {            
                if(request.responseCode == 200)
                {
                    Character character = JsonUtility.FromJson<Character>(request.downloadHandler.text);

                    StartCoroutine(DownloadImage(character.image, MyDeck[index]));
                    MyDeckLabes[index].text = character.name;
                }
                else
                {
                    Debug.Log(request.error);
                }
            }

        }

        IEnumerator DownloadImage(string MediaUrl,RawImage image)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
            yield return request.SendWebRequest();

            if(request.isNetworkError)
            {
                Debug.Log(request.error);
            }
            else if(!request.isHttpError)
            {
                image.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            }
        }

        IEnumerator GetDeck(int index)
        {
            int CardID = MyUser.deck[index];
            UnityWebRequest request = UnityWebRequest.Get(url + "/users/" + UserId);
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                Debug.Log("NETWORK ERROR:" + request.error);
            }
            else
            {
                if (request.responseCode == 200)
                {
                    MyUser = JsonUtility.FromJson<User>(request.downloadHandler.text);

                    DeckNumber[index].text  = MyUser.deck[index].ToString();
                }
            }

        }
    }

    
}
[System.Serializable]
public class UserList
{
    public List<User> users;
}

[System.Serializable]
public class User
{
    public int id;
    public string username;
    public int[] deck;
}

public class Character
{
    public int id;
    public string name;
    public string image;
}
