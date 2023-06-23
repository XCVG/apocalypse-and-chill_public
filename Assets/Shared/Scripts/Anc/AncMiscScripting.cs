using CommonCore;
using CommonCore.Audio;
using CommonCore.LockPause;
using CommonCore.Scripting;
using CommonCore.State;
using CommonCore.UI;
using CommonCore.World;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Vnx;

namespace Anc
{
    
    public static class AncConsoleCommands
    {
        [Command(useClassName = false)]
        public static void ShowMainMenu()
        {
            var mainMenuController = CoreUtils.GetUIRoot().GetComponentInChildren<MainMenuController>();
            if(mainMenuController != null)
            {
                var mainMenuT = mainMenuController.transform;
                mainMenuT.Find("MenuBar").gameObject.SetActive(true);
                mainMenuT.Find("SubMenuPanelContainer").gameObject.SetActive(true);
                mainMenuT.Find("AltMainMenuSystem").gameObject.SetActive(false);
            }
        }
    }


    public static class AncScripts
    {
        [CCScript]
        private static void IntroScratchingNoises(ScriptExecutionContext context)
        {
            PlaySoundAndWait(context, "Scratching");
        }

        [CCScript]
        private static void IntroEntryNoises(ScriptExecutionContext context)
        {
            PlaySoundAndWait(context, "BreakIn");
        }

        [CCScript]
        private static void FightSlamDoor(ScriptExecutionContext context)
        {
            PlaySoundAndWait(context, "DoorSlam");

            var effect = WorldUtils.SpawnEffect("DoorBreakEffect", Vector3.zero, Quaternion.identity, null, true);
            effect.name = "DoorBreakEffect";
        }

        [CCScript]
        private static void FightClearDoor(ScriptExecutionContext context)
        {
            UnityEngine.Object.Destroy(WorldUtils.FindObjectByTID("DoorBreakEffect"));

            PlaySoundAndWait(context, "DoorDestroy");
        }

        [CCScript]
        private static void FightGunSounds1(ScriptExecutionContext context)
        {
            CCBase.GetModule<AudioModule>().AudioPlayer.PlaySound("GunLoading1", SoundType.Sound, false);
        }

        [CCScript]
        private static void FightGunSounds2(ScriptExecutionContext context)
        {
            CCBase.GetModule<AudioModule>().AudioPlayer.PlaySound("GunLoading2", SoundType.Sound, false);
        }

        [CCScript]
        private static void FightScreams(ScriptExecutionContext context)
        {
            PlaySoundAndWait(context, "FightScreams");
        }

        [CCScript]
        private static void FightOminousNoise(ScriptExecutionContext context)
        {
            PlaySoundAndWait(context, "FightScreams");
        }

        [CCScript]
        private static void EveningTryTransform(ScriptExecutionContext context)
        {
            PlaySoundAndWait(context, "TryTransform");
        }

        [CCScript]
        private static void EveningDetransformStart(ScriptExecutionContext context)
        {
            PlaySoundAndWait(context, "TryTransform");
        }

        [CCScript]
        private static void EveningDetransform(ScriptExecutionContext context)
        {
            var vnxController = context.Caller as VnxController;            
            //var image = steveTransform.GetComponent<Image>();

            vnxController.StartCoroutine(coEveningDetransform());

            IEnumerator coEveningDetransform()
            {
                vnxController.ContinueButton.gameObject.SetActive(false);
                yield return null;
                var steveTransform = vnxController.Stage.FindDeepChild("steve");
                var image = steveTransform.GetComponent<Image>();

                CCBase.GetModule<AudioModule>().AudioPlayer.PlaySound("Detransform", SoundType.Sound, false);

                image.sprite = getSprite("stransform_1");
                yield return new WaitForSecondsEx(0.12f, false, PauseLockType.AllowCutscene, true);
                image.sprite = getSprite("stransform_2");
                yield return new WaitForSecondsEx(0.12f, false, PauseLockType.AllowCutscene, true);
                image.sprite = getSprite("stransform_3");
                yield return new WaitForSecondsEx(0.12f, false, PauseLockType.AllowCutscene, true);
                image.sprite = getSprite("stransform_4");
                yield return new WaitForSecondsEx(0.12f, false, PauseLockType.AllowCutscene, true);
                image.sprite = getSprite("stransform_5");
                yield return new WaitForSecondsEx(0.12f, false, PauseLockType.AllowCutscene, true);
                image.sprite = getSprite("stransform_6");
                yield return new WaitForSecondsEx(0.12f, false, PauseLockType.AllowCutscene, true);
                image.sprite = getSprite("stransform_7");
                yield return new WaitForSecondsEx(0.12f, false, PauseLockType.AllowCutscene, true);
                image.sprite = getSprite("stransform_8");
                yield return new WaitForSecondsEx(0.12f, false, PauseLockType.AllowCutscene, true);

                yield return new WaitForSecondsEx(1f, false, PauseLockType.AllowCutscene, true);

                vnxController.ContinueButton.gameObject.SetActive(true);
            }

            Sprite getSprite(string spriteName)
            {
                return CoreUtils.LoadResource<Sprite>("dialogue/char/" + spriteName);
            }
        }

        [CCScript]
        private static bool HasAllFlags(ScriptExecutionContext context, string flags)
        {
            return !flags
                .Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Select(s => GameState.Instance.CampaignState.HasFlag(s))
                .Any(s => !s);
            //what the fuck lol
            
        }

        private static void PlaySoundAndWait(ScriptExecutionContext context, string sound)
        {
            //Debug.Log($"PlaySoundAndWait: {sound}");

            var audioModule = CCBase.GetModule<AudioModule>();
            var audioClip = audioModule.GetSound(sound, SoundType.Any);
            if(audioClip)
            {
                var vnxController = context.Caller as VnxController;
                bool ignorePause = LockPauseModule.IsPaused();
                var soundInfo = audioModule.AudioPlayer.PlaySoundEx(audioClip, false, ignorePause, false, false, 1.0f, Vector3.zero);
                if(audioClip.length > 0)
                {
                    vnxController.StartCoroutine(CoWaitForSound());
                }

                IEnumerator CoWaitForSound()
                {
                    vnxController.ContinueButton.gameObject.SetActive(false);
                    yield return null;

                    while(soundInfo.Source.isPlaying)
                    {
                        yield return null;
                    }

                    vnxController.ContinueButton.gameObject.SetActive(true);
                }
            }
            else
            {
                Debug.LogWarning($"[PlaySoundAndWait] can't find sound \"{sound}\"");
            }
            
        }

    }

}

