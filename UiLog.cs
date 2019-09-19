using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static UnityEngine.Application;

public class UiLog : MonoBehaviour
{
    private static StringBuilder log = new StringBuilder();
    private static GameObject instance;

    public static GUIStyle style { get; set; }

    public static int MaxLogs { get => _maxLogs; set { _maxLogs = value < 1 ? 1 : value; } }
    private static int _maxLogs = 100;

    private static List<LogRegister> logRegister = new List<LogRegister>();

    public int fontSize = 40;

    public bool defaultLog = true;
    public bool warningLog = true;
    public bool errorLog = true;

    public Color defaultColor = Color.black;
    public Color warningColor = Color.blue;
    public Color errorColor = Color.red;

    public static int _fontSize = 40;
    
    public static bool _defaultLog;
    public static bool _warningLog;
    public static bool _errorLog;

    public static Color _defaultColor = Color.black;
    public static Color _warningColor = Color.blue;
    public static Color _errorColor = Color.red;

    private struct LogRegister
    {
        public string condition;
        public string stackTrace;
        public LogType type;

        public LogRegister(string condition, string stackTrace, LogType type)
        {
            this.condition = condition;
            this.stackTrace = stackTrace;
            this.type = type;
        }
    }

    public static void Display(bool show)
    {
        if(show)
        {
            if (!instance)
            {
                instance = new GameObject("UI-logger", typeof(UiLog));
                instance.hideFlags = HideFlags.HideInHierarchy;
            }

            Application.logMessageReceived -= LogReceiver;
            Application.logMessageReceived += LogReceiver;
        }
        else
        {
            Application.logMessageReceived -= LogReceiver;

            if (instance)
            {
                Destroy(instance);
                instance = null;
            }
        }
    }

    private static void LogReceiver(string condition, string stackTrace, LogType type)
    {
        if (!IsTypeEnabled(type))
            return;

        logRegister.Add(new LogRegister(condition, stackTrace, type));

        while (logRegister.Count > MaxLogs)
        {
            logRegister.RemoveAt(0);
        }

        Repaint();
    }

    private static void Repaint()
    {
        log.Clear();

        string defaultC = ToRGBHex(_defaultColor);
        string warning = ToRGBHex(_warningColor);
        string error = ToRGBHex(_errorColor);

        for (int i = 0; i < logRegister.Count; i++)
        {
            string color = string.Empty;
            switch (logRegister[i].type)
            {
                case LogType.Error:
                    color = error;
                    break;
                case LogType.Assert:
                    color = error;
                    break;
                case LogType.Warning:
                    color = warning;
                    break;
                case LogType.Log:
                    color = defaultC;
                    break;
                case LogType.Exception:
                    color = error;
                    break;
            }

            string thisLog = string.Empty;

            if (logRegister[i].type == LogType.Log)
            {
                log.Append("<color=");
                log.Append(color);
                log.Append(">");
                log.Append(logRegister[i].condition);
                log.Append("</color>");
                log.Append("\n");
            }
            else
            {
                log.Append("<color=");
                log.Append(color);
                log.Append(">");
                log.Append(logRegister[i].condition);
                log.Append("\n");
                log.Append(logRegister[i].stackTrace);
                log.Append("</color>");
                log.Append("\n");
            }
        }
    }

    private static bool IsTypeEnabled(LogType type)
    {
        bool enabled = false;
        switch (type)
        {
            case LogType.Error:
                enabled = _errorLog;
                break;
            case LogType.Assert:
                enabled = _errorLog;
                break;
            case LogType.Warning:
                enabled = _warningLog;
                break;
            case LogType.Log:
                enabled = _defaultLog;
                break;
            case LogType.Exception:
                enabled = _errorLog;
                break;
        }

        return enabled;
    }

    public static void Clear()
    {
        log.Clear();
        logRegister.Clear();
    }

    public static string ToRGBHex(Color c)
    {
        return string.Format("#{0:X2}{1:X2}{2:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b));
    }

    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }

    private void Start()
    {
        if (!instance)
        {
            instance = gameObject;
            Display(true);

            //set the colors and the logs of the instance to the static values so it can persist throgh scenes
            _fontSize = fontSize;

            _defaultColor = defaultColor;
            _warningColor = warningColor;
            _errorColor = errorColor;

            _defaultLog = defaultLog;
            _warningLog = warningLog;
            _errorLog = errorLog;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Display(true);

        if (Input.GetKeyDown(KeyCode.Alpha1))
            Debug.Log("Unity - Log");
        if (Input.GetKeyDown(KeyCode.Alpha2))
            Debug.LogError("Unity - Error");
        if (Input.GetKeyDown(KeyCode.Alpha3))
            Debug.LogWarning("Unity - Warning");
        if (Input.GetKeyDown(KeyCode.Alpha4))
            print(ToRGBHex(_defaultColor));
    }

    private void OnGUI()
    {
        GUI.Label(
            new Rect(0, 0, Screen.width, Screen.height),
            log.ToString(),
            style != null ? style : new GUIStyle()
            {
                fontSize = _fontSize * Screen.height / Screen.height,
                padding = new RectOffset(10,10,10,10),
                alignment = TextAnchor.LowerLeft,
                wordWrap = true,
                richText = true,
            });
    }

    private void OnValidate()
    {
        _fontSize = fontSize;

        _defaultColor = defaultColor;
        _warningColor = warningColor;
        _errorColor = errorColor;

        _defaultLog = defaultLog;
        _warningLog = warningLog;
        _errorLog = errorLog;

        Repaint();
    }
}
