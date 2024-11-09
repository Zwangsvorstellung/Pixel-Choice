using Raylib_cs;
using System.Diagnostics;
using static Raylib_cs.Raylib;

public class SceneGameplay : Scene
{
    private float timer;
    Color colorLifeIndicator = Color.White;
    Map Map = new Map();
    Monster Monster = new Monster();
    Anim AnimText = new Anim(0, 0, "", Color.Black);
    Anim Anim = new Anim(0, 0);
    Snake Snake = new Snake();
    JellyFish Jelly = new JellyFish();
    Crabe Crabe = new Crabe();
    Personnage heroCurrent = new Snake();
    Random rnd = new Random();
    private static Dictionary<string, object> listHeros = new Dictionary<string, object>();
    Color colorCase = Color.Black;

    static int[,] gridGame = new int[10, 18] {
        {3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3},
        {3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3},
        {3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3},
        {3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3},
        {3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3},
        {3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3},
        {3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3},
        {3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3},
        {3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3},
        {3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3},
    };

    public SceneGameplay()
    {
        listHeros["Le Serpent"] = Snake;
        listHeros["La Méduse"] = Jelly;
        listHeros["Le Crabe"] = Crabe;
        GameState.Instance.debugMagic.Clear();
    }

    public override void Update()
    {
        if (GameState.Instance.persoChoice == "Le Serpent")
        {
            heroCurrent = Snake;
        }
        else if (GameState.Instance.persoChoice == "La Méduse")
        {
            heroCurrent = Jelly;
        }
        else if (GameState.Instance.persoChoice == "Le Crabe")
        {
            heroCurrent = Crabe;
        }
        else if (GameState.Instance.persoChoice == "Le Hasard")
        {
            GameState.Instance.persoChoice = listHeros.ElementAt(rnd.Next(0, listHeros.Count())).Key;
            heroCurrent = (Personnage)listHeros[GameState.Instance.persoChoice];
        }

        timer += 0.5f;

#if DEBUG
        GameState.Instance.debugMagic.AddOption("FPS", Raylib.GetFPS());
        GameState.Instance.debugMagic.AddOption("Le timer", timer);
        GameState.Instance.debugMagic.AddOption("Nom", this.name);
#endif

        if (Raylib.IsKeyPressed(KeyboardKey.Escape))
        {
            GameState.Instance.ChangeScene("menu");
        }
        else if (Raylib.IsKeyPressed(KeyboardKey.Enter))
        {
            GameState.Instance.ChangeScene("pause");
        }
        else if (heroCurrent.mouvementCaseCount <= 0)
        {
            resetGame(heroCurrent, AnimText, Map);
            GameState.Instance.ChangeScene("gameover");
        }
        else if (heroCurrent.ChangeMap)
        {
            resetGameChangeMap(heroCurrent, AnimText, Map);
        }


        // Position du héros sur la grille
        Rectangle recHero = new Rectangle(heroCurrent.posHeros.X, heroCurrent.posHeros.Y, 64, heroCurrent.textureHero.Height);

        heroCurrent.color = heroCurrent.colorDefault;
        heroCurrent.isContact = false;

        // check collision avec les objets
        foreach (var obj in Object.listObjet)
        {
            Rectangle zoneObject = new Rectangle(obj.posObject.X, obj.posObject.Y, 64, obj.textureObject.Height);
            if (CheckCollisionRecs(recHero, zoneObject))
            {
                if (obj.posObject.X >= heroCurrent.posHeros.X)
                {
                    heroCurrent.isContact = true;
                    GameState.Instance.SetScore(obj.value);
                    GameState.Instance.SetobjectTotal();

                    Object.listObjet.Remove(obj);
                    AnimText.posObject.X = obj.posObject.X;
                    AnimText.posObject.Y = obj.posObject.Y;
                    AnimText.color = Color.White;
                    AnimText.text = "+" + obj.value;

                    Anim.posObject.X = obj.posObject.X;
                    Anim.posObject.Y = obj.posObject.Y;
                    break;
                }
            }
        }

        // check collision avec les monstre
        Rectangle zoneMonster = new Rectangle(Monster.positionMonster.X, Monster.positionMonster.Y, Monster.textureMonster.Width, Monster.textureMonster.Height);
        if (CheckCollisionRecs(recHero, zoneMonster))
        {
            heroCurrent.color = heroCurrent.colorCollision;
            heroCurrent.mouvementCaseCount = heroCurrent.mouvementCaseCount - 1;
        }

        updateMapFog(gridGame, recHero, heroCurrent.fogCapacity, heroCurrent.deplacementMap);

        Map.Update();
        Monster.Update();
        heroCurrent.Update();
        AnimText.Update();
        Anim.Update();
        base.Update();
    }

    public override void Draw()
    {
        ClearBackground(Color.Black);
        Map.Draw();
        Monster.Draw();
        heroCurrent.Draw();

        initMapFog(gridGame, colorCase);
        Map.DrawDecors();
        // info texte du drop
        AnimText.Draw();
        Anim.Draw();

        if (heroCurrent.mouvementCaseCount < 10) colorLifeIndicator = Color.Red;
        else
            colorLifeIndicator = Color.White;

        DrawText("Score : " + GameState.Instance.score, Funcs.CenterElement(200, "X") - 150, 5, 32, Color.White);
        DrawText("Déplacement : " + heroCurrent.mouvementCaseCount, GameState.Instance.WidthScreen - 600, 10, 30, colorLifeIndicator);
        DrawText("Map : " + GameState.Instance.mapTotal, GameState.Instance.WidthScreen - 160, 35, 20, Color.White);
        DrawText("Objets : " + GameState.Instance.objectTotal, GameState.Instance.WidthScreen - 160, 60, 20, Color.White);

        base.Draw();
    }

    public override void Show()
    {
        base.Show();
    }

    public static void resetGameChangeMap(Personnage perso, Anim textDrop, Map map)
    {
        resetFogGrid();
        Object.listObjet.Clear();
        Object.generateObjects();
        perso.initPositionHero();
        perso.initMaxMouvementCount();
        textDrop.resetText();
        perso.ChangeMap = false;
        map.generateRandomExit();
    }

    public static void resetGame(Personnage perso, Anim textDrop, Map map)
    {
        resetFogGrid();
        Object.listObjet.Clear();
        Object.generateObjects();
        perso.initPositionHero();
        perso.initMaxMouvementCount();
        GameState.Instance.ResetStats();
        textDrop.resetText();
        map.generateRandomExit();
    }
    private void initMapFog(int[,] tabInt, Color colorCase)
    {
        int x = 64;
        int y = 192;
        int l, c; //indice vertical et horizontal de tabInt
        for (l = 0; l < tabInt.GetLength(0); l++)
        {
            for (c = 0; c < tabInt.GetLength(1); c++)
            {
                Rectangle frameRec = new Rectangle(x, y, 64, 64);
                if (tabInt[l, c] == 3)
                    colorCase = Color.Black;
                else if (tabInt[l, c] == 2)
                    colorCase = Fade(Color.Black, 0.8f);
                else
                    colorCase = new Color(0, 0, 0, 0);

                DrawRectangleRec(frameRec, colorCase);
                x = x + 64;
            }
            y = y + 64;
            x = 64;
        }
    }

    private void updateMapFog(int[,] tabInt, Rectangle positionHero, int heroCapacityFog, int heroDeplacementMap)
    {
        int x = 64;
        int y = 192;
        int l, c; //indice vertical et horizontal de tabInt
        for (l = 0; l < tabInt.GetLength(0); l++)
        {
            for (c = 0; c < tabInt.GetLength(1); c++)
            {
                Rectangle frameRec = new Rectangle(x, y, 64, 64);
                if (CheckCollisionRecs(positionHero, frameRec))
                {
                    // case ou le heros se dirige
                    tabInt[l, c] = 0;

                    // case devant lui
                    if (c + 1 < tabInt.GetLength(1) - 1)
                    {
                        tabInt[l, c + 1] = 2;
                    }
                    // case au dessous de lui
                    if (l + 1 < tabInt.GetLength(0) && tabInt[l + 1, c] == 3)
                    {
                        tabInt[l + 1, c] = 2;
                    }
                    // case au dessus de lui
                    if (l - 1 >= 0 && tabInt[l - 1, c] == 3)
                    {
                        tabInt[l - 1, c] = 2;
                    }

                    // 2 cases en dessous de lui pour heroCapacityFog 2
                    if (heroCapacityFog == 2)
                    {
                        // la diagonale en dessous
                        if (l + 1 < tabInt.GetLength(0) && c + 1 < tabInt.GetLength(1) && tabInt[l + 1, c] + 1 == 3)
                        {
                            tabInt[l + 1, c + 1] = 2;
                        }
                        // la diagonale au dessus
                        if (l - 1 > 0 && c + 1 < tabInt.GetLength(1))
                        {
                            tabInt[l - 1, c + 1] = 2;
                        }
                    }
                }
                x = x + heroDeplacementMap;
            }
            y = y + heroDeplacementMap;
            x = heroDeplacementMap;
        }
    }

    static public void resetFogGrid()
    {
        int l, c;
        for (l = 0; l < gridGame.GetLength(0); l++)
        {
            for (c = 0; c < gridGame.GetLength(1); c++)
            {
                gridGame[l, c] = 3;
            }
        }
    }
}