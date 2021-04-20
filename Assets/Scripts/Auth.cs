using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Menus;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Auth : MonoBehaviour
{

    [SerializeField] private Text email;
    [SerializeField] private Text password;
    [SerializeField] private Text error;
    
    private static string username = null;
    public static string USERNAME => username;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
            MainMenuManager.Instance.OpenMenu("Main");
        else
            MainMenuManager.Instance.OpenMenu("LoginMenu"); 
    }

    public void Login()
    {
        if(string.IsNullOrEmpty(email.text) || string.IsNullOrEmpty(password.text))
            error.text = "Invalid email or password";
        else
        {
            FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance)
                .SignInWithEmailAndPasswordAsync(email.text, password.text);
        }
    }

    private IEnumerable TrytoLogin(string email, string password)
    { 
        Task<FirebaseUser> registerTask = FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => registerTask.IsCompleted);
        if (registerTask.Exception != null)
            error.text = "Invalid email or password";
        else if (!registerTask.Result.IsEmailVerified)
            error.text = "Your email must be verified to login";
        else
        {
            username = registerTask.Result.DisplayName;
            MainMenuManager.Instance.OpenMenu("Loading");
            Connect();
        }
    }

    void Connect()
    {
        Debug.Log("Connecting to Server...");
        PhotonNetwork.ConnectUsingSettings();
    }

}
