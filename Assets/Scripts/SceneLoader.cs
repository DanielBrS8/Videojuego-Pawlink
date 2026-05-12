using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
<<<<<<< HEAD
    public void LoadGame()            => SceneManager.LoadScene("GameScene");
    public void LoadTitle()           => SceneManager.LoadScene("TitleScene");
    public void LoadMinijuegosMenu()  => SceneManager.LoadScene("MinijuegosMenuScene");
    public void LoadMinijuegoPesca()  => SceneManager.LoadScene("MinijuegosPescaScene");
    public void LoadMinijuegoEsquiva() => SceneManager.LoadScene("MinijuegosEsquivaScene");
    public void LoadMinijuegoMemory()  => SceneManager.LoadScene("MinijuegosMemoryScene");
=======
    public void LoadGame()              => SceneManager.LoadScene("GameScene");
    public void LoadTitle()             => SceneManager.LoadScene("TitleScene");
    public void LoadHub()               => SceneManager.LoadScene("HubScene");
    public void LoadCuidados()          => SceneManager.LoadScene("GameScene");
    public void LoadMinijuegos()        => SceneManager.LoadScene("MinijuegosMenuScene");
    public void LoadTienda()            => SceneManager.LoadScene("TiendaScene");
    public void LoadLogros()            => SceneManager.LoadScene("LogrosScene");
    public void LoadMinijuegoRaton()    => SceneManager.LoadScene("MinijuegoRatonScene");
>>>>>>> e0c651cabc38e6ede1a7f82693974f0d78ae4d2e
}
