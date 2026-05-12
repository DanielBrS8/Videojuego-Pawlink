using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadGame()            => SceneManager.LoadScene("GameScene");
    public void LoadTitle()           => SceneManager.LoadScene("TitleScene");
    public void LoadMinijuegosMenu()  => SceneManager.LoadScene("MinijuegosMenuScene");
    public void LoadMinijuegoPesca()  => SceneManager.LoadScene("MinijuegosPescaScene");
    public void LoadMinijuegoEsquiva() => SceneManager.LoadScene("MinijuegosEsquivaScene");
    public void LoadMinijuegoMemory()  => SceneManager.LoadScene("MinijuegosMemoryScene");
}
