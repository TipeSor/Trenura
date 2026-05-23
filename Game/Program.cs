using System.Numerics;
using Engine;
using Raylib_cs;

Window win = new Window(640, 960, "Cool Game", new Color(113, 69, 55, 220));

TextureManager.AddEmbeddedTexture("ground", "Assets/Textures/Ground.png");
TextureManager.AddEmbeddedTexture("coin", "Assets/Textures/Coin.png");

TextureManager.AddEmbeddedTexture("ht_coin", "Assets/Textures/CoinSheet.png");
TextureManager.AddEmbeddedTexture("spinner", "Assets/Textures/SpinnerSheet.png");

TilemapManager.AddEmbeddedTilemap("level_1", "Assets/Tilemap/Level1.xml");
TilemapManager.AddEmbeddedTilemap("level_2", "Assets/Tilemap/Level2.xml");
TilemapManager.AddEmbeddedTilemap("level_3", "Assets/Tilemap/Level3.xml");
TilemapManager.AddEmbeddedTilemap("level_4", "Assets/Tilemap/Level4.xml");
TilemapManager.AddEmbeddedTilemap("level_5", "Assets/Tilemap/Level5.xml");
TilemapManager.AddEmbeddedTilemap("level_6", "Assets/Tilemap/Level6.xml");

GameManager.Window = win;

win.AddScene(new MainMenu());
win.AddScene(new Level1());
win.AddScene(new Level2());
win.AddScene(new Level3());
win.AddScene(new TutorialSubmit());
win.AddScene(new Level4());
win.AddScene(new Level5());
win.AddScene(new Level6());
win.AddScene(new LevelSpinner());
win.AddScene(new CoinFlip());
win.AddScene(new RunSummary());

// win.CurrentSceneIndex = GameManager.SceneIndex.CoinFlip;

win.Run();
