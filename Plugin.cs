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
//[assembly: TargetFramework(".NETStandard,Version=v2.1", FrameworkDisplayName = ".NET Standard 2.1")]
//[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
[module: UnverifiableCode]
//[module: RefSafetyRules(11)]
namespace YTTV
{
    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "MARMods.YTTV";

        public const string PLUGIN_NAME = "YTTV";

        // Update this whenever mod version changes
        public const string PLUGIN_VERSION = "1.0.2";
    }
}
namespace Television_Controller
{
    // Update this whenever mod version changes
    [BepInPlugin("MARMods.YTTV", "YTTV", "1.0.2")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin instance;

        private static Harmony HarmonyLib;

        public static Configs config;

        private void Awake()
        {
            //IL_011b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0125: Expected O, but got Unknown
            instance = this;
            config = new Configs();
            config.GetConfig();
            string text = config.languages.Value.ToLower();
            if (!(text == "ru"))
            {
                if (text == "en")
                {
                    config.GetLang().GetConfigEN();
                }
            }
            else
            {
                config.GetLang().GetConfigRU();
            }
            WriteLogo();
            if (!Directory.Exists("YTTV\\lang"))
            {
                Directory.CreateDirectory("YTTV\\lang");
            }
            if (!Directory.Exists("YTTV\\other"))
            {
                Directory.CreateDirectory("YTTV\\other");
            }
            if (File.Exists("YTTV\\other\\test.mp4"))
            {
                File.Delete("YTTV\\other\\test.mp4");
            }
            if (!File.Exists("YTTV\\cache"))
            {
                using (StreamWriter streamWriter = File.CreateText("YTTV\\cache"))
                {
                }
            }

            /*
            // Force mod to always re-download yt-dlp to replace previous exe
            new Thread((ThreadStart)delegate
            {
                DownloadFiles(new Uri("https://github.com/yt-dlp/yt-dlp/releases/download/2025.08.11/yt-dlp.exe"), "YTTV\\other\\yt-dlp.exe");
            }).Start();
            */

            string exePath = Path.Combine("YTTV", "other", "yt-dlp.exe");
            string versionPath = Path.Combine("YTTV", "other", "yt-dlp.version");
            string expectedVersion = "2025.08.11";

            bool needsUpdate = true;

            if (File.Exists(exePath) && File.Exists(versionPath))
            {
                string localVersion = File.ReadAllText(versionPath).Trim();
                if (localVersion == expectedVersion)
                    needsUpdate = false;
            }

            if (needsUpdate)
            {
                new Thread(() =>
                {
                    try
                    {
                        // Download yt-dlp.exe
                        DownloadFiles(
                            new Uri($"https://github.com/yt-dlp/yt-dlp/releases/download/{expectedVersion}/yt-dlp.exe"),
                            exePath
                        );

                        // Only write the version file if the exe exists after download
                        if (File.Exists(exePath))
                            File.WriteAllText(versionPath, expectedVersion);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"Failed to download yt-dlp.exe: {e.Message}");
                    }
                }).Start();
            }

            HarmonyLib = new Harmony("com.marmods.YTTV");
            HarmonyLib.PatchAll(typeof(YTTV));
        }

        public void WriteLogo()
        {
            Logger.LogInfo((object)"YTTV Loaded!");
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
                //IL_0006: Unknown result type (might be due to invalid IL or missing references)
                //IL_000c: Expected O, but got Unknown
                ConfigFile val = new ConfigFile("YTTV\\lang\\television_ru.cfg", true);
                main_1 = val.Bind<string>("General", "Main_1", "Пожалуйста, подождите, загружаются дополнительные библиотеки, чтобы модификация заработала.", (ConfigDescription)null);
                main_2 = val.Bind<string>("General", "Main_2", "Включить/Выключить Телевизор : [E]\n@2 - @3\n@1 громкость\nУвеличить громкость [PU]\nУменьшить громкость [PG]", (ConfigDescription)null);
                main_3 = val.Bind<string>("General", "Main_3", "Все дополнительные библиотеки загружены, теперь вы можете использовать команды для телевизора.", (ConfigDescription)null);
                main_4 = val.Bind<string>("General", "Main_4", "Подождите, видео еще загружается!", (ConfigDescription)null);
                main_5 = val.Bind<string>("General", "Main_5", "Команды:\n/tplay - Проиграть видео\n/ttime - Поставить позицию видео\n/treset - Сбросить позцию видео вначало\n/tposition - Включить/Выключить запоминания позиции после выключение телевизора\n/tvolume - Изменить громкость видео", (ConfigDescription)null);
                main_6 = val.Bind<string>("General", "Main_6", "Введите правильный URL-адрес!", (ConfigDescription)null);
                main_7 = val.Bind<string>("General", "Main_7", "Пожалуйста подождите...", (ConfigDescription)null);
                main_8 = val.Bind<string>("General", "Main_8", "Видео был загружен в телевизор", (ConfigDescription)null);
                main_9 = val.Bind<string>("General", "Main_9", "@1 изменил громкость видео @2", (ConfigDescription)null);
                main_10 = val.Bind<string>("General", "Main_10", "Введите правильную громкость видео (пример: 0, 10, 20, 30...)!", (ConfigDescription)null);
                main_11 = val.Bind<string>("General", "Main_11", "Ссылка недействительная!", (ConfigDescription)null);
                main_12 = val.Bind<string>("General", "Main_12", "Позиция видео изменена на @1!", (ConfigDescription)null);
                main_13 = val.Bind<string>("General", "Main_13", "Запоминание позиции @1!", (ConfigDescription)null);
            }

            public void GetConfigEN()
            {
                //IL_0006: Unknown result type (might be due to invalid IL or missing references)
                //IL_000c: Expected O, but got Unknown
                ConfigFile val = new ConfigFile("YTTV\\lang\\television_en.cfg", true);
                main_1 = val.Bind<string>("General", "Main_1", "Please wait, additional libraries are being loaded for the modification to work.", (ConfigDescription)null);
                main_2 = val.Bind<string>("General", "Main_2", "Enable/Disable Television : [E]\n@2 - @3\n@1 volume\nIncrease volume [PU]\nDecrease volume [PG]", (ConfigDescription)null);
                main_3 = val.Bind<string>("General", "Main_3", "All libraries have loaded, now you can use the television commands.", (ConfigDescription)null);
                main_4 = val.Bind<string>("General", "Main_4", "Another video is being uploaded to the television!", (ConfigDescription)null);
                main_5 = val.Bind<string>("General", "Main_5", "/tplay [LINK] - Play video from this link\n/ttime [TIMESTAMP] - Set video to this timestamp\n/treset - Restart video\n/tposition [TRUE/FALSE] - Toggle if video resets on TV on/off\n/tvolume [0-100] - Set TV volume to this percentage", (ConfigDescription)null);
                main_6 = val.Bind<string>("General", "Main_6", "Enter the correct URL!", (ConfigDescription)null);
                main_7 = val.Bind<string>("General", "Main_7", "Please wait...", (ConfigDescription)null);
                main_8 = val.Bind<string>("General", "Main_8", "The video was uploaded to the television", (ConfigDescription)null);
                main_9 = val.Bind<string>("General", "Main_9", "@1 changed the volume @2 of the television.", (ConfigDescription)null);
                main_10 = val.Bind<string>("General", "Main_10", "Enter the correct Volume (example: 0, 10, 20, 30...)!", (ConfigDescription)null);
                main_11 = val.Bind<string>("General", "Main_11", "Link is invalid!", (ConfigDescription)null);
                main_12 = val.Bind<string>("General", "Main_12", "Video position changed to @1!", (ConfigDescription)null);
                main_13 = val.Bind<string>("General", "Main_13", "Position memorization is @1!", (ConfigDescription)null);
            }
        }

        public ConfigEntry<bool> requstbattery;

        public ConfigEntry<string> languages;

        public static Lang lang = new Lang();

        public void GetConfig()
        {
            //IL_0006: Unknown result type (might be due to invalid IL or missing references)
            //IL_000c: Expected O, but got Unknown
            ConfigFile val = new ConfigFile("YTTV\\television_controller.cfg", true);
            languages = val.Bind<string>("General", "Languages", "en", "EN/RU/AR");
        }

        public Lang GetLang()
        {
            return lang;
        }
    }
    public class YTTV
    {
        public static TVScript tvScript = new TVScript();

        public static InteractTrigger tigger = new InteractTrigger();

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
        [HarmonyPrefix]
        public static bool SetHoverTipAndCurrentInteractTrigger(PlayerControllerB __instance, ref RaycastHit ___hit, ref Ray ___interactRay, ref int ___playerMask, ref int ___interactableObjectsMask)
        {
            //IL_0017: Unknown result type (might be due to invalid IL or missing references)
            //IL_0027: Unknown result type (might be due to invalid IL or missing references)
            //IL_002c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0031: Unknown result type (might be due to invalid IL or missing references)
            //IL_0037: Unknown result type (might be due to invalid IL or missing references)
            //IL_058d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0598: Expected O, but got Unknown
            //IL_0051: Unknown result type (might be due to invalid IL or missing references)
            //IL_0056: Unknown result type (might be due to invalid IL or missing references)
            //IL_006f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0074: Unknown result type (might be due to invalid IL or missing references)
            //IL_066a: Unknown result type (might be due to invalid IL or missing references)
            //IL_0093: Unknown result type (might be due to invalid IL or missing references)
            //IL_0098: Unknown result type (might be due to invalid IL or missing references)
            //IL_00aa: Unknown result type (might be due to invalid IL or missing references)
            //IL_00b5: Expected O, but got Unknown
            //IL_067f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0684: Unknown result type (might be due to invalid IL or missing references)
            //IL_069a: Unknown result type (might be due to invalid IL or missing references)
            //IL_06a5: Expected O, but got Unknown
            //IL_04c3: Unknown result type (might be due to invalid IL or missing references)
            //IL_04c8: Unknown result type (might be due to invalid IL or missing references)
            //IL_00bb: Unknown result type (might be due to invalid IL or missing references)
            //IL_00c0: Unknown result type (might be due to invalid IL or missing references)
            //IL_051f: Unknown result type (might be due to invalid IL or missing references)
            //IL_052a: Expected O, but got Unknown
            //IL_033e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0343: Unknown result type (might be due to invalid IL or missing references)
            //IL_0359: Unknown result type (might be due to invalid IL or missing references)
            //IL_0364: Unknown result type (might be due to invalid IL or missing references)
            //IL_036e: Expected O, but got Unknown
            //IL_036e: Expected O, but got Unknown
            //IL_00f0: Unknown result type (might be due to invalid IL or missing references)
            //IL_00fb: Expected O, but got Unknown
            //IL_0500: Unknown result type (might be due to invalid IL or missing references)
            //IL_050b: Expected O, but got Unknown
            //IL_0391: Unknown result type (might be due to invalid IL or missing references)
            //IL_039c: Expected O, but got Unknown
            //IL_0376: Unknown result type (might be due to invalid IL or missing references)
            //IL_0381: Expected O, but got Unknown
            //IL_03d1: Unknown result type (might be due to invalid IL or missing references)
            //IL_03dc: Expected O, but got Unknown
            RaycastHit val;
            if (!__instance.isGrabbingObjectAnimation)
            {
                ___interactRay = new Ray(((Component)__instance.gameplayCamera).transform.position, ((Component)__instance.gameplayCamera).transform.forward);
                if (Physics.Raycast(___interactRay, out ___hit, __instance.grabDistance, ___interactableObjectsMask))
                {
                    val = ___hit;
                    if (((Component)((RaycastHit)(val)).collider).gameObject.layer != 8)
                    {
                        val = ___hit;
                        string tag = ((Component)((RaycastHit)(val)).collider).tag;
                        if (!(tag == "PhysicsProp"))
                        {
                            val = ___hit;
                            if ((Object)((Component)((RaycastHit)(val)).transform).gameObject.GetComponent<InteractTrigger>() != (Object)null)
                            {
                                val = ___hit;
                                InteractTrigger component = ((Component)((RaycastHit)(val)).transform).gameObject.GetComponent<InteractTrigger>();
                                if (component.hoverTip == "Switch TV: [LMB]")
                                {
                                    tigger = component;
                                }
                                if ((Object)tigger != (Object)null)
                                {
                                    if (!File.Exists("YTTV\\other\\yt-dlp.exe"))
                                    {
                                        tigger.hoverTip = Plugin.config.GetLang().main_1.Value;
                                    }
                                    else
                                    {
                                        float volume = tvScript.tvSFX.volume;
                                        int num = (int)curretTime % 3600;
                                        string text = Mathf.Floor((float)((int)curretTime / 3600)).ToString("00");
                                        string text2 = Mathf.Floor((float)(num / 60)).ToString("00");
                                        string text3 = Mathf.Floor((float)(num % 60)).ToString("00");
                                        int num2 = (int)totalTime % 3600;
                                        string text4 = Mathf.Floor((float)((int)totalTime / 3600)).ToString("00");
                                        string text5 = Mathf.Floor((float)(num2 / 60)).ToString("00");
                                        string text6 = Mathf.Floor((float)(num2 % 60)).ToString("00");
                                        tigger.hoverTip = Plugin.config.GetLang().main_2.Value.Replace("@1", $"{Math.Round(volume * 100f)}%").Replace("@2", text + ":" + text2 + ":" + text3).Replace("@3", text4 + ":" + text5 + ":" + text6);
                                        if (((ButtonControl)Keyboard.current.pageDownKey).wasPressedThisFrame && volume > 0f)
                                        {
                                            float num3 = volume - 0.1f;
                                            tvScript.tvSFX.volume = num3;
                                            volumeMain = num3;
                                            ChangeVolume(new StreamWriter("YTTV\\cache"));
                                        }
                                        if (((ButtonControl)Keyboard.current.pageUpKey).wasPressedThisFrame && volume < 1f)
                                        {
                                            float num4 = volume + 0.1f;
                                            tvScript.tvSFX.volume = num4;
                                            volumeMain = num4;
                                            ChangeVolume(new StreamWriter("YTTV\\cache"));
                                        }
                                    }
                                }
                            }
                            if (tag == "InteractTrigger")
                            {
                                val = ___hit;
                                InteractTrigger component2 = ((Component)((RaycastHit)(val)).transform).gameObject.GetComponent<InteractTrigger>();
                                if ((Object)component2 != (Object)__instance.previousHoveringOverTrigger && (Object)__instance.previousHoveringOverTrigger != (Object)null)
                                {
                                    __instance.previousHoveringOverTrigger.isBeingHeldByPlayer = false;
                                }
                                if (!((Object)component2 == (Object)null))
                                {
                                    __instance.hoveringOverTrigger = component2;
                                    if (!component2.interactable)
                                    {
                                        __instance.cursorIcon.sprite = component2.disabledHoverIcon;
                                        ((Behaviour)__instance.cursorIcon).enabled = (Object)component2.disabledHoverIcon != (Object)null;
                                        ((TMP_Text)__instance.cursorTip).text = component2.disabledHoverTip;
                                    }
                                    else if (component2.isPlayingSpecialAnimation)
                                    {
                                        ((Behaviour)__instance.cursorIcon).enabled = false;
                                        ((TMP_Text)__instance.cursorTip).text = "";
                                    }
                                    else if (__instance.isHoldingInteract)
                                    {
                                        if (__instance.twoHanded)
                                        {
                                            ((TMP_Text)__instance.cursorTip).text = "[Hands full]";
                                        }
                                        else if (!string.IsNullOrEmpty(component2.holdTip))
                                        {
                                            ((TMP_Text)__instance.cursorTip).text = component2.holdTip;
                                        }
                                    }
                                    else
                                    {
                                        ((Behaviour)__instance.cursorIcon).enabled = true;
                                        __instance.cursorIcon.sprite = component2.hoverIcon;
                                        ((TMP_Text)__instance.cursorTip).text = component2.hoverTip;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (FirstEmptyItemSlot(__instance) == -1)
                            {
                                ((TMP_Text)__instance.cursorTip).text = "Inventory full!";
                            }
                            else
                            {
                                val = ___hit;
                                GrabbableObject component3 = ((Component)((RaycastHit)(val)).collider).gameObject.GetComponent<GrabbableObject>();
                                if (!GameNetworkManager.Instance.gameHasStarted && !component3.itemProperties.canBeGrabbedBeforeGameStart && (Object)StartOfRound.Instance.testRoom == (Object)null)
                                {
                                    ((TMP_Text)__instance.cursorTip).text = "(Cannot pickup until ship is landed)";
                                }
                                if ((Object)component3 != (Object)null && !string.IsNullOrEmpty(component3.customGrabTooltip))
                                {
                                    ((TMP_Text)__instance.cursorTip).text = component3.customGrabTooltip;
                                }
                            }
                            ((Behaviour)__instance.cursorIcon).enabled = true;
                            __instance.cursorIcon.sprite = __instance.grabItemIcon;
                        }
                        goto IL_05ad;
                    }
                }
                ((Behaviour)__instance.cursorIcon).enabled = false;
                ((TMP_Text)__instance.cursorTip).text = "";
                if ((Object)__instance.hoveringOverTrigger != (Object)null)
                {
                    __instance.previousHoveringOverTrigger = __instance.hoveringOverTrigger;
                }
                __instance.hoveringOverTrigger = null;
            }
            goto IL_05ad;
        IL_05ad:
            if (StartOfRound.Instance.localPlayerUsingController)
            {
                StringBuilder stringBuilder = new StringBuilder(((TMP_Text)__instance.cursorTip).text);
                stringBuilder.Replace("[E]", "[X]");
                stringBuilder.Replace("[LMB]", "[X]");
                stringBuilder.Replace("[RMB]", "[R-Trigger]");
                stringBuilder.Replace("[F]", "[R-Shoulder]");
                stringBuilder.Replace("[Z]", "[L-Shoulder]");
                ((TMP_Text)__instance.cursorTip).text = stringBuilder.ToString();
            }
            else
            {
                ((TMP_Text)__instance.cursorTip).text = ((TMP_Text)__instance.cursorTip).text.Replace("[LMB]", "[E]");
            }
            if (!__instance.isFreeCamera && Physics.Raycast(___interactRay, out ___hit, 5f, ___playerMask))
            {
                val = ___hit;
                PlayerControllerB component4 = ((Component)((RaycastHit)(val)).collider).gameObject.GetComponent<PlayerControllerB>();
                if ((Object)component4 != (Object)null)
                {
                    component4.ShowNameBillboard();
                }
            }
            return false;
        }

        public static void ChangeVolume(StreamWriter streamWriter)
        {
            streamWriter.WriteLine(volumeMain.ToString());
        }

        public static int FirstEmptyItemSlot(PlayerControllerB __instance)
        {
            //IL_000f: Unknown result type (might be due to invalid IL or missing references)
            //IL_001a: Expected O, but got Unknown
            //IL_0031: Unknown result type (might be due to invalid IL or missing references)
            //IL_003c: Expected O, but got Unknown
            int result = -1;
            if ((Object)__instance.ItemSlots[__instance.currentItemSlot] == (Object)null)
            {
                result = __instance.currentItemSlot;
            }
            else
            {
                for (int i = 0; i < __instance.ItemSlots.Length; i++)
                {
                    if ((Object)__instance.ItemSlots[i] == (Object)null)
                    {
                        result = i;
                        break;
                    }
                }
            }
            return result;
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
                    if (Plugin.config.languages.Value.ToLower().Equals("en"))
                    {
                        DrawString(__instance, Plugin.config.GetLang().main_5.Value, "Television", nameOfUserWhoTyped);
                    }
                    if (Plugin.config.languages.Value.ToLower().Equals("ru"))
                    {
                        DrawString(__instance, Plugin.config.GetLang().main_5.Value, "Television", nameOfUserWhoTyped);
                    }
                    break;
                case "tplay":
                    if (new Regex("^https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$").IsMatch(vs[1]))
                    {
                        string text4 = vs[1].Remove(0, 8);
                        if (!(text4.Substring(0, text4.IndexOf('/')) == "www.youtube.com"))
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
                            process.StartInfo.Arguments = "--cookies \"cookies.txt\" -f \"[height <=360][ext = mp4] / wv[ext = mp4]\" --extractor-args \"youtube:player_client = tv\" --force-ipv4 -N 16 " + vs[1] + " -o test.%(ext)s";
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
                                else if (Process.GetProcessById(process.Id).HasExited)
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
                                    if (!tvScript.tvOn)
                                    {
                                        break;
                                    }
                                    int num10 = Convert.ToInt32(array[0]);
                                    int num11 = Convert.ToInt32(array[1]);
                                    switch (num10)
                                    {
                                        case 0:
                                            switch (num11)
                                            {
                                                case 0:
                                                    tvScript.video.time = 0.0;
                                                    DrawString(__instance, Plugin.config.GetLang().main_12.Value.Replace("@1", "00:00:00"), "Television", nameOfUserWhoTyped);
                                                    break;
                                                case 1:
                                                case 2:
                                                case 3:
                                                case 4:
                                                case 5:
                                                case 6:
                                                case 7:
                                                case 8:
                                                case 9:
                                                case 10:
                                                case 11:
                                                case 12:
                                                case 13:
                                                case 14:
                                                case 15:
                                                case 16:
                                                case 17:
                                                case 18:
                                                case 19:
                                                case 20:
                                                case 21:
                                                case 22:
                                                case 23:
                                                case 24:
                                                case 25:
                                                case 26:
                                                case 27:
                                                case 28:
                                                case 29:
                                                case 30:
                                                case 31:
                                                case 32:
                                                case 33:
                                                case 34:
                                                case 35:
                                                case 36:
                                                case 37:
                                                case 38:
                                                case 39:
                                                case 40:
                                                case 41:
                                                case 42:
                                                case 43:
                                                case 44:
                                                case 45:
                                                case 46:
                                                case 47:
                                                case 48:
                                                case 49:
                                                case 50:
                                                case 51:
                                                case 52:
                                                case 53:
                                                case 54:
                                                case 55:
                                                case 56:
                                                case 57:
                                                case 58:
                                                case 59:
                                                    tvScript.video.time = num11;
                                                    DrawString(__instance, Plugin.config.GetLang().main_12.Value.Replace("@1", "00:00:" + num11.ToString("00")), "Television", nameOfUserWhoTyped);
                                                    break;
                                            }
                                            break;
                                        case 1:
                                        case 2:
                                        case 3:
                                        case 4:
                                        case 5:
                                        case 6:
                                        case 7:
                                        case 8:
                                        case 9:
                                        case 10:
                                        case 11:
                                        case 12:
                                        case 13:
                                        case 14:
                                        case 15:
                                        case 16:
                                        case 17:
                                        case 18:
                                        case 19:
                                        case 20:
                                        case 21:
                                        case 22:
                                        case 23:
                                        case 24:
                                        case 25:
                                        case 26:
                                        case 27:
                                        case 28:
                                        case 29:
                                        case 30:
                                        case 31:
                                        case 32:
                                        case 33:
                                        case 34:
                                        case 35:
                                        case 36:
                                        case 37:
                                        case 38:
                                        case 39:
                                        case 40:
                                        case 41:
                                        case 42:
                                        case 43:
                                        case 44:
                                        case 45:
                                        case 46:
                                        case 47:
                                        case 48:
                                        case 49:
                                        case 50:
                                        case 51:
                                        case 52:
                                        case 53:
                                        case 54:
                                        case 55:
                                        case 56:
                                        case 57:
                                        case 58:
                                        case 59:
                                            {
                                                int num12 = num10 * 60;
                                                if (num11 > 0 && num11 < 60)
                                                {
                                                    int num13 = num12 + num11;
                                                    tvScript.video.time = num13;
                                                    DrawString(__instance, Plugin.config.GetLang().main_12.Value.Replace("@1", "00:" + num10.ToString("00") + ":" + num11.ToString("00")), "Television", nameOfUserWhoTyped);
                                                }
                                                else
                                                {
                                                    tvScript.video.time = num12;
                                                    DrawString(__instance, Plugin.config.GetLang().main_12.Value.Replace("@1", "00:" + num10.ToString("00") + ":00"), "Television", nameOfUserWhoTyped);
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    if (!tvScript.tvOn)
                                    {
                                        break;
                                    }
                                    int num2 = Convert.ToInt32(array[0]);
                                    int num3 = Convert.ToInt32(array[1]);
                                    int num4 = Convert.ToInt32(array[2]);
                                    switch (num2)
                                    {
                                        case 0:
                                            if (num3 == 0 && num4 == 0)
                                            {
                                                tvScript.video.time = 0.0;
                                                DrawString(__instance, Plugin.config.GetLang().main_12.Value.Replace("@1", "00:00:00"), "Television", nameOfUserWhoTyped);
                                            }
                                            else if (num3 > 0 && num3 < 60)
                                            {
                                                int num8 = num3 * 60;
                                                if (num4 > 0 && num4 < 60)
                                                {
                                                    int num9 = num8 + num4;
                                                    tvScript.video.time = num9;
                                                    DrawString(__instance, Plugin.config.GetLang().main_12.Value.Replace("@1", "00:" + num3.ToString("00") + ":" + num4.ToString("00")), "Television", nameOfUserWhoTyped);
                                                }
                                                else
                                                {
                                                    tvScript.video.time = num8;
                                                    DrawString(__instance, Plugin.config.GetLang().main_12.Value.Replace("@1", "00:" + num3.ToString("00") + ":00"), "Television", nameOfUserWhoTyped);
                                                }
                                            }
                                            break;
                                        case 1:
                                        case 2:
                                            {
                                                int num5 = num2 * 3600;
                                                if (num3 > 0 && num3 < 60)
                                                {
                                                    int num6 = num5 + num3 * 60;
                                                    if (num4 > 0 && num4 < 60)
                                                    {
                                                        int num7 = num6 + num4;
                                                        tvScript.video.time = num7;
                                                        DrawString(__instance, Plugin.config.GetLang().main_12.Value.Replace("@1", num2.ToString("00") + ":" + num3.ToString("00") + ":" + num4.ToString("00")), "Television", nameOfUserWhoTyped);
                                                    }
                                                    else
                                                    {
                                                        tvScript.video.time = num6;
                                                        DrawString(__instance, Plugin.config.GetLang().main_12.Value.Replace("@1", num2.ToString("00") + ":" + num3.ToString("00") + ":00"), "Television", nameOfUserWhoTyped);
                                                    }
                                                }
                                                else
                                                {
                                                    tvScript.video.time = num5;
                                                    DrawString(__instance, Plugin.config.GetLang().main_12.Value.Replace("@1", num2.ToString("00") + ":00:00"), "Television", nameOfUserWhoTyped);
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case "tposition":
                    {
                        string text = Plugin.config.languages.Value.ToLower();
                        if (!(text == "ru"))
                        {
                            if (text == "en")
                            {
                                if (vs[1].ToLower() == "true")
                                {
                                    positionSafe = true;
                                }
                                if (vs[1].ToLower() == "false")
                                {
                                    positionSafe = false;
                                }
                                string text2 = (positionSafe ? "enabled" : "disabled");
                                DrawString(__instance, Plugin.config.GetLang().main_13.Value.Replace("@1", text2 ?? ""), "Television", nameOfUserWhoTyped);
                            }
                        }
                        else
                        {
                            if (vs[1].ToLower() == "true")
                            {
                                positionSafe = true;
                            }
                            if (vs[1].ToLower() == "false")
                            {
                                positionSafe = false;
                            }
                            string text3 = (positionSafe ? "включено" : "выключено");
                            DrawString(__instance, Plugin.config.GetLang().main_13.Value.Replace("@1", text3 ?? ""), "Television", nameOfUserWhoTyped);
                        }
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
                        float volume = tvScript.tvSFX.volume;
                        float num = (float)(Convert.ToInt32(vs[1]) / 10) * 0.1f;
                        if (volume != num && (volume < num || volume > num))
                        {
                            tvScript.tvSFX.volume = num;
                            volumeMain = num;
                            ChangeVolume(new StreamWriter("YTTV\\cache"));
                            DrawString(__instance, Plugin.config.GetLang().main_9.Value.Replace("@1", nameOfUserWhoTyped ?? "").Replace("@2", vs[1] + "%"), "Television", nameOfUserWhoTyped);
                        }
                        break;
                    }
            }
        }

        [HarmonyPatch(typeof(HUDManager), "SubmitChat_performed")]
        [HarmonyPrefix]
        private static bool SubmitChat_performed(HUDManager __instance, ref UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            //IL_001b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0020: Unknown result type (might be due to invalid IL or missing references)
            //IL_0033: Unknown result type (might be due to invalid IL or missing references)
            //IL_003e: Expected O, but got Unknown
            //IL_0104: Unknown result type (might be due to invalid IL or missing references)
            //IL_011a: Unknown result type (might be due to invalid IL or missing references)
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

        [HarmonyPatch(typeof(HUDManager), "Update")]
        [HarmonyPostfix]
        private static void StartPostfixs(HUDManager __instance)
        {
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
            return false; // prevent original OnEnable
        }
    }
    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "Television_Controller";

        public const string PLUGIN_NAME = "Television_Controller";

        // Update this whenever mod version changes
        public const string PLUGIN_VERSION = "1.0.2";
    }
}
namespace System.Runtime.CompilerServices
{
    [CompilerGenerated]
    [Microsoft.CodeAnalysis.Embedded]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, AllowMultiple = false, Inherited = false)]
    internal sealed class NullableAttribute : Attribute
    {
        public readonly byte[] NullableFlags;

        public NullableAttribute(byte P_0)
        {
            NullableFlags = new byte[1] { P_0 };
        }

        public NullableAttribute(byte[] P_0)
        {
            NullableFlags = P_0;
        }
    }
    [CompilerGenerated]
    [Microsoft.CodeAnalysis.Embedded]
    [AttributeUsage(AttributeTargets.Module, AllowMultiple = false, Inherited = false)]
    internal sealed class RefSafetyRulesAttribute : Attribute
    {
        public readonly int Version;

        public RefSafetyRulesAttribute(int P_0)
        {
            Version = P_0;
        }
    }
}
namespace Microsoft.CodeAnalysis
{
    [CompilerGenerated]
    [Microsoft.CodeAnalysis.Embedded]
    internal sealed class EmbeddedAttribute : Attribute
    {
    }
}