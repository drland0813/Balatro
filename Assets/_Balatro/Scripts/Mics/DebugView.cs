using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDebugCategory
{
    string DebugValueForKey(string key);
}

public interface IDebugCheat
{
    void ProcessCheatCommand(string group, string command);
}

public class DebugView : MonoBehaviour
{
    class DebugInfo
    {
        public IDebugCategory Category;
        public List<string> Keys;
    }

    class DebugCheatInfo
    {
        public IDebugCheat Group;
        public string Command;
        public string HelpDesc;
        public bool DisplayHelp;
    }

#if ENABLE_DEBUG_VIEW
    static DebugView s_instance;
    static Dictionary<string, DebugInfo> s_categories = new Dictionary<string, DebugInfo>();
    static string[] s_categoryNames;
    static string s_debugStatus;
    static Dictionary<string, DebugCheatInfo> s_debugCheats = new Dictionary<string, DebugCheatInfo>();
#endif

    public static void RegisterDebugCategory(string name, IDebugCategory category, List<string> keys)
    {
#if ENABLE_DEBUG_VIEW
        if (s_categories.ContainsKey(name))
        {
            Debug.LogWarningFormat("Category {0} already existed!", name);
            return;
        }

        if (category == null)
        {
            s_categories.Add(name, null);
        }
        else
        {
            s_categories.Add(name, new DebugInfo
            {
                Category = category,
                Keys = keys
            });
        }

        s_categoryNames = new List<string>(s_categories.Keys).ToArray();
#endif
    }

    public static void RemoveDebugCategory(string name)
    {
#if ENABLE_DEBUG_VIEW
        s_categories.Remove(name);
        s_categoryNames = new List<string>(s_categories.Keys).ToArray();
#endif
    }

    public static void RegisterDebugCheatGroup(string name, IDebugCheat debugCheat, string helpDesc = null)
    {
#if ENABLE_DEBUG_VIEW
        if (s_debugCheats.ContainsKey(name))
        {
            Debug.LogWarningFormat("Cheat group {0} already existed!", name);
            return;
        }

        s_debugCheats.Add(name, new DebugCheatInfo()
        {
            Group = debugCheat,
            Command = string.Empty,
            HelpDesc = helpDesc
        });

        if (!s_categories.ContainsKey("CHEAT"))
        {
            RegisterDebugCategory("CHEAT", null, null);
        }
#endif
    }

    public static void RemoveDebugCheatGroup(string name)
    {
#if ENABLE_DEBUG_VIEW
        s_debugCheats.Remove(name);
        if (s_debugCheats.Count == 0)
        {
            RemoveDebugCategory("CHEAT");
        }
#endif
    }

    public static void SetDebugStatus(string status)
    {
#if ENABLE_DEBUG_VIEW
        s_debugStatus = status;
#endif
    }

#if ENABLE_DEBUG_VIEW
    void Awake()
    {
        if (s_instance != null)
        {
            Destroy(s_instance);
            s_categories.Clear();
        }

        s_instance = this;
    }
#endif

#if ENABLE_DEBUG_VIEW
    bool showDebugView = false;
    bool disableDebugView = false;
    int _currentCategoryIdx;

    void OnGUI()
    {
        if (disableDebugView)
            return;

        Vector2 nativeSize = new Vector2(480, 640);
        Vector3 scale = new Vector3(Screen.width / nativeSize.x, Screen.height / nativeSize.y, 1.0f);
        GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, scale);

        GUILayout.BeginArea(new Rect(10, 10, 480 - 10, 640 - 10));
        GUILayout.BeginVertical();

        if (showDebugView)
        {
            if (!string.IsNullOrEmpty(s_debugStatus))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Status:", GUILayout.Width(50));
                GUILayout.Label(s_debugStatus, GUILayout.MaxWidth(200));
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginVertical();
            // categories
            if (s_categories.Count > 0)
            {
                if (_currentCategoryIdx >= s_categoryNames.Length)
                    _currentCategoryIdx = 0;

                _currentCategoryIdx = GUILayout.Toolbar(_currentCategoryIdx, s_categoryNames);
                var category = s_categories[s_categoryNames[_currentCategoryIdx]];
                // cheat
                if (category == null)
                {
                    foreach (var keyPair in s_debugCheats)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(keyPair.Key, GUILayout.Width(100));
                        keyPair.Value.Command = GUILayout.TextField(keyPair.Value.Command, GUILayout.Width(120));
                        if (GUILayout.Button("SEND", GUILayout.Width(80)))
                        {
                            keyPair.Value.Command = keyPair.Value.Command.Trim();
                            keyPair.Value.Group.ProcessCheatCommand(keyPair.Key, keyPair.Value.Command);
                            keyPair.Value.Command = string.Empty;
                        }

                        if (!keyPair.Value.DisplayHelp && !string.IsNullOrEmpty(keyPair.Value.HelpDesc))
                        {
                            if (GUILayout.Button("+", GUILayout.Width(40)))
                                keyPair.Value.DisplayHelp = true;
                        }
                        else if (keyPair.Value.DisplayHelp)
                        {
                            if (GUILayout.Button("-", GUILayout.Width(40)))
                                keyPair.Value.DisplayHelp = false;
                        }

                        GUILayout.EndHorizontal();

                        if (keyPair.Value.DisplayHelp)
                            GUILayout.Label(keyPair.Value.HelpDesc, GUILayout.Width(300));
                    }
                }
                else
                {
                    foreach (var stat in category.Keys)
                    {
                        GUILayout.BeginHorizontal();
                        var value = category.Category.DebugValueForKey(stat);
                        GUILayout.Label(stat, GUILayout.Width(200));
                        GUILayout.Label(value, GUILayout.MaxWidth(480));
                        GUILayout.EndHorizontal();
                    }
                }
            }
            GUILayout.EndVertical();

            if (GUILayout.Button("CLEAR DATA", GUILayout.Width(100)))
                PlayerPrefs.DeleteAll();

            if (GUILayout.Button("HIDE", GUILayout.Width(100)))
                showDebugView = false;
        }
        else
        {
            if (GUI.Button(new Rect(480 - 20 - 120, 0, 120, 25), "SHOW DEBUG"))
                showDebugView = true;

            if (GUI.Button(new Rect(480 - 20 - 120, 30, 120, 25), "CLOSE DEBUG"))
                disableDebugView = true;
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
#endif
}
