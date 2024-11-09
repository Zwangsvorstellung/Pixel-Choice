using System.Diagnostics;
using Raylib_cs;

public class SceneOptions : Scene
{
    Input testInput = new Input
    {
        Rect = new Rectangle(GameState.Instance.WidthScreen / 2.0f - 100, 180, 225, 50),
    };

    Button backButton = new Button
    {
        Rect = new Rectangle(10, 90, 200, 40),
        Text = "Retour",
        colorText = Color.White
    };

    Button okButton = new Button
    {
        Rect = new Rectangle(10 + 200 + 5, 90, 200, 40),
        Text = "OK",
        colorText = Color.White
    };

    private ButtonsList buttonsList = new ButtonsList();

    private bool isFullScreen;

    public SceneOptions()
    {
        buttonsList.AddButton(okButton);
        buttonsList.AddButton(backButton);
        buttonsList.AddInput(testInput);
        isFullScreen = GameState.Instance.fullScreen;
    }

    public override void Show()
    {
        base.Show();
    }

    public override void Update()
    {
        buttonsList.Update();

        if (Raylib.IsKeyPressed(KeyboardKey.Right))
        {
            if (GameState.Instance.masterVolume < 1f)
            {
                GameState.Instance.SetVolume(GameState.Instance.masterVolume + .01f);
            }
        }
        if (Raylib.IsKeyPressed(KeyboardKey.Left))
        {
            if (GameState.Instance.masterVolume > 0f)
            {
                GameState.Instance.SetVolume(GameState.Instance.masterVolume - .01f);
            }
        }

        if (Raylib.IsKeyPressed(KeyboardKey.F))
        {
            isFullScreen = !isFullScreen;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Escape))
        {
            GameState.Instance.ChangeScene("menu");
        }

        if (backButton.IsClicked)
        {
            GameState.Instance.ChangeScene("menu");
        }
        else if (okButton.IsClicked)
        {
            // Sauvegarde
            if (isFullScreen != GameState.Instance.fullScreen)
            {
                //Raylib.ToggleFullscreen();
            }
            OptionsFile optionsFile = new OptionsFile();
            //optionsFile.AddOption("bidon", "valeur de test");
            //var testObject = new { mana = 100, arrows = 10, PV = 10 };
            //optionsFile.AddOption("slot1", testObject);

            optionsFile.AddOption("volume", GameState.Instance.masterVolume);
            optionsFile.AddOption("fullscreen", isFullScreen);

            optionsFile.Save();

            GameState.Instance.ChangeScene("menu");
        }
        else if (testInput.IsClicked)
        {

        }

        base.Update();
    }

    public override void Draw()
    {
        Raylib.ClearBackground(Color.Black);

        Raylib.DrawText("OPTIONS", 5, 5, 25, Color.White);
        int screenWidth = Raylib.GetScreenWidth();
        Raylib.DrawLine(0, 30, screenWidth, 30, Color.White);

        int pourcent = (int)(GameState.Instance.masterVolume * 100);
        Raylib.DrawText($"Volume : {pourcent} %", 10, 35, 20, Color.White);

        string bFull = "Non";
        if (isFullScreen)
        {
            bFull = "Oui";
        }
        Raylib.DrawText($"Plein écran : {bFull}", 10, 65, 20, Color.Black);

        buttonsList.Draw();

        base.Draw();
    }

    public override void Hide()
    {
        base.Hide();
    }

    public override void Close()
    {
        Debug.WriteLine("Destruction de la scene options");
        base.Close();
    }
}