
using UnityEngine;


public class Managers : MonoBehaviour
{
    //ΩÃ±€≈Ê ∆–≈œ
    static Managers s_instance;
   
    static Managers Instance { get { Init();  return s_instance; } }
    #region Contents

    GameManagers _game = new GameManagers();
    public static GameManagers Game { get { return Instance._game; } }

    #endregion

    #region Core

    AudioManagers _sound = new AudioManagers();
    DataManagers _data = new DataManagers();
    InputManagers input = new InputManagers();
    PoolManagers _pool = new PoolManagers();
    ResourceManagers resource = new ResourceManagers();
    SceneManagersEx _scene = new SceneManagersEx();
    UIManagers ui = new UIManagers();

    public static AudioManagers Sound { get { Init(); return Instance._sound; } }
    public static DataManagers Data { get { return Instance._data; } }
    public static InputManagers Input { get { return Instance.input; } }
    public static PoolManagers Pool { get { return Instance._pool; } }
    public static ResourceManagers Resource { get { return Instance.resource; } }
    public static SceneManagersEx Scene { get { return Instance._scene; } }
    public static UIManagers UI {get{return Instance.ui ;}}

    #endregion
    void Start()
    {
        Init();
    }
    void Update()
    {
        input.OnUpdate();
    }
    static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();
            s_instance._data.Init();
            s_instance._pool.Init();
            s_instance._sound.Init();
        }

    } 
    public static void Clear()
    {
        //æ¿ ≥—æÓ∞•∂ß 
        Sound.Clear();
        Input.Clear();
        UI.Clear();
        Scene.Clear();
        Pool.Clear();
    }
}
