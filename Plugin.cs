using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using GameNetcodeStuff;
using HarmonyLib;
using Microsoft.CodeAnalysis;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Video;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[module: UnverifiableCode]

namespace YTTV
{
    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "MARMods.YTTV";
        public const string PLUGIN_NAME = "YTTV";
        public const string PLUGIN_VERSION = "1.1.1";
    }
}

namespace Television_Controller
{
    [BepInPlugin("MARMods.YTTV", "YTTV", "1.1.1")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin instance;
        private static Harmony HarmonyLib;
        public static Configs config;

        private void Awake()
        {
            instance = this;
            config = new Configs();
            config.GetConfig();

            string text = config.languages.Value.ToLower();
            if (text == "en") config.GetLang().GetConfigEN();
            else if (text == "ru") config.GetLang().GetConfigRU();
            else config.GetLang().GetConfigEN();

            WriteLogo();

            if (!Directory.Exists("YTTV\\lang")) Directory.CreateDirectory("YTTV\\lang");
            if (!Directory.Exists("YTTV\\other")) Directory.CreateDirectory("YTTV\\other");

            if (File.Exists("YTTV\\other\\test.mp4")) File.Delete("YTTV\\other\\test.mp4");
            if (!File.Exists("YTTV\\cache"))
            {
                using (StreamWriter streamWriter = File.CreateText("YTTV\\cache")) { }
            }

            string ytDlpPath = Path.Combine("YTTV", "other", "yt-dlp.exe");
            string versionPath = Path.Combine("YTTV", "other", "yt-dlp.version");
            string expectedVersion = "2025.12.08";

            new Thread(() =>
            {
                try
                {
                    bool needsUpdate = true;
                    if (File.Exists(ytDlpPath) && File.Exists(versionPath))
                    {
                        string localVersion = File.ReadAllText(versionPath).Trim();
                        if (localVersion == expectedVersion)
                            needsUpdate = false;
                    }

                    if (needsUpdate)
                    {
                        Logger.LogInfo($"YTTV: Downloading yt-dlp version {expectedVersion}...");
                        DownloadFileSync(
                            new Uri($"https://github.com/yt-dlp/yt-dlp/releases/download/{expectedVersion}/yt-dlp.exe"),
                            ytDlpPath
                        );

                        if (File.Exists(ytDlpPath))
                            File.WriteAllText(versionPath, expectedVersion);
                    }

                    Logger.LogInfo("YTTV: Ready.");
                }
                catch (Exception e)
                {
                    Logger.LogError($"YTTV: Update failed (Check Internet/VPN): {e.Message}");
                }
            }).Start();

            HarmonyLib = new Harmony("com.marmods.YTTV");
            HarmonyLib.PatchAll(typeof(YTTV));
        }

        public void WriteLogo()
        {
            Logger.LogInfo((object)"YTTV Loaded!");
        }

        public static void DownloadFileSync(Uri uri, string filename)
        {
            try
            {
                if (File.Exists(filename)) File.Delete(filename);

                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                    webClient.DownloadFile(uri, filename);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void DownloadFiles(Uri uri, string filename)
        {
            WebClient webClient = new WebClient();
            webClient.DownloadFileCompleted += Web_DownloadFileCompleted;
            webClient.DownloadFileAsync(uri, filename);
        }

        private static void Web_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Thread.CurrentThread.Abort();
        }

        public void Log(object message)
        {
            Logger.LogInfo(message);
        }
    }

    public class Configs
    {
        public class Lang
        {
            public ConfigEntry<string> main_1;
            public ConfigEntry<string> main_2;
            public ConfigEntry<string> main_3;
            public ConfigEntry<string> main_4;
            public ConfigEntry<string> main_5;
            public ConfigEntry<string> main_6;
            public ConfigEntry<string> main_7;
            public ConfigEntry<string> main_8;
            public ConfigEntry<string> main_9;
            public ConfigEntry<string> main_10;
            public ConfigEntry<string> main_11;
            public ConfigEntry<string> main_12;
            public ConfigEntry<string> main_13;

            public void GetConfigRU()
            {
                ConfigFile val = new ConfigFile("YTTV\\lang\\television_ru.cfg", true);
                main_1 = val.Bind<string>("General", "Main_1", "Пожалуйста, подождите, загружаются дополнительные библиотеки...", (ConfigDescription)null);
                main_2 = val.Bind<string>("General", "Main_2", "Включить/Выключить Телевизор : [E]\n@2 - @3\n@1 громкость\nУвеличить громкость [PU]\nУменьшить громкость [PG]", (ConfigDescription)null);
                main_3 = val.Bind<string>("General", "Main_3", "Библиотеки загружены.", (ConfigDescription)null);
                main_4 = val.Bind<string>("General", "Main_4", "Видео загружается!", (ConfigDescription)null);
                main_5 = val.Bind<string>("General", "Main_5", "Команды:\n/tplay - Проиграть\n/ttime - Время\n/treset - Сброс\n/tposition - Позиция\n/tvolume - Громкость", (ConfigDescription)null);
                main_6 = val.Bind<string>("General", "Main_6", "Неверный URL!", (ConfigDescription)null);
                main_7 = val.Bind<string>("General", "Main_7", "Пожалуйста подождите...", (ConfigDescription)null);
                main_8 = val.Bind<string>("General", "Main_8", "Видео готово", (ConfigDescription)null);
                main_9 = val.Bind<string>("General", "Main_9", "@1 изменил громкость @2", (ConfigDescription)null);
                main_10 = val.Bind<string>("General", "Main_10", "Неверная громкость!", (ConfigDescription)null);
                main_11 = val.Bind<string>("General", "Main_11", "Ссылка недействительная!", (ConfigDescription)null);
                main_12 = val.Bind<string>("General", "Main_12", "Позиция: @1", (ConfigDescription)null);
                main_13 = val.Bind<string>("General", "Main_13", "Запоминание: @1", (ConfigDescription)null);
            }

            public void GetConfigEN()
            {
                ConfigFile val = new ConfigFile("YTTV\\lang\\television_en.cfg", true);
                main_1 = val.Bind<string>("General", "Main_1", "Please wait, downloading libraries...", (ConfigDescription)null);
                main_2 = val.Bind<string>("General", "Main_2", "Toggle TV : [E]\n@2 - @3\n@1 volume\nVol Up [PG UP]\nVol Down [PG DN]", (ConfigDescription)null);
                main_3 = val.Bind<string>("General", "Main_3", "Libraries loaded.", (ConfigDescription)null);
                main_4 = val.Bind<string>("General", "Main_4", "Video is loading!", (ConfigDescription)null);
                main_5 = val.Bind<string>("General", "Main_5", "/tplay [LINK]\n/ttime [TIME]\n/treset\n/tposition [BOOL]\n/tvolume [0-100]", (ConfigDescription)null);
                main_6 = val.Bind<string>("General", "Main_6", "Invalid URL!", (ConfigDescription)null);
                main_7 = val.Bind<string>("General", "Main_7", "Please wait...", (ConfigDescription)null);
                main_8 = val.Bind<string>("General", "Main_8", "Video loaded.", (ConfigDescription)null);
                main_9 = val.Bind<string>("General", "Main_9", "@1 set volume to @2", (ConfigDescription)null);
                main_10 = val.Bind<string>("General", "Main_10", "Invalid Volume!", (ConfigDescription)null);
                main_11 = val.Bind<string>("General", "Main_11", "Link is invalid!", (ConfigDescription)null);
                main_12 = val.Bind<string>("General", "Main_12", "Time: @1", (ConfigDescription)null);
                main_13 = val.Bind<string>("General", "Main_13", "Memory: @1", (ConfigDescription)null);
            }
        }

        public ConfigEntry<bool> requstbattery;
        public ConfigEntry<string> languages;
        public static Lang lang = new Lang();

        public void GetConfig()
        {
            string configPath = Path.Combine(Paths.ConfigPath, "YTTV.cfg");
            ConfigFile val = new ConfigFile(configPath, true);
            languages = val.Bind<string>("General", "Languages", "en", "en = English, ru = Русский");
        }

        public Lang GetLang()
        {
            return lang;
        }
    }

    public class YTTV
    {
        public static TVScript tvScript = new TVScript();
        public static string LastMessage;
        public static bool LoadingVideo = false;
        public static bool LoadingLibrary = false;
        public static string LastnameOfUserWhoTyped;
        public static string Lastname;
        public static double curretTime = 0.0;
        public static double totalTime = 0.0;
        public static bool positionSafe = false;
        public static double positionTime = 0.0;
        public static bool isPlayTelevision = false;
        public static double volumeMain = 0.0;

        [HarmonyPatch(typeof(PlayerControllerB), "SetHoverTipAndCurrentInteractTrigger")]
        [HarmonyPostfix]
        public static void SetHoverTipAndCurrentInteractTrigger(PlayerControllerB __instance)
        {
            InteractTrigger currentTrigger = __instance.hoveringOverTrigger;

            if (currentTrigger == null || !currentTrigger.interactable) return;

            // Check if this is the TV Trigger
            if (currentTrigger.transform.parent != null &&
               (currentTrigger.transform.parent.name.Contains("Television") || currentTrigger.hoverTip.Contains("Switch TV")))
            {
                if (!File.Exists("YTTV\\other\\yt-dlp.exe"))
                {
                    currentTrigger.hoverTip = Plugin.config.GetLang().main_1.Value;
                }
                else
                {
                    // Calculate Time Strings
                    float volume = tvScript.tvSFX.volume;
                    int curSeconds = (int)curretTime % 3600;
                    string curH = Mathf.Floor((float)((int)curretTime / 3600)).ToString("00");
                    string curM = Mathf.Floor((float)(curSeconds / 60)).ToString("00");
                    string curS = Mathf.Floor((float)(curSeconds % 60)).ToString("00");

                    int totSeconds = (int)totalTime % 3600;
                    string totH = Mathf.Floor((float)((int)totalTime / 3600)).ToString("00");
                    string totM = Mathf.Floor((float)(totSeconds / 60)).ToString("00");
                    string totS = Mathf.Floor((float)(totSeconds % 60)).ToString("00");

                    // Set the tooltip
                    currentTrigger.hoverTip = Plugin.config.GetLang().main_2.Value
                        .Replace("@1", $"{Math.Round(volume * 100f)}%")
                        .Replace("@2", $"{curH}:{curM}:{curS}")
                        .Replace("@3", $"{totH}:{totM}:{totS}");

                    // Handle Input (PageUp / PageDown)
                    if (((ButtonControl)Keyboard.current.pageDownKey).wasPressedThisFrame && volume > 0f)
                    {
                        float newVol = volume - 0.1f;
                        if (newVol < 0f) newVol = 0f;

                        tvScript.tvSFX.volume = newVol;
                        volumeMain = newVol;
                        ChangeVolume(new StreamWriter("YTTV\\cache"));
                    }
                    if (((ButtonControl)Keyboard.current.pageUpKey).wasPressedThisFrame && volume < 1f)
                    {
                        float newVol = volume + 0.1f;
                        if (newVol > 1f) newVol = 1f;

                        tvScript.tvSFX.volume = newVol;
                        volumeMain = newVol;
                        ChangeVolume(new StreamWriter("YTTV\\cache"));
                    }
                }
            }
        }

        public static void ChangeVolume(StreamWriter streamWriter)
        {
            streamWriter.WriteLine(volumeMain.ToString());
            streamWriter.Close();
        }

        [HarmonyPatch(typeof(HUDManager), "AddPlayerChatMessageServerRpc")]
        [HarmonyPostfix]
        [ServerRpc(RequireOwnership = false)]
        private static void AddPlayerChatMessageServerRpc(HUDManager __instance, string chatMessage, int playerId)
        {
            if (chatMessage.Length > 50)
            {
                ((object)__instance).GetType().GetMethod("AddPlayerChatMessageClientRpc", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(__instance, new object[2] { chatMessage, playerId });
            }
        }

        [HarmonyPatch(typeof(HUDManager), "AddChatMessage")]
        [HarmonyPrefix]
        private static bool AddChatMessage(HUDManager __instance, string chatMessage, string nameOfUserWhoTyped)
        {
            if (!(__instance.lastChatMessage == chatMessage))
            {
                __instance.lastChatMessage = chatMessage;
                __instance.PingHUDElement(__instance.Chat, 4f, 1f, 0.2f);
                if (__instance.ChatMessageHistory.Count >= 4)
                {
                    ((TMP_Text)__instance.chatText).text.Remove(0, __instance.ChatMessageHistory[0].Length);
                    __instance.ChatMessageHistory.Remove(__instance.ChatMessageHistory[0]);
                }
                StringBuilder stringBuilder = new StringBuilder(chatMessage);
                stringBuilder.Replace("[playerNum0]", StartOfRound.Instance.allPlayerScripts[0].playerUsername);
                stringBuilder.Replace("[playerNum1]", StartOfRound.Instance.allPlayerScripts[1].playerUsername);
                stringBuilder.Replace("[playerNum2]", StartOfRound.Instance.allPlayerScripts[2].playerUsername);
                stringBuilder.Replace("[playerNum3]", StartOfRound.Instance.allPlayerScripts[3].playerUsername);
                chatMessage = stringBuilder.ToString();
                string item = ((!string.IsNullOrEmpty(nameOfUserWhoTyped)) ? ("<color=#FF0000>" + nameOfUserWhoTyped + "</color>: <color=#FFFF00>'" + chatMessage + "'</color>") : ("<color=#7069ff>" + chatMessage + "</color>"));
                __instance.ChatMessageHistory.Add(item);
                ((TMP_Text)__instance.chatText).text = "";
                for (int i = 0; i < __instance.ChatMessageHistory.Count; i++)
                {
                    TextMeshProUGUI chatText = __instance.chatText;
                    ((TMP_Text)chatText).text = ((TMP_Text)chatText).text + "\n" + __instance.ChatMessageHistory[i];
                }
            }
            AddChatMessageMain(__instance, chatMessage, nameOfUserWhoTyped);
            return false;
        }

        [HarmonyPatch(typeof(HUDManager), "Update")]
        [HarmonyPrefix]
        private static void Updat(HUDManager __instance)
        {
            if (File.Exists("YTTV\\other\\yt-dlp.exe") && LoadingLibrary)
            {
                DrawString(__instance, Plugin.config.GetLang().main_3.Value, "Television", "Television");
                LoadingLibrary = false;
            }
        }

        public static async void AddChatMessageMain(HUDManager __instance, string chatMessage, string nameOfUserWhoTyped)
        {
            if (!File.Exists("YTTV\\other\\yt-dlp.exe"))
            {
                DrawString(__instance, Plugin.config.GetLang().main_1.Value, "Television", nameOfUserWhoTyped);
                LoadingLibrary = true;
                return;
            }
            string[] vs = chatMessage.Split(new char[1] { ' ' });
            switch (vs[0].Replace("/", ""))
            {
                case "thelp":
                    DrawString(__instance, Plugin.config.GetLang().main_5.Value, "Television", nameOfUserWhoTyped);
                    break;
                case "tplay":
                    if (new Regex("^https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$").IsMatch(vs[1]))
                    {
                        string text4 = vs[1].Remove(0, 8);
                        if (!(text4.Contains("youtube.com") || text4.Contains("youtu.be")))
                        {
                            break;
                        }
                        if (vs[1].Contains("list"))
                        {
                            DrawString(__instance, Plugin.config.GetLang().main_6.Value, "Television", nameOfUserWhoTyped);
                            break;
                        }
                        LoadingVideo = true;
                        if (File.Exists("YTTV\\other\\test.mp4"))
                        {
                            File.Delete("YTTV\\other\\test.mp4");
                        }
                        DrawString(__instance, Plugin.config.GetLang().main_7.Value, "Television", nameOfUserWhoTyped);
                        if (isPlayTelevision)
                        {
                            break;
                        }
                        isPlayTelevision = true;

                        await Task.Run(delegate
                        {
                            bool flag = false;
                            bool flag2 = false;
                            Process process = new Process();
                            process.StartInfo.FileName = "YTTV\\other\\yt-dlp.exe";
                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.Arguments = "--cookies \"cookies.txt\" -f \"bv*[height<=360][ext=mp4]+ba[ext=m4a]/b[height<=360][ext=mp4]/b[ext=mp4]\" --force-ipv4 -N 4 " + vs[1] + " -o test.%(ext)s";
                            process.StartInfo.WorkingDirectory = "YTTV\\other";
                            process.StartInfo.CreateNoWindow = true;
                            process.Start();

                            while (!flag)
                            {
                                if (flag2)
                                {
                                    if (File.Exists("YTTV\\other\\test.mp4"))
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                                else if (process.HasExited)
                                {
                                    if (File.Exists("YTTV\\other\\test.mp4.part") || File.Exists("YTTV\\other\\test.mp4"))
                                    {
                                        flag2 = true;
                                    }
                                    else if (!File.Exists("YTTV\\other\\test.mp4"))
                                    {
                                        DrawString(__instance, Plugin.config.GetLang().main_11.Value, "Television", nameOfUserWhoTyped);
                                        break;
                                    }
                                }
                                Thread.Sleep(1000);
                            }
                        });

                        if (!File.Exists("YTTV\\other\\test.mp4"))
                        {
                            LoadingVideo = false;
                            break;
                        }

                        if (tvScript.tvOn)
                        {
                            tvScript.tvOn = false;
                            SetTVScreenMaterial(tvScript, on: false);
                            tvScript.tvSFX.Stop();
                            tvScript.video.Stop();
                            tvScript.tvSFX.PlayOneShot(tvScript.switchTVOff);
                            WalkieTalkie.TransmitOneShotAudio(tvScript.tvSFX, tvScript.switchTVOff, 1f);
                        }
                        tvScript.video.url = "file:///" + Paths.GameRootPath + "\\YTTV\\other\\test.mp4";
                        tvScript.video.controlledAudioTrackCount = 1;
                        tvScript.video.audioOutputMode = (VideoAudioOutputMode)1;
                        tvScript.video.SetTargetAudioSource((ushort)0, tvScript.tvSFX);
                        tvScript.video.Stop();
                        tvScript.tvSFX.Stop();
                        LoadingVideo = false;
                        isPlayTelevision = false;
                        positionTime = 0.0;
                        DrawString(__instance, Plugin.config.GetLang().main_8.Value, "Television", nameOfUserWhoTyped);
                    }
                    else
                    {
                        DrawString(__instance, Plugin.config.GetLang().main_6.Value, "Television", nameOfUserWhoTyped);
                    }
                    break;
                case "ttime":
                    {
                        string[] array = vs[1].Split(new char[1] { ':' });
                        switch (array.Length)
                        {
                            case 2:
                                {
                                    if (!tvScript.tvOn) break;
                                    int num10 = Convert.ToInt32(array[0]);
                                    int num11 = Convert.ToInt32(array[1]);
                                    if (num10 == 0)
                                    {
                                        tvScript.video.time = num11;
                                        DrawString(__instance, Plugin.config.GetLang().main_12.Value.Replace("@1", $"00:00:{num11:00}"), "Television", nameOfUserWhoTyped);
                                    }
                                    else
                                    {
                                        tvScript.video.time = (num10 * 60) + num11;
                                        DrawString(__instance, Plugin.config.GetLang().main_12.Value.Replace("@1", $"00:{num10:00}:{num11:00}"), "Television", nameOfUserWhoTyped);
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    if (!tvScript.tvOn) break;
                                    int num2 = Convert.ToInt32(array[0]);
                                    int num3 = Convert.ToInt32(array[1]);
                                    int num4 = Convert.ToInt32(array[2]);
                                    tvScript.video.time = (num2 * 3600) + (num3 * 60) + num4;
                                    DrawString(__instance, Plugin.config.GetLang().main_12.Value.Replace("@1", $"{num2:00}:{num3:00}:{num4:00}"), "Television", nameOfUserWhoTyped);
                                    break;
                                }
                        }
                        break;
                    }
                case "tposition":
                    {
                        if (vs[1].ToLower() == "true") positionSafe = true;
                        if (vs[1].ToLower() == "false") positionSafe = false;
                        string text2 = (positionSafe ? "enabled" : "disabled");
                        DrawString(__instance, Plugin.config.GetLang().main_13.Value.Replace("@1", text2), "Television", nameOfUserWhoTyped);
                        break;
                    }
                case "treset":
                    if (tvScript.tvOn)
                    {
                        tvScript.video.time = 0.0;
                        DrawString(__instance, Plugin.config.GetLang().main_12.Value.Replace("@1", "00:00:00"), "Television", nameOfUserWhoTyped);
                    }
                    break;
                case "tvolume":
                    {
                        if (nameOfUserWhoTyped == GameNetworkManager.Instance.localPlayerController.playerUsername)
                        {
                            float volume = tvScript.tvSFX.volume;
                            float num = (float)(Convert.ToInt32(vs[1]) / 10) * 0.1f;
                            if (volume != num)
                            {
                                tvScript.tvSFX.volume = num;
                                volumeMain = num;
                                ChangeVolume(new StreamWriter("YTTV\\cache"));
                                DrawString(__instance, Plugin.config.GetLang().main_9.Value.Replace("@1", nameOfUserWhoTyped ?? "").Replace("@2", vs[1] + "%"), "Television", nameOfUserWhoTyped);
                            }
                        }
                        break;
                    }
            }
        }

        [HarmonyPatch(typeof(HUDManager), "SubmitChat_performed")]
        [HarmonyPrefix]
        private static bool SubmitChat_performed(HUDManager __instance, ref UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (!LoadingVideo)
            {
                __instance.localPlayer = GameNetworkManager.Instance.localPlayerController;
                CallbackContext val = context;
                if (((CallbackContext)(val)).performed && !((Object)__instance.localPlayer == (Object)null) && __instance.localPlayer.isTypingChat && ((((NetworkBehaviour)__instance.localPlayer).IsOwner && (!((NetworkBehaviour)__instance).IsServer || __instance.localPlayer.isHostPlayerObject)) || __instance.localPlayer.isTestingPlayer) && !__instance.localPlayer.isPlayerDead)
                {
                    if (!string.IsNullOrEmpty(__instance.chatTextField.text) && __instance.chatTextField.text.Length < 200)
                    {
                        __instance.AddTextToChatOnServer(__instance.chatTextField.text, (int)__instance.localPlayer.playerClientId);
                    }
                    for (int i = 0; i < StartOfRound.Instance.allPlayerScripts.Length; i++)
                    {
                        if (StartOfRound.Instance.allPlayerScripts[i].isPlayerControlled && Vector3.Distance(((Component)GameNetworkManager.Instance.localPlayerController).transform.position, ((Component)StartOfRound.Instance.allPlayerScripts[i]).transform.position) > 24.4f && (!GameNetworkManager.Instance.localPlayerController.holdingWalkieTalkie || !StartOfRound.Instance.allPlayerScripts[i].holdingWalkieTalkie))
                        {
                            __instance.playerCouldRecieveTextChatAnimator.SetTrigger("ping");
                            break;
                        }
                    }
                    __instance.localPlayer.isTypingChat = false;
                    __instance.chatTextField.text = "";
                    EventSystem.current.SetSelectedGameObject((GameObject)null);
                    __instance.PingHUDElement(__instance.Chat, 2f, 1f, 0.2f);
                    ((Behaviour)__instance.typingIndicator).enabled = false;
                    return false;
                }
            }
            return false;
        }

        public static void DrawString(HUDManager __instance, string chatMessage, string nameOfUserWhoTyped, string name)
        {
            if (!(LastMessage == chatMessage))
            {
                LastMessage = chatMessage;
                LastnameOfUserWhoTyped = name;
                __instance.PingHUDElement(__instance.Chat, 4f, 1f, 0.2f);
                if (__instance.ChatMessageHistory.Count >= 4)
                {
                    ((TMP_Text)__instance.chatText).text.Remove(0, __instance.ChatMessageHistory[0].Length);
                    __instance.ChatMessageHistory.Remove(__instance.ChatMessageHistory[0]);
                }
                StringBuilder stringBuilder = new StringBuilder(chatMessage);
                stringBuilder.Replace("[playerNum0]", StartOfRound.Instance.allPlayerScripts[0].playerUsername);
                stringBuilder.Replace("[playerNum1]", StartOfRound.Instance.allPlayerScripts[1].playerUsername);
                stringBuilder.Replace("[playerNum2]", StartOfRound.Instance.allPlayerScripts[2].playerUsername);
                stringBuilder.Replace("[playerNum3]", StartOfRound.Instance.allPlayerScripts[3].playerUsername);
                chatMessage = stringBuilder.ToString();
                string item = ((!string.IsNullOrEmpty(nameOfUserWhoTyped)) ? ("<color=#FF0000>" + nameOfUserWhoTyped + "</color>: <color=#FFFF00>'" + chatMessage + "'</color>") : ("<color=#7069ff>" + chatMessage + "</color>"));
                __instance.ChatMessageHistory.Add(item);
                ((TMP_Text)__instance.chatText).text = "";
                for (int i = 0; i < __instance.ChatMessageHistory.Count; i++)
                {
                    TextMeshProUGUI chatText = __instance.chatText;
                    ((TMP_Text)chatText).text = ((TMP_Text)chatText).text + "\n" + __instance.ChatMessageHistory[i];
                }
            }
        }

        [HarmonyPatch(typeof(HUDManager), "Start")]
        [HarmonyPostfix]
        private static void StartPostfix(HUDManager __instance)
        {
            __instance.chatTextField.characterLimit = 200;
        }

        [HarmonyPatch(typeof(TVScript), "TurnTVOnOff")]
        [HarmonyPrefix]
        public static bool TurnTVOnOff(TVScript __instance, bool on)
        {
            __instance.tvOn = on;
            if (on)
            {
                if (positionSafe)
                {
                    tvScript.video.time = positionTime;
                }
                SetTVScreenMaterial(__instance, on: true);
                __instance.video.Play();
                __instance.tvSFX.Play();
                __instance.tvSFX.PlayOneShot(__instance.switchTVOn);
                WalkieTalkie.TransmitOneShotAudio(__instance.tvSFX, __instance.switchTVOn, 1f);
            }
            else
            {
                if (positionSafe)
                {
                    positionTime = tvScript.video.time;
                }
                SetTVScreenMaterial(__instance, on: false);
                __instance.tvSFX.Stop();
                __instance.video.Stop();
                __instance.tvSFX.PlayOneShot(__instance.switchTVOff);
                WalkieTalkie.TransmitOneShotAudio(__instance.tvSFX, __instance.switchTVOff, 1f);
            }
            return false;
        }

        public static void SetTVScreenMaterial(TVScript __instance, bool on)
        {
            Material[] sharedMaterials = ((Renderer)__instance.tvMesh).sharedMaterials;
            if (on)
            {
                sharedMaterials[1] = __instance.tvOnMaterial;
            }
            else
            {
                sharedMaterials[1] = __instance.tvOffMaterial;
            }
            ((Renderer)__instance.tvMesh).sharedMaterials = sharedMaterials;
            ((Behaviour)__instance.tvLight).enabled = on;
        }

        [HarmonyPatch(typeof(TVScript), "TVFinishedClip")]
        [HarmonyPrefix]
        public static bool TVFinishedClip()
        {
            return false;
        }

        [HarmonyPatch(typeof(TVScript), "Update")]
        [HarmonyPrefix]
        public static bool Update()
        {
            curretTime = tvScript.video.time;
            totalTime = tvScript.video.length;
            return false;
        }

        [HarmonyPatch(typeof(TVScript), "OnEnable")]
        [HarmonyPrefix]
        public static bool Prefix(TVScript __instance)
        {
            tvScript = __instance;
            __instance.video.clip = null;
            __instance.tvSFX.clip = null;

            if (File.Exists("YTTV\\cache"))
            {
                using (StreamReader streamReader = new StreamReader("YTTV\\cache"))
                {
                    string value;
                    while ((value = streamReader.ReadLine()) != null)
                    {
                        __instance.tvSFX.volume = Convert.ToSingle(value);
                    }
                }
            }
            else
            {
                __instance.tvSFX.volume = 0.5f;
            }

            __instance.video.Stop();
            __instance.tvSFX.Stop();
            return false;
        }
    }
}