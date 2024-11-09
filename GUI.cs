using System.Diagnostics;
using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;
public class Button
{
    public Rectangle Rect { get; set; }
    public string Text { get; set; } = "";
    public int FontSize { get; set; }
    public bool IsClicked { get; set; } = false;
    public Color colorTransparent = new Color(0, 0, 0, 0);
    public Color colorText = Color.White;

}

public class Input()
{
    public int letterCount = 0;
    public bool mouseOnText = false;
    public int framesCounter = 0;
    public Rectangle Rect { get; set; }
    public bool IsClicked { get; set; } = false;
    public Color color = Color.Red;
    public char MAX_INPUT_CHARS = '9';
    //char name[MAX_INPUT_CHARS + 1] = "\0";      // NOTE: One extra space required for null terminator char '\0'
    public char[] chars = new char[] { '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0' };
}

public class ButtonsList
{
    private List<Button> buttons = new List<Button>();
    private List<Input> inputs = new List<Input>();
    public Color colorTransparent = new Color(0, 0, 0, 0);

    public void AddButton(Button button)
    {
        button.FontSize = 30;
        button.colorText = Color.White;
        buttons.Add(button);
    }

    public void AddInput(Input input)
    {
        inputs.Add(input);
    }

    public void Update()
    {
        foreach (Button button in buttons)
        {
            button.IsClicked = false;
            if (Raylib.CheckCollisionPointRec(GetMousePosition(), button.Rect))
            {
                button.FontSize = 40;
                button.colorText = Color.SkyBlue;
                if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    button.IsClicked = true;
                }
            }
            else
            {
                button.FontSize = 30;
                button.colorText = Color.White;
            }
        }

        foreach (Input input in inputs)
        {
            input.IsClicked = false;
            if (CheckCollisionPointRec(GetMousePosition(), input.Rect)) input.mouseOnText = true;
            else input.mouseOnText = false;

            if (input.mouseOnText)
            {
                // Set the window's cursor to the I-Beam
                SetMouseCursor(MouseCursor.IBeam);

                // Get char pressed (unicode character) on the queue
                int key = GetCharPressed();

                while (key > 0)
                {
                    // NOTE: Only allow keys in range [32..125]
                    if ((key >= 32) && (key <= 125) && (input.letterCount < input.MAX_INPUT_CHARS))
                    {
                        input.chars[input.letterCount] = (char)key;
                        input.chars[input.letterCount + 1] = '\0'; // Add null terminator at the end of the string.
                        input.letterCount++;
                    }
                    key = GetCharPressed();  // Check next character in the queue
                }

                if (IsKeyPressed(KeyboardKey.Backspace))
                {
                    input.letterCount--;
                    if (input.letterCount < 0) input.letterCount = 0;
                    input.chars[input.letterCount] = '\0';
                }

                input.color = Color.SkyBlue;
                if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    input.IsClicked = true;
                }
            }
            else SetMouseCursor(MouseCursor.Default);


            if (input.mouseOnText) input.framesCounter++;
            else input.framesCounter = 0;
            input.color = Color.Blue;

        }
    }

    public void Draw()
    {
        foreach (Button button in buttons)
        {
            Raylib.DrawRectangleRec(button.Rect, colorTransparent);
            //Raylib.DrawRectangleLinesEx(button.Rect, 2, Color.Red);
            Raylib.DrawText(button.Text, Funcs.CenterElement(100, "X"), (int)button.Rect.Y, button.FontSize, button.colorText);
        }

        foreach (Input input in inputs)
        {
            DrawText("PLACE MOUSE OVER INPUT BOX!", 240, 140, 20, Color.Beige);


            DrawRectangleRec(input.Rect, Color.LightGray);
            if (input.mouseOnText) DrawRectangleLines((int)input.Rect.X, (int)input.Rect.Y, (int)input.Rect.Width, (int)input.Rect.Height, Color.Red);
            else DrawRectangleLines((int)input.Rect.X, (int)input.Rect.Y, (int)input.Rect.Width, (int)input.Rect.Height, Color.DarkGray);

            DrawText(input.chars[0].ToString(), (int)input.Rect.X + 5, (int)input.Rect.Y + 8, 40, Color.Maroon);

            DrawText("INPUT CHARS:" + input.letterCount + " / " + input.MAX_INPUT_CHARS, 315, 250, 20, Color.DarkGray);

            if (input.mouseOnText)
            {
                if (input.letterCount < input.MAX_INPUT_CHARS)
                {
                    // Draw blinking underscore char
                    //if (((input.framesCounter / 20) % 2) == 0)
                    //    DrawText("_", (int)input.Rect.X + 8 + MeasureText(input.chars, 40), (int)input.Rect.Y + 12, 40, Color.Maroon);
                }
                else DrawText("Press BACKSPACE to delete chars...", 230, 300, 20, Color.Gray);
            }
        }
    }
}
