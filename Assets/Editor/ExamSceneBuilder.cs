using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ThreeInOne.Editor
{
    // Builds the four exam scenes from the reused class games:
    //   MainMenu    - new menu matching the exam paper
    //   DrivingGame - Prototype 1 (Mad Driver)
    //   FlyingGame  - Challenge 1 (Fly Like a Bird)
    //   SumoGame    - Prototype 4 (I'm a Sumo and a Ball)
    // and adds the In-Game (pause) Menu to each game scene.
    public static class ExamSceneBuilder
    {
        public const string MainMenuScene = "Assets/Scenes/MainMenu.unity";
        public const string DrivingScene = "Assets/Scenes/DrivingGame.unity";
        public const string FlyingScene = "Assets/Scenes/FlyingGame.unity";
        public const string SumoScene = "Assets/Scenes/SumoGame.unity";

        public const string StudentName = "Ye Htet Aung";

        [MenuItem("Three In One/Build All Scenes")]
        public static void BuildAll()
        {
            ConvertBuiltInMaterialsToUrp();
            BuildMainMenu();
            BuildDrivingGame();
            BuildFlyingGame();
            BuildSumoGame();

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(MainMenuScene, true),
                new EditorBuildSettingsScene(DrivingScene, true),
                new EditorBuildSettingsScene(FlyingScene, true),
                new EditorBuildSettingsScene(SumoScene, true)
            };

            AssetDatabase.SaveAssets();
            EditorSceneManager.OpenScene(MainMenuScene);
            Debug.Log("[ThreeInOne] All scenes built and added to Build Settings.");
        }

        // ---------------- Main Menu ----------------

        public const string GameTitle = "HOLY TRINITY";

        // Retro arcade palette
        static readonly Color RetroBlack = new Color(0.03f, 0.02f, 0.08f);
        static readonly Color RetroYellow = new Color(1f, 0.91f, 0.10f);
        static readonly Color RetroRed = new Color(0.90f, 0.12f, 0.22f);
        static readonly Color RetroCyan = new Color(0.20f, 0.95f, 1f);
        static readonly Color DrivingColor = new Color(1f, 0.55f, 0.05f);   // orange
        static readonly Color FlyingColor = new Color(0.20f, 0.60f, 1f);    // blue
        static readonly Color SumoColor = new Color(0.25f, 0.95f, 0.30f);   // green
        static readonly Color ExitColor = new Color(0.90f, 0.12f, 0.22f);   // red

        static void BuildMainMenu()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = RetroBlack;

            MainMenu menu = new GameObject("Main Menu").AddComponent<MainMenu>();

            Canvas canvas = CreateCanvas("Main Menu Canvas");

            // Retro arcade background: black, pixel starfield, CRT scanlines, pixel frame
            CreateFullScreenPanel(canvas.transform, "Background", RetroBlack);
            GameObject stars = CreateFullScreenPanel(canvas.transform, "Starfield", Color.white);
            stars.GetComponent<Image>().sprite = StarfieldSprite();
            CreatePixelFrame(canvas.transform, 24f, 12f);

            // Title near the top, centered, with a rainbow stripe band under it
            Text title = CreateText(canvas.transform, "Title", GameTitle, 140, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 310f), new Vector2(1700f, 180f));
            StyleHeading(title);
            CreateStripeBand(canvas.transform, new Vector2(0f, 205f), 1000f);

            Text tagline = CreateText(canvas.transform, "Tagline", "3-IN-1 GAME PACK", 40, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 150f), new Vector2(1200f, 60f));
            tagline.color = RetroCyan;
            tagline.fontStyle = FontStyle.Bold;

            // Four stacked, left-aligned menu items, each with its game's pixel square
            const float left = -355f;
            Vector2 leftPivot = new Vector2(0f, 0.5f);
            CreateMenuButton(canvas.transform, "Mad Driver", TextAnchor.MiddleLeft, new Vector2(left, 30f), leftPivot, menu.PlayMadDriver, DrivingColor);
            CreateMenuButton(canvas.transform, "Fly Like a Bird", TextAnchor.MiddleLeft, new Vector2(left, -60f), leftPivot, menu.PlayFlyLikeABird, FlyingColor);
            CreateMenuButton(canvas.transform, "I'm a Sumo and a Ball", TextAnchor.MiddleLeft, new Vector2(left, -150f), leftPivot, menu.PlaySumo, SumoColor);
            CreateMenuButton(canvas.transform, "Exit", TextAnchor.MiddleLeft, new Vector2(left, -240f), leftPivot, menu.ExitGame, ExitColor);

            // Byline in the bottom-right corner
            Text byline = CreateText(canvas.transform, "Byline", "By " + StudentName, 44, TextAnchor.LowerRight,
                new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-70f, 55f), new Vector2(900f, 80f));
            byline.rectTransform.pivot = new Vector2(1f, 0f);
            byline.color = RetroCyan;
            byline.fontStyle = FontStyle.Bold;

            // Scanlines on top of everything
            GameObject scanlines = CreateFullScreenPanel(canvas.transform, "Scanlines", Color.white);
            scanlines.GetComponent<Image>().sprite = ScanlineSprite();
            scanlines.GetComponent<Image>().raycastTarget = false;

            EnsureEventSystem();
            EditorSceneManager.SaveScene(scene, MainMenuScene);
        }

        // ---------------- Driving (Prototype 1) ----------------

        static void BuildDrivingGame()
        {
            Scene scene = EditorSceneManager.OpenScene("Assets/Scenes/Prototype 1.unity", OpenSceneMode.Single);

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                if (root.name.Contains("Road")) EnsureColliders(root);
            }

            GameObject player = SpawnPrefab("Assets/Course Library/Vehicles/Veh_Car_Blue_Z.prefab", new Vector3(0f, 0.1f, 0f));
            player.name = "Player";
            EnsureColliders(player);
            Rigidbody playerRb = player.AddComponent<Rigidbody>();
            playerRb.mass = 1000f;
            playerRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            player.AddComponent<VehicleController>();

            // Obstacles on the road, like the Prototype 1 lesson
            string[] obstacles =
            {
                "Assets/Course Library/Obstacles/Crate_01.prefab",
                "Assets/Course Library/Obstacles/Barrel_02.prefab",
                "Assets/Course Library/Obstacles/Prop_Cone_01.prefab",
                "Assets/Course Library/Obstacles/Prop_Spool_02.prefab",
                "Assets/Course Library/Obstacles/Crate_01.prefab",
                "Assets/Course Library/Obstacles/Prop_Barrier02.prefab"
            };
            for (int i = 0; i < obstacles.Length; i++)
            {
                float x = (i % 2 == 0) ? -2f : 2f;
                GameObject obstacle = SpawnPrefab(obstacles[i], new Vector3(x, 0.1f, 20f + i * 20f));
                EnsureColliders(obstacle);
                obstacle.AddComponent<Rigidbody>().mass = 50f;
            }

            GameObject cameraObject = Camera.main != null ? Camera.main.gameObject : Object.FindAnyObjectByType<Camera>().gameObject;
            FollowPlayer follow = cameraObject.GetComponent<FollowPlayer>() ?? cameraObject.AddComponent<FollowPlayer>();
            follow.player = player;
            follow.offset = new Vector3(0f, 5f, -7f);
            cameraObject.transform.position = player.transform.position + follow.offset;
            cameraObject.transform.rotation = Quaternion.Euler(20f, 0f, 0f);
            UseSkyColour(cameraObject.GetComponent<Camera>());

            AddPauseMenu();
            EditorSceneManager.SaveScene(scene, DrivingScene);
        }

        // ---------------- Flying (Challenge 1) ----------------

        static void BuildFlyingGame()
        {
            Scene scene = EditorSceneManager.OpenScene("Assets/Challenge 1/Challenge 1.unity", OpenSceneMode.Single);

            PlayerControllerX plane = Object.FindAnyObjectByType<PlayerControllerX>();
            if (plane == null)
            {
                GameObject planeObject = scene.GetRootGameObjects().First(g => g.name.Contains("Plane"));
                plane = planeObject.AddComponent<PlayerControllerX>();
            }
            plane.speed = 15f;
            plane.rotationSpeed = 60f;

            FollowPlayerX follow = Object.FindAnyObjectByType<FollowPlayerX>();
            if (follow == null)
            {
                follow = Camera.main.gameObject.AddComponent<FollowPlayerX>();
            }
            follow.plane = plane.gameObject;
            // Challenge 1 fix: camera sits beside the plane and looks at it from the side
            follow.transform.position = plane.transform.position + new Vector3(30f, 0f, 10f);
            follow.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
            UseSkyColour(follow.GetComponent<Camera>());

            Transform propeller = plane.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.ToLower().Contains("propel"));
            if (propeller != null && propeller.GetComponent<SpinPropellerX>() == null)
            {
                propeller.gameObject.AddComponent<SpinPropellerX>();
            }

            // The obstacle clouds use a pure black material; make them white clouds instead
            Material black = AssetDatabase.LoadAssetAtPath<Material>("Assets/Challenge 1/_Source_Files/Materials/Black.mat");
            Material cloud = CloudMaterial();
            foreach (Renderer renderer in Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include))
            {
                Material[] materials = renderer.sharedMaterials;
                bool changed = false;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] == black)
                    {
                        materials[i] = cloud;
                        changed = true;
                    }
                }
                if (changed) renderer.sharedMaterials = materials;
            }

            AddPauseMenu();
            EditorSceneManager.SaveScene(scene, FlyingScene);
        }

        // ---------------- Sumo (Prototype 4) ----------------

        static void BuildSumoGame()
        {
            Scene scene = EditorSceneManager.OpenScene("Assets/Sumo/Prototype 4.unity", OpenSceneMode.Single);
            AddPauseMenu();
            EditorSceneManager.SaveScene(scene, SumoScene);
        }

        // ---------------- In-Game Menu ----------------

        static void AddPauseMenu()
        {
            PauseMenu pauseMenu = new GameObject("Pause Menu").AddComponent<PauseMenu>();

            Canvas canvas = CreateCanvas("Pause Menu Canvas");

            // Dim the frozen game behind the menu
            GameObject panel = CreateFullScreenPanel(canvas.transform, "Pause Panel", new Color(0f, 0f, 0f, 0.6f));
            pauseMenu.pausePanel = panel;

            // Retro box: black with a chunky cyan pixel border
            GameObject card = new GameObject("Card", typeof(RectTransform), typeof(Image));
            card.transform.SetParent(panel.transform, false);
            card.GetComponent<Image>().color = new Color(RetroBlack.r, RetroBlack.g, RetroBlack.b, 0.96f);
            RectTransform cardRect = card.GetComponent<RectTransform>();
            cardRect.anchoredPosition = new Vector2(0f, 60f);
            cardRect.sizeDelta = new Vector2(1000f, 680f);
            GameObject cardStars = CreateFullScreenPanel(card.transform, "Starfield", Color.white);
            cardStars.GetComponent<Image>().sprite = StarfieldSprite();
            CreatePixelFrame(card.transform, 0f, 12f);

            Text paused = CreateText(panel.transform, "Paused Title", "PAUSED", 130, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 290f), new Vector2(1200f, 170f));
            StyleHeading(paused);
            CreateStripeBand(panel.transform, new Vector2(0f, 190f), 600f);

            Vector2 center = new Vector2(0.5f, 0.5f);
            CreateMenuButton(panel.transform, "Resume", TextAnchor.MiddleCenter, new Vector2(0f, 40f), center, pauseMenu.Resume, null);
            CreateMenuButton(panel.transform, "Restart", TextAnchor.MiddleCenter, new Vector2(0f, -50f), center, pauseMenu.Restart, null);
            CreateMenuButton(panel.transform, "Back to Main Menu", TextAnchor.MiddleCenter, new Vector2(0f, -140f), center, pauseMenu.BackToMainMenu, null);

            GameObject scanlines = CreateFullScreenPanel(panel.transform, "Scanlines", Color.white);
            scanlines.GetComponent<Image>().sprite = ScanlineSprite();
            scanlines.GetComponent<Image>().raycastTarget = false;

            panel.SetActive(false);
            EnsureEventSystem();
        }

        // White cloud: URP Unlit so it always reads as a bright cloud from every side.
        static Material CloudMaterial()
        {
            const string path = "Assets/Challenge 1/_Source_Files/Materials/Cloud.mat";
            Material cloud = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (cloud == null)
            {
                cloud = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
                AssetDatabase.CreateAsset(cloud, path);
            }
            cloud.shader = Shader.Find("Universal Render Pipeline/Unlit");
            cloud.SetColor("_BaseColor", new Color(0.97f, 0.98f, 1f));
            EditorUtility.SetDirty(cloud);
            AssetDatabase.SaveAssets();
            return cloud;
        }

        // The built-in default skybox renders black in this URP setup; use a sky colour instead.
        static void UseSkyColour(Camera camera)
        {
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.55f, 0.80f, 0.95f);

            // Flat ambient light so shaded sides are not black (the skybox ambient is never baked)
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.55f, 0.57f, 0.62f);
        }

        // ---------------- Menu styling ----------------

        // Arcade title: yellow letters with a hard red drop shadow and black outline.
        static void StyleHeading(Text text)
        {
            text.color = RetroYellow;
            text.fontStyle = FontStyle.Bold;
            Outline outline = text.gameObject.AddComponent<Outline>();
            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(4f, -4f);
            Shadow shadow = text.gameObject.AddComponent<Shadow>();
            shadow.effectColor = RetroRed;
            shadow.effectDistance = new Vector2(10f, -10f);
        }

        // Chunky double pixel border (cyan outside, magenta inside).
        static void CreatePixelFrame(Transform parent, float inset, float thickness)
        {
            CreateBorder(parent, "Frame Outer", inset, thickness, RetroCyan);
            CreateBorder(parent, "Frame Inner", inset + thickness * 2f, thickness * 0.5f, new Color(1f, 0.2f, 0.75f));
        }

        static void CreateBorder(Transform parent, string name, float inset, float thickness, Color color)
        {
            // top, bottom, left, right bars
            Vector4[] bars =
            {
                new Vector4(0f, 1f, 1f, 1f), new Vector4(0f, 0f, 1f, 0f),
                new Vector4(0f, 0f, 0f, 1f), new Vector4(1f, 0f, 1f, 1f)
            };
            for (int i = 0; i < bars.Length; i++)
            {
                GameObject bar = new GameObject(name + " " + i, typeof(RectTransform), typeof(Image));
                bar.transform.SetParent(parent, false);
                Image image = bar.GetComponent<Image>();
                image.color = color;
                image.raycastTarget = false;
                RectTransform rect = bar.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(bars[i].x, bars[i].y);
                rect.anchorMax = new Vector2(bars[i].z, bars[i].w);
                bool horizontal = i < 2;
                float sign = (i == 0 || i == 3) ? -1f : 1f;
                rect.sizeDelta = horizontal ? new Vector2(-inset * 2f, thickness) : new Vector2(thickness, -inset * 2f);
                rect.anchoredPosition = horizontal ? new Vector2(0f, sign * (inset + thickness / 2f)) : new Vector2(sign * (inset + thickness / 2f), 0f);
            }
        }

        // 80s rainbow stripe band: red, orange, yellow, green, cyan.
        static void CreateStripeBand(Transform parent, Vector2 position, float width)
        {
            Color[] colors = { RetroRed, DrivingColor, RetroYellow, SumoColor, RetroCyan };
            for (int i = 0; i < colors.Length; i++)
            {
                GameObject stripe = new GameObject("Stripe " + i, typeof(RectTransform), typeof(Image));
                stripe.transform.SetParent(parent, false);
                Image image = stripe.GetComponent<Image>();
                image.color = colors[i];
                image.raycastTarget = false;
                RectTransform rect = stripe.GetComponent<RectTransform>();
                rect.anchoredPosition = position + new Vector2(0f, -i * 10f);
                rect.sizeDelta = new Vector2(width, 6f);
            }
        }

        // Pixel starfield (point filtered so each star is a crisp square).
        static Sprite StarfieldSprite()
        {
            System.Random random = new System.Random(2026);
            Color[] starColors = { Color.white, Color.white, RetroCyan, RetroYellow, new Color(1f, 0.4f, 0.8f) };
            Dictionary<int, Color> stars = new Dictionary<int, Color>();
            for (int i = 0; i < 260; i++)
            {
                stars[random.Next(480) + random.Next(270) * 480] = starColors[random.Next(starColors.Length)] * new Color(1f, 1f, 1f, 0.5f + (float)random.NextDouble() * 0.5f);
            }
            return GeneratedSprite("Assets/UI/RetroStars.png", 480, 270, (x, y) =>
                stars.TryGetValue(x + y * 480, out Color c) ? c : new Color(0f, 0f, 0f, 0f), true);
        }

        // CRT scanlines: every other row slightly dark.
        static Sprite ScanlineSprite()
        {
            return GeneratedSprite("Assets/UI/RetroScanlines.png", 4, 540, (x, y) =>
                y % 2 == 0 ? new Color(0f, 0f, 0f, 0.22f) : new Color(0f, 0f, 0f, 0f), true);
        }

        static Sprite GeneratedSprite(string path, int width, int height, System.Func<int, int, Color> pixel, bool pixelated = false)
        {
            if (!System.IO.File.Exists(path))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
                Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                        texture.SetPixel(x, y, pixel(x, y));
                System.IO.File.WriteAllBytes(path, texture.EncodeToPNG());
                Object.DestroyImmediate(texture);
                AssetDatabase.ImportAsset(path);
            }

            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
            if (importer.textureType != TextureImporterType.Sprite || importer.spriteImportMode != SpriteImportMode.Single)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.filterMode = pixelated ? FilterMode.Point : FilterMode.Bilinear;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
            }
            Sprite sprite = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().FirstOrDefault();
            if (sprite == null) throw new System.Exception("Could not load generated sprite " + path);
            return sprite;
        }

        // ---------------- Materials ----------------

        // Some class-package materials use Built-in pipeline shaders (Standard, Unlit,
        // legacy Particles) which render pink under URP. Switch them to URP shaders.
        [MenuItem("Three In One/Convert Built-in Materials to URP")]
        public static void ConvertBuiltInMaterialsToUrp()
        {
            Shader lit = Shader.Find("Universal Render Pipeline/Lit");
            Shader unlit = Shader.Find("Universal Render Pipeline/Unlit");
            Shader particles = Shader.Find("Universal Render Pipeline/Particles/Unlit");

            foreach (string guid in AssetDatabase.FindAssets("t:Material", new[] { "Assets" }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (material == null || material.shader == null) continue;

                string shaderName = material.shader.name;
                if (shaderName.StartsWith("Universal Render Pipeline") || shaderName.StartsWith("Shader Graphs")) continue;

                Texture mainTexture = material.HasProperty("_MainTex") ? material.GetTexture("_MainTex") : null;
                Color color = material.HasProperty("_Color") ? material.GetColor("_Color") : Color.white;

                Shader target;
                if (shaderName.StartsWith("Particles") || shaderName.StartsWith("Legacy Shaders/Particles")) target = particles;
                else if (shaderName.StartsWith("Unlit") || shaderName.StartsWith("Skybox")) target = unlit;
                else target = lit;

                material.shader = target;
                material.SetTexture("_BaseMap", mainTexture);
                material.SetColor("_BaseColor", color);
                EditorUtility.SetDirty(material);
                Debug.Log("[ThreeInOne] Converted " + path + " from '" + shaderName + "' to '" + target.name + "'");
            }

            // The mountain backdrop faces away from the sun, so show it unlit (its real colours)
            foreach (string guid in AssetDatabase.FindAssets("PolygonNature_MountainSkybox t:Material", new[] { "Assets" }))
            {
                Material sky = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));
                if (sky.shader != unlit)
                {
                    Texture baseMap = sky.GetTexture("_BaseMap");
                    sky.shader = unlit;
                    sky.SetTexture("_BaseMap", baseMap);
                    sky.SetColor("_BaseColor", Color.white);
                    EditorUtility.SetDirty(sky);
                    Debug.Log("[ThreeInOne] Mountain backdrop set to URP Unlit");
                }
            }
            AssetDatabase.SaveAssets();
        }

        // ---------------- Helpers ----------------

        static GameObject SpawnPrefab(string path, Vector3 position)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                throw new System.Exception("Missing prefab: " + path);
            }
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.position = position;
            return instance;
        }

        // Adds box colliders to every mesh if the object has no collider yet.
        static void EnsureColliders(GameObject root)
        {
            if (root.GetComponentInChildren<Collider>() != null) return;
            foreach (MeshFilter meshFilter in root.GetComponentsInChildren<MeshFilter>())
            {
                meshFilter.gameObject.AddComponent<BoxCollider>();
            }
        }

        static Canvas CreateCanvas(string name)
        {
            GameObject canvasObject = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            return canvas;
        }

        static GameObject CreateFullScreenPanel(Transform parent, string name, Color color)
        {
            GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            panel.GetComponent<Image>().color = color;
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return panel;
        }

        static Text CreateText(Transform parent, string name, string value, int fontSize, TextAnchor alignment,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size)
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);

            Text text = textObject.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = value;
            text.fontSize = fontSize;
            text.color = Color.black;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;

            RectTransform rect = text.rectTransform;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            return text;
        }

        // A text menu item: white text that turns gold on hover, with an optional colour bar.
        static Button CreateMenuButton(Transform parent, string label, TextAnchor alignment, Vector2 position, Vector2 pivot, UnityAction onClick, Color? accent)
        {
            GameObject buttonObject = new GameObject(label + " Button", typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);

            Image hitArea = buttonObject.GetComponent<Image>();
            hitArea.color = new Color(1f, 1f, 1f, 0f);

            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = pivot;
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(800f, 80f);

            if (accent.HasValue)
            {
                GameObject bar = new GameObject("Colour Bar", typeof(RectTransform), typeof(Image));
                bar.transform.SetParent(buttonObject.transform, false);
                Image barImage = bar.GetComponent<Image>();
                barImage.color = accent.Value;
                barImage.raycastTarget = false;
                RectTransform barRect = bar.GetComponent<RectTransform>();
                barRect.anchorMin = new Vector2(0f, 0.5f);
                barRect.anchorMax = new Vector2(0f, 0.5f);
                barRect.pivot = new Vector2(1f, 0.5f);
                barRect.anchoredPosition = new Vector2(-28f, 0f);
                barRect.sizeDelta = new Vector2(30f, 30f);
            }

            Text text = CreateText(buttonObject.transform, "Label", label, 64, alignment,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            text.color = Color.white;
            text.fontStyle = FontStyle.Bold;
            Shadow shadow = text.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0.25f, 0.15f, 0.65f, 1f);
            shadow.effectDistance = new Vector2(5f, -5f);

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = text;
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = RetroYellow;
            colors.selectedColor = Color.white;
            colors.pressedColor = RetroRed;
            colors.fadeDuration = 0.08f;
            button.colors = colors;

            UnityEventTools.AddPersistentListener(button.onClick, onClick);
            return button;
        }

        static void EnsureEventSystem()
        {
            if (Object.FindAnyObjectByType<EventSystem>() == null)
            {
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }
        }
    }
}
