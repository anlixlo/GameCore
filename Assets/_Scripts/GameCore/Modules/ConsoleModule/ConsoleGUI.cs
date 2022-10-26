using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore
{
    public class ConsoleGUI : MonoBehaviour
    {
        [Header("UI Setting")]
        private bool _isSetupButton = false;
        private bool _isSetupBackPanel = false;
        private bool _isShowExtraData = false;

        private Vector2 _designResolution = new Vector2(1920, 1080);   // smaller resolution, the UI will larger
        private Vector2 _consoleButtonPos = new Vector2(10, 10);
        private Vector2 _consoleButtonScale = new Vector2(100, 50);

        private Vector2 _commandInputPos = new Vector2(0, 1000);
        private Vector2 _commandInputScale = new Vector2(1880, 40);
        private int _commandInputFontSize = 30;

        private Vector2 _debugLogConsolePos = new Vector2(0, 90);
        private Vector2 _debugLogConsoleScale = new Vector2(1880, 900);
        private int _debugLogConsoleFontSize = 30;

        private Color _defaultColor = new Color(0.25f, 0.25f, 0.25f, 0.75f);

        [Header("Font Setting")]
        public Font uIFont;

        protected bool isVisible = false;

        // Internal Data
        private RectTransform _backPanelTF;
        private RectTransform _consoleButtonTF;
        private bool mHasInit = false;

        private string _commandInput = "";

        public delegate void consoleCommandCallback(string command);

        public consoleCommandCallback processCommandCallback;

        //Debug Log Console
        private int _maxLines = 25;
        private Queue<string> _debugLogQueue = new Queue<string>();
        private string _debugLogConsoleLabel = "";

        protected GUILayoutOption[] mButtonSizeOptions = {
            GUILayout.Width(200),
            GUILayout.Height(40),
        };

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            Setup();
            Close();    // GM Tool closed by default
        }

        private void OnEnable()
        {
            SetupLogMessageReceivedThreaded();
        }

        private void OnDisable()
        {
            ReleaseLogMessageReceivedThreaded();
        }

        public void Setup()
        {
            DontDestroyOnLoad(this.gameObject);

            Canvas _canvas = this.gameObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 1;

            CanvasScaler _canvasScaler = this.gameObject.AddComponent<CanvasScaler>();
            _canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _canvasScaler.referenceResolution = _designResolution;
            _canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            _canvasScaler.matchWidthOrHeight = 1;

            this.gameObject.AddComponent<GraphicRaycaster>();

            // Back Panel
            GameObject _backPanelObj = new GameObject("BackPanel");
            _backPanelObj.AddComponent<RectTransform>();
            _backPanelObj.transform.SetParent(this.transform);
            _backPanelTF = (RectTransform)_backPanelObj.transform;
            _backPanelTF.anchorMin = Vector2.zero;
            _backPanelTF.anchorMax = Vector2.one;
            _backPanelTF.offsetMin = Vector2.zero;
            _backPanelTF.offsetMax = Vector2.zero;

            if (_isSetupBackPanel)
            {
                Image _backPanelImg = _backPanelObj.AddComponent<Image>();
                _backPanelImg.color = _defaultColor;
                _backPanelImg.type = Image.Type.Sliced;
                _backPanelImg.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            }

            if (_isSetupButton)
            {
                // Console Button
                GameObject _consoleButtonObj = new GameObject("ConsoleButton");
                _consoleButtonObj.AddComponent<RectTransform>();
                _consoleButtonObj.transform.SetParent(this.transform);
                _consoleButtonTF = (RectTransform)_consoleButtonObj.transform;
                _consoleButtonTF.anchorMin = Vector2.zero;
                _consoleButtonTF.anchorMax = Vector2.zero;
                _consoleButtonTF.pivot = Vector2.zero;
                _consoleButtonTF.anchoredPosition = _consoleButtonPos;
                _consoleButtonTF.sizeDelta = _consoleButtonScale;

                Image _consoleButtonImg = _consoleButtonObj.AddComponent<Image>();
                _consoleButtonImg.color = _defaultColor;
                _consoleButtonImg.type = Image.Type.Sliced;
                _consoleButtonImg.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

                Button _consoleButton = _consoleButtonObj.AddComponent<Button>();
                _consoleButton.onClick.AddListener(delegate { Show(); });
            }
        }

        private void SetupConsoleInputControl()
        {

            if (Event.current.type != EventType.KeyDown)
            {
                return;
            }

            if (Event.current.isKey)
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.Return:
                    case KeyCode.KeypadEnter:
                        ProcessCommand();
                        Event.current.Use();    // Ignore event, otherwise there will be control name conflicts!
                        break;
                    case KeyCode.Escape:
                        Switch();
                        Event.current.Use();
                        break;
                }
            }
        }

        #region Visibility Handling 
        private void Show()
        {
            isVisible = true;
            if (_backPanelTF != null)
            {
                _backPanelTF.gameObject.SetActive(true);
            }
            SetConsoleButtonVisible(false);
        }

        private void Close()
        {
            SetConsoleButtonVisible(true);
            isVisible = false;
            if (_backPanelTF != null)
            {
                _backPanelTF.gameObject.SetActive(false);
            }
        }

        private void Switch()
        {
            if (isVisible)
                Close();
            else
                Show();
        }

        #endregion

        #region GM Button

        private void SetConsoleButtonVisible(bool flag)
        {
            if (_consoleButtonTF == null)
            {
                return;
            }

            _consoleButtonTF.gameObject.SetActive(flag);
        }

        #endregion

        void OnGUI()
        {
            if (mHasInit == false)
            {
                //Debug.Log("INPUT Res" + designResolution);
                ResponsiveIMGUI.InitDesignResolution(_designResolution);
                mHasInit = true;
            }
            ResponsiveIMGUI.ScaleGUI();

            SetupConsoleInputControl();

            if (isVisible == false)
            {
                SetConsoleButtonVisible(true);
                return;
            }

            //Skin Setup
            GUI.skin.box.alignment = TextAnchor.MiddleCenter;
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUI.skin.button.alignment = TextAnchor.MiddleCenter;
            GUI.skin.box.font = uIFont;
            GUI.skin.label.font = uIFont;
            GUI.skin.button.font = uIFont;
            GUI.skin.textArea.fontSize = _debugLogConsoleFontSize;

            GMToolUI();
        }

        #region GM Tool 

        void GMToolUI()
        {
            Rect rect = new Rect(20, 20, _designResolution.x - 20, _designResolution.y - 20);

            GUILayout.BeginArea(rect);
            GUILayout.BeginVertical();

            SetupDebugLogConsole();
            SetupCommandInput();

            if (_isShowExtraData)
            {
                SetHeader("Game Version : " + Application.version + "  FPS: " + GetFramesPerSecond());
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        #endregion

        // General Stuff
        void SetHeader(string _headerStr)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(_headerStr, GUILayout.Height(30));
            GUILayout.EndHorizontal();
        }

        private string GetFramesPerSecond()
        {
            return ((int)(1.0f / Time.smoothDeltaTime)).ToString();
        }

        #region Console Command Input

        private void SetupCommandInput()
        {
            GUI.skin.textField.fontSize = _commandInputFontSize;
            GUI.SetNextControlName("CommandInput");
            _commandInput = GUI.TextField(new Rect(_commandInputPos.x, _commandInputPos.y, _commandInputScale.x, _commandInputScale.y), _commandInput);
            GUI.FocusControl("CommandInput");
        }

        private void ProcessCommand()
        {
            if (processCommandCallback != null)
            {
                processCommandCallback(_commandInput);
            }

            if(_commandInput != "")
            {
                _commandInput = "";
            }
        }

        #endregion

        #region Debug Log Console

        private void SetupDebugLogConsole()
        {
            GUI.Label(new Rect(_debugLogConsolePos.x, _debugLogConsolePos.y, _debugLogConsoleScale.x, _debugLogConsoleScale.y), _debugLogConsoleLabel, GUI.skin.textArea);
        }

        private void SetupLogMessageReceivedThreaded()
        {
            Application.logMessageReceivedThreaded += HandleLog;
        }

        private void ReleaseLogMessageReceivedThreaded()
        {
            Application.logMessageReceivedThreaded -= HandleLog;
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (_debugLogQueue.Count >= _maxLines)
                _debugLogQueue.Dequeue();

            _debugLogQueue.Enqueue(logString);

            StringBuilder _stringBulider = new StringBuilder();

            foreach (string logMessage in _debugLogQueue)
            {
                string _logStr = logMessage + "\n";
                _stringBulider.Append(_logStr);
            }

            _debugLogConsoleLabel = _stringBulider.ToString();
        }

        #endregion
    }
}
