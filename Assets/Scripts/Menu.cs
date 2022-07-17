using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private Slider volume;

    public void Versus()
    {
        Audio.GetInstance().PlaySound("UI", "error_style_1_001"); // TODO Load game scene
    }

    public void Challenge() {
        Audio.GetInstance().PlaySound("UI", "error_style_1_001");
    }

    public void Credits()
    {
        Audio.GetInstance().PlaySound("UI", "confirm_style_1_001");
    }

    public void Options()
    {
        Audio.GetInstance().PlaySound("UI", "confirm_style_1_001");
    }

    public void Volume()
    {
        Audio.GetInstance().Volume(volume.value);
        PlayerPrefs.SetFloat("volume", volume.value);
    }

    public void Back()
    {
        Audio.GetInstance().PlaySound("UI", "back_style_1_001");
    }

    public void Quit()
    {
        Audio.GetInstance().PlaySound("UI", "confirm_style_1_001");
        Application.Quit();
    }

    private void Start()
    {
        volume.value = PlayerPrefs.GetFloat("volume");
        Audio.GetInstance().PlayMusic("sadpiano");
    }
}
