using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Menus;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Auth : MonoBehaviour
{

    [SerializeField] private InputField email;
    [SerializeField] private InputField password;
    [SerializeField] private Text error;
    
    [SerializeField] private Text welcomeMessage;
    
    private static string _username = null;
    public static string USERNAME => _username;

    // Start is called before the first frame update
    void Start()
    {
        InitDiscordRPC();
        if (PhotonNetwork.IsConnected)
        {
            MainMenuManager.Instance.OpenMenu("Main");
            welcomeMessage.text = $"Welcome,\n{_username}";
        }
        else if (PlayerPrefs.HasKey("TokenFirebase"))
            StartCoroutine(TrytoLogin(PlayerPrefs.GetString("TokenFirebase")));
        else
            MainMenuManager.Instance.OpenMenu("LoginMenu"); 
    }

    public void Login()
    {
        if(string.IsNullOrEmpty(email.text) || string.IsNullOrEmpty(password.text))
            error.text = "Invalid email or password";
        else
        {
            StartCoroutine(TrytoLogin(email.text, password.text));
        }
    }

    private IEnumerator TrytoLogin(string email, string password)
    { 
        Task<FirebaseUser> registerTask = FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => registerTask.IsCompleted);
        this.password.text = string.Empty;
        if (registerTask.Exception != null)
            error.text = registerTask.Exception.GetBaseException().Message;
        else if (!registerTask.Result.IsEmailVerified)
        {
            error.text = "Your email must be verified to login";
            registerTask.Result.SendEmailVerificationAsync();
        }
        else
        {
            ResetField();
            MainMenuManager.Instance.OpenMenu("Loading");
            
            if (string.IsNullOrEmpty(registerTask.Result.DisplayName))
            {
                _username = "User " + new System.Random().Next(10000);
                registerTask.Result.UpdateUserProfileAsync(new UserProfile {DisplayName = _username});
            }
            else _username = registerTask.Result.DisplayName;

            welcomeMessage.text = $"Welcome,\n{_username}";
            PlayerPrefs.SetString("TokenFirebase", email + "|" + password);
            
            Connect();
        }
    }

    private IEnumerator TrytoLogin(string token)
    {
        string[] pass = token.Split('|');
        if (pass.Length >= 2)
        {
            string email = pass[0];
            pass[0] = "";
            string password = string.Join("|", pass).Substring(1);
            Task<FirebaseUser> registerTask = FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil(() => registerTask.IsCompleted);
            if (registerTask.Exception == null)
            {
                _username = registerTask.Result.DisplayName;
                welcomeMessage.text = $"Welcome,\n{_username}";
                Connect();
                yield break;
            }
        }
        MainMenuManager.Instance.OpenMenu("LoginMenu");
        PlayerPrefs.SetString("TokenFirebase", null);
    }

    private void ResetField()
    {
        email.text = string.Empty;
        password.text = string.Empty;
        error.text = string.Empty;
    }

    void Connect()
    {
        Debug.Log("Connecting to Server...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Logout()
    {
        PlayerPrefs.SetString("TokenFirebase", null);
        FirebaseAuth.DefaultInstance.SignOut();
        PhotonNetwork.Disconnect();
        MainMenuManager.Instance.OpenMenu("LoginMenu");
        welcomeMessage.text = string.Empty;
    }

    public void InitDiscordRPC()
    {
        var handlers = new DiscordRpc.EventHandlers();
        DiscordRpc.Initialize("844697540087382026", ref handlers, true, string.Empty);
        DiscordRpc.RichPresence presence = new DiscordRpc.RichPresence();
        presence.largeImageKey = "icon";
        presence.largeImageText = Application.version;
        presence.state = "In Menu";
        presence.startTimestamp = 0;
        DiscordRpc.UpdatePresence(presence);
    }

    public void ReadyCallback()
    {
        
    }
    
    public void CreateAccount()
    {
        Application.OpenURL("https://fric-frac.fr/signup");
    }
    
    public void ForgotPassword()
    {
        Application.OpenURL("https://fric-frac.fr/reset");
    }
}
