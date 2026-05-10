using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadGame()              => SceneManager.LoadScene("GameScene");
    public void LoadTitle()             => SceneManager.LoadScene("TitleScene");
    public void LoadHub()               => SceneManager.LoadScene("HubScene");
    public void LoadCuidados()          => SceneManager.LoadScene("GameScene");
    public void LoadMinijuegos()        => SceneManager.LoadScene("MinijuegosMenuScene");
    public void LoadTienda()            => SceneManager.LoadScene("TiendaScene");
    public void LoadLogros()            => SceneManager.LoadScene("LogrosScene");
    public void LoadMinijuegoRaton()    => SceneManager.LoadScene("MinijuegoRatonScene");
}
