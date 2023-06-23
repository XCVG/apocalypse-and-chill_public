using CommonCore;
using CommonCore.Audio;
using CommonCore.Experimental;
using CommonCore.LockPause;
using CommonCore.RpgGame.Dialogue;
using CommonCore.State;
using CommonCore.Video;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Anc
{

    public class AncMainSequence : MonoBehaviour
    {

        private Coroutine CurrentCoroutine;
        private AudioPlayer AudioPlayer;
        [SerializeField]
        private VideoPlayer VideoPlayer;

        private void Start()
        {
            AudioPlayer = CCBase.GetModule<AudioModule>().AudioPlayer;

            SetDebugState();

            SetInitialState();
            //SetMusic();

            BeginNextSequence();
        }

        private void SetDebugState()
        {
            if (!CoreParams.IsDebug)
                return;

            if (GameState.Instance.CampaignState.GetQuestStage("AncMainQuest") == 0)
            {
                GameState.Instance.CampaignState.SetQuestStage("AncMainQuest", 10); //CHANGE THIS FOR TESTING
            }
        }

        private void SetInitialState()
        {
            
        }

        public void BeginNextSequence()
        {
            var stage = GameState.Instance.CampaignState.GetQuestStage("AncMainQuest");
            switch (stage)
            {
                case 0:
                case 10:
                    StartCoroutine(CoIntro());
                    break;
                case 100:
                    StartCoroutine(CoIntro2());
                    break;
                case 200:
                    StartCoroutine(CoFightScene());
                    break;
                case 220:
                    StartCoroutine(CoFightScene2());
                    break;
                case 230:
                    StartCoroutine(CoFightScene3());
                    break;
                case 240:
                    StartCoroutine(CoFightScene4());
                    break;
                case 250:
                    StartCoroutine(CoFightScene5());
                    break;
                case 260:
                    StartCoroutine(CoFightScene6());
                    break;
                case 300:
                    StartCoroutine(CoPostFightScene());
                    break;
                case 400:
                    StartCoroutine(CoParkNightScene());
                    break;
                case 500:
                    StartCoroutine(CoMorningScene());
                    break;
                case 600:
                    StartCoroutine(CoLunchScene());
                    break;
                case 700:
                    StartCoroutine(CoBathroomScene());
                    break;
                case 800:
                    StartCoroutine(CoEveningScene1());
                    break;
                case 820:
                    StartCoroutine(CoEveningScene2());
                    break;
                case 830:
                    StartCoroutine(CoEveningScene3());
                    break;
                case 900:
                    StartCoroutine(CoBattleMontage());
                    break;
                case 1000:
                    StartCoroutine(CoEveningPostFight());
                    break;
                case 2000:
                    StartCoroutine(CoPostGame());
                    break;
                default:
                    Debug.LogWarning("No sequence found for quest stage " + stage);
                    break;
            }
        }

        private IEnumerator CoIntro()
        {
            AudioPlayer.PlayMusic("electronic", MusicSlot.Event, 0.75f, true, false);

            yield return DialogueInitiator.RunDialogueCoroutine("AncIntro", false);
            var slideshow = SlideshowControllerEx.GetInstance();
            slideshow.ShowImage("SteveGaming");

            yield return new WaitForSecondsEx(5f, false, PauseLockType.AllowCutscene);

            ScreenFader.FadeTo(Color.black, 3f, PauseLockType.AllowCutscene);
            MusicFader.FadeOut(MusicSlot.Event, 3f);
            yield return new WaitForSecondsEx(5f, false, PauseLockType.AllowCutscene);

            GameState.Instance.CampaignState.SetQuestStage("AncMainQuest", 100);
            SaveUtils.DoAutoSave();
            BeginNextSequence();
        }

        private IEnumerator CoIntro2()
        {
            var slideshow = SlideshowControllerEx.GetInstance();
            slideshow.ShowImage("SteveGaming2");

            AudioPlayer.PlayMusic("tension", MusicSlot.Event, 0.01f, true, false);
            ScreenFader.FadeFrom(Color.black, 3f, PauseLockType.AllowCutscene);
            MusicFader.FadeIn(MusicSlot.Event, 0.75f, 3f);
            yield return new WaitForSecondsEx(5f, false, PauseLockType.AllowCutscene);

            slideshow.ClearImage();

            yield return DialogueInitiator.RunDialogueCoroutine("AncIntro2", false);

            //more sequence?
            slideshow.ShowImage("SteveBedroom");

            AudioPlayer.PlaySound("Run", SoundType.Sound, false);
            //MusicFader.FadeOut(MusicSlot.Event, 3.0f);
            ScreenFader.FadeTo(Color.black, 3f, PauseLockType.AllowCutscene);

            yield return new WaitForSecondsEx(3.1f, false, PauseLockType.AllowCutscene);

            slideshow.ClearImage();

            GameState.Instance.CampaignState.SetQuestStage("AncMainQuest", 200);
            SaveUtils.DoAutoSave();
            BeginNextSequence();
        }

        private IEnumerator CoFightScene()
        {
            ScreenFader.ClearFade();
            AudioPlayer.PlayMusic("tension", MusicSlot.Event, 0.75f, true, false);

            yield return DialogueInitiator.RunDialogueCoroutine("AncPreFight", false);


            GameState.Instance.CampaignState.SetQuestStage("AncMainQuest", 220);
            BeginNextSequence();
        }

        private IEnumerator CoFightScene2()
        {
            AudioPlayer.PlayMusic("tension", MusicSlot.Event, 0.75f, true, false);

            //Mitch shoots
            var slideshow = SlideshowControllerEx.GetInstance();
            slideshow.ClearImage();

            slideshow.ShowImage("FightShoot1");
            yield return new WaitForSecondsEx(0.5f, false, PauseLockType.AllowCutscene, true);

            AudioPlayer.PlaySound("CFPistol2", SoundType.Sound, false); //reuse pistol sound from california
            slideshow.ShowImage("FightShoot2");
            yield return new WaitForSecondsEx(0.07f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightShoot3");
            yield return new WaitForSecondsEx(0.05f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightShoot4");
            yield return new WaitForSecondsEx(0.05f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightShoot5");
            yield return new WaitForSecondsEx(0.05f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightShoot6");
            yield return new WaitForSecondsEx(0.08f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightShoot7");
            yield return new WaitForSecondsEx(0.08f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightShoot8");
            yield return new WaitForSecondsEx(0.08f, false, PauseLockType.AllowCutscene, true);


            slideshow.ShowImage("FightShoot1A");
            yield return new WaitForSecondsEx(0.2f, false, PauseLockType.AllowCutscene, true);

            AudioPlayer.PlaySound("CFPistol2", SoundType.Sound, false); //reuse pistol sound from california
            slideshow.ShowImage("FightShoot2");
            yield return new WaitForSecondsEx(0.07f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightShoot3");
            yield return new WaitForSecondsEx(0.05f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightShoot4");
            yield return new WaitForSecondsEx(0.05f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightShoot5");
            yield return new WaitForSecondsEx(0.05f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightShoot6");
            yield return new WaitForSecondsEx(0.08f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightShoot7");
            yield return new WaitForSecondsEx(0.08f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightShoot8");
            yield return new WaitForSecondsEx(0.08f, false, PauseLockType.AllowCutscene, true);

            slideshow.ShowImage("FightShoot1A");
            yield return new WaitForSecondsEx(0.2f, false, PauseLockType.AllowCutscene, true);

            AudioPlayer.PlaySound("CFPistol2", SoundType.Sound, false); //reuse pistol sound from california
            slideshow.ShowImage("FightShoot2");
            yield return new WaitForSecondsEx(0.07f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightShoot3");
            yield return new WaitForSecondsEx(0.05f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightShoot4");
            yield return new WaitForSecondsEx(0.05f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightShoot5");
            yield return new WaitForSecondsEx(0.05f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightShoot6");
            yield return new WaitForSecondsEx(0.08f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightShoot7");
            yield return new WaitForSecondsEx(0.08f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightShoot8");
            yield return new WaitForSecondsEx(0.08f, false, PauseLockType.AllowCutscene, true);

            slideshow.ShowImage("FightShoot1A");
            yield return new WaitForSecondsEx(0.25f, false, PauseLockType.AllowCutscene, true);

            yield return null;

            //shade react sequence

            slideshow.ShowImage("FightReact1");
            yield return new WaitForSecondsEx(0.2f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightReact2");
            yield return new WaitForSecondsEx(0.2f, false, PauseLockType.AllowCutscene, true);

            AudioPlayer.PlaySound("CFPistol2", SoundType.Sound, false);
            AudioPlayer.PlaySound("ShadeImpactBad", SoundType.Sound, false);
            slideshow.ShowImage("FightReact3");
            yield return new WaitForSecondsEx(0.2f, false, PauseLockType.AllowCutscene, true);


            slideshow.ShowImage("FightReact4");
            yield return new WaitForSecondsEx(0.2f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightReact5");
            yield return new WaitForSecondsEx(0.2f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightReact6");
            yield return new WaitForSecondsEx(0.2f, false, PauseLockType.AllowCutscene, true);

            AudioPlayer.PlaySound("CFPistol2", SoundType.Sound, false);
            AudioPlayer.PlaySound("ShadeImpactBad", SoundType.Sound, false);
            slideshow.ShowImage("FightReact7");
            yield return new WaitForSecondsEx(0.2f, false, PauseLockType.AllowCutscene, true);

            slideshow.ShowImage("FightReact8");
            yield return new WaitForSecondsEx(0.2f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightReact9");
            yield return new WaitForSecondsEx(0.2f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightReact10");
            yield return new WaitForSecondsEx(0.2f, false, PauseLockType.AllowCutscene, true);

            AudioPlayer.PlaySound("CFPistol2", SoundType.Sound, false);
            AudioPlayer.PlaySound("ShadeImpactBad", SoundType.Sound, false);
            slideshow.ShowImage("FightReact11");
            yield return new WaitForSecondsEx(0.2f, false, PauseLockType.AllowCutscene, true);

            slideshow.ShowImage("FightReact12");
            yield return new WaitForSecondsEx(0.2f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightReact13");
            yield return new WaitForSecondsEx(0.2f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightReact14");
            yield return new WaitForSecondsEx(0.33f, false, PauseLockType.AllowCutscene, true);

            slideshow.ClearImage();

            GameState.Instance.CampaignState.SetQuestStage("AncMainQuest", 230);
            BeginNextSequence();
        }

        private IEnumerator CoFightScene3()
        {
            AudioPlayer.PlayMusic("tension", MusicSlot.Event, 0.75f, true, false);

            //Shades bum-rush
            var slideshow = SlideshowControllerEx.GetInstance();
            slideshow.ClearImage();

            //shade approach

            const float approachInterval = 0.3f;

            AudioPlayer.PlaySound("FightAttackSequence1", SoundType.Sound, false); //will be unholy scream, full loudness gunshot

            slideshow.ShowImage("FightStandoff1");
            yield return new WaitForSecondsEx(approachInterval, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightStandoff2");
            yield return new WaitForSecondsEx(approachInterval, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightStandoff3");
            yield return new WaitForSecondsEx(approachInterval, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightStandoff4");
            yield return new WaitForSecondsEx(approachInterval, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightStandoff5");
            AudioPlayer.PlaySound("Fall0", SoundType.Sound, false);
            yield return new WaitForSecondsEx(approachInterval, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightStandoff6");
            yield return new WaitForSecondsEx(approachInterval, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightStandoff7");
            yield return new WaitForSecondsEx(approachInterval, false, PauseLockType.AllowCutscene, true);

            //2.1s elapsed

            //Steve perspective
            slideshow.ShowImage("FightAttack1");

            //start to black out, pitch shift the music down, play the muffled sounds
            AudioPlayer.PlaySound("FightAttackSequence2", SoundType.Sound, false); //will be unholy scream, a few gunshots, heartbeat, fading/pitching down
            const float fadeTime = 7f;
            //const float prewarm = 1f;
            //float rate = 1f / fadeTime;

            var mPlayer = AudioPlayer.gameObject.transform.FindDeepChild("MusicPlayer_1").GetComponent<AudioSource>();

            yield return null;
            for(float elapsed = 0; elapsed < fadeTime; elapsed += Time.deltaTime)
            {
                float v = elapsed / fadeTime;
                ScreenFader.SetColor(new Color(0, 0, 0, v), false, false);
                float m = 0.5f + ((1 - v) / 2f);
                //Debug.Log(m);
                mPlayer.pitch = m;

                yield return null;
            }

            AudioPlayer.StopMusic(MusicSlot.Event);
            ScreenFader.SetColor(Color.black);
            mPlayer.pitch = 1;

            //yield return new WaitForSecondsEx(2f, false, PauseLockType.AllowCutscene, true);
            yield return null;

            GameState.Instance.CampaignState.SetQuestStage("AncMainQuest", 240);
            BeginNextSequence();
        }

        private IEnumerator CoFightScene4()
        {
            ScreenFader.SetColor(Color.black);

            var slideshow = SlideshowControllerEx.GetInstance();
            slideshow.ShowImage("FightAttack2");

            yield return new WaitForSecondsEx(3f, false, PauseLockType.AllowCutscene, true);

            //fade to white, then fade in, powerful music and magic sound

            //TODO sound and music
            AudioPlayer.PlaySound("FightTransform", SoundType.Sound, false);

            ScreenFader.FadeTo(Color.white, 2f, PauseLockType.AllowCutscene);
            yield return new WaitForSecondsEx(2.1f, false, PauseLockType.AllowCutscene, true);
            AudioPlayer.PlayMusic("power", MusicSlot.Event, 0.75f, true, false);
            ScreenFader.FadeFrom(Color.white, 2f, PauseLockType.AllowCutscene);
            yield return new WaitForSecondsEx(2.1f, false, PauseLockType.AllowCutscene, true);

            //white flash, smash cut to shade being blown across the room and obliterated

            //part1: blow the shade across the room
            //AudioPlayer.PlaySound("FightMagicTemp", SoundType.Sound, false);
            AudioPlayer.PlaySound("MagicBetter", SoundType.Sound, false);
            const float d1Interval = 0.15f;
            slideshow.ShowImage("FightDestroy1");
            yield return new WaitForSecondsEx(d1Interval, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightDestroy2");
            yield return new WaitForSecondsEx(d1Interval, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightDestroy3");
            yield return new WaitForSecondsEx(d1Interval, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightDestroy4");
            yield return new WaitForSecondsEx(d1Interval, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightDestroy5");
            yield return new WaitForSecondsEx(d1Interval, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightDestroy6");
            yield return new WaitForSecondsEx(d1Interval, false, PauseLockType.AllowCutscene, true);
            AudioPlayer.PlaySound("FightDestroyWall", SoundType.Sound, false);
            slideshow.ShowImage("FightDestroy7");
            yield return new WaitForSecondsEx(d1Interval, false, PauseLockType.AllowCutscene, true);            
            slideshow.ShowImage("FightDestroy8");
            yield return new WaitForSecondsEx(d1Interval, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightDestroy9");
            yield return new WaitForSecondsEx(d1Interval, false, PauseLockType.AllowCutscene, true);
            //1.35s total...

            yield return new WaitForSecondsEx(0.25f, false, PauseLockType.AllowCutscene, true);
            //1.6s with the extra 0.25s of hold

            //part2: kill the shade attacking mitch
            const float d2Interval = 0.2f;
            
            slideshow.ShowImage("FightDestroyA1");
            yield return new WaitForSecondsEx(1f, false, PauseLockType.AllowCutscene, true);
            AudioPlayer.PlaySound("FightMagicShort", SoundType.Sound, false);
            yield return new WaitForSecondsEx(d2Interval, false, PauseLockType.AllowCutscene, true);
            
            slideshow.ShowImage("FightDestroyA2");
            yield return new WaitForSecondsEx(d2Interval, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightDestroyA3");
            yield return new WaitForSecondsEx(d2Interval, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightDestroyA4");
            yield return new WaitForSecondsEx(d2Interval, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightDestroyA5");
            yield return new WaitForSecondsEx(d2Interval, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightDestroyA6");
            yield return new WaitForSecondsEx(d2Interval, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightDestroyA7");
            yield return new WaitForSecondsEx(d2Interval, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightDestroyA8");
            yield return new WaitForSecondsEx(d2Interval, false, PauseLockType.AllowCutscene, true);

            yield return new WaitForSecondsEx(1.2f, false, PauseLockType.AllowCutscene, true);

            //show Steph in magical form in all her glory
            const float hInterval = 0.25f;

            slideshow.ShowImage("FightHero1");
            yield return new WaitForSecondsEx(hInterval, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightHero2");
            yield return new WaitForSecondsEx(hInterval, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightHero3");
            yield return new WaitForSecondsEx(hInterval, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightHero4");
            yield return new WaitForSecondsEx(hInterval, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightHero5");
            yield return new WaitForSecondsEx(hInterval, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightHero6");
            yield return new WaitForSecondsEx(hInterval, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightHero7");
            yield return new WaitForSecondsEx(hInterval, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightHero8");
            yield return new WaitForSecondsEx(hInterval, false, PauseLockType.AllowCutscene, true);

            yield return new WaitForSecondsEx(1f, false, PauseLockType.AllowCutscene, true);

            MusicFader.FadeOut(MusicSlot.Event, 3f);
            yield return new WaitForSecondsEx(3.1f, false, PauseLockType.AllowCutscene, true);

            GameState.Instance.CampaignState.SetQuestStage("AncMainQuest", 250);
            SaveUtils.DoAutoSave();
            BeginNextSequence();
        }

        private IEnumerator CoFightScene5()
        {
            //brief discussion including a closeup of the hole with more shades visible
            //Mitch is like "think you can do that again?"

            var slideshow = SlideshowControllerEx.GetInstance();
            slideshow.ClearImage();

            // music change
            yield return null;
            AudioPlayer.PlayMusic("mystery", MusicSlot.Event, 1f, true, false);

            yield return DialogueInitiator.RunDialogueCoroutine("AncFightInter", false, null);

            yield return null;

            GameState.Instance.CampaignState.SetQuestStage("AncMainQuest", 260);
            //SaveUtils.DoAutoSave();
            BeginNextSequence();
        }

        private IEnumerator CoFightScene6()
        {
            var slideshow = SlideshowControllerEx.GetInstance();
            slideshow.ClearImage();

            yield return null;

            //music
            AudioPlayer.PlayMusic("mystery", MusicSlot.Event, 1f, true, false);

            slideshow.ShowImage("FightReturn4");
            yield return new WaitForSecondsEx(1f, false, PauseLockType.AllowCutscene, true);
            AudioPlayer.PlaySound("FightMagic2", SoundType.Sound, false);
            slideshow.ShowImage("FightReturn5");
            yield return new WaitForSecondsEx(0.33f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightReturn6");
            yield return new WaitForSecondsEx(0.33f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightReturn7");
            yield return new WaitForSecondsEx(0.33f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightReturn8");
            yield return new WaitForSecondsEx(0.33f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightReturn9");
            yield return new WaitForSecondsEx(2f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("FightReturnPeek");
            yield return new WaitForSecondsEx(2f, false, PauseLockType.AllowCutscene, true);

            GameState.Instance.CampaignState.SetQuestStage("AncMainQuest", 300);
            SaveUtils.DoAutoSave();
            BeginNextSequence();
        }

        private IEnumerator CoPostFightScene()
        {
            var slideshow = SlideshowControllerEx.GetInstance();
            slideshow.ClearImage();

            // music
            AudioPlayer.PlayMusic("mystery", MusicSlot.Event, 1f, true, false);

            yield return DialogueInitiator.RunDialogueCoroutine("AncPostFight", false, null);

            //TODO fade?

            GameState.Instance.CampaignState.SetQuestStage("AncMainQuest", 400);
            //SaveUtils.DoAutoSave();
            BeginNextSequence();
        }

        private IEnumerator CoParkNightScene()
        {
            var slideshow = SlideshowControllerEx.GetInstance();
            slideshow.ClearImage();

            ScreenFader.SetColor(Color.black, true, false);
            slideshow.ShowImage("ParkNight");

            AudioPlayer.PlayMusic("park", MusicSlot.Event, 1.0f, true, false);

            ScreenFader.FadeFrom(Color.black, 2f, PauseLockType.AllowCutscene);            

            yield return new WaitForSecondsEx(2.1f, false, PauseLockType.AllowCutscene);

            yield return DialogueInitiator.RunDialogueCoroutine("AncParkNight", false, null);

            ScreenFader.FadeTo(Color.black, 5f, PauseLockType.AllowCutscene);
            MusicFader.FadeOut(MusicSlot.Event, 5f, false, false);
            yield return new WaitForSecondsEx(5.1f, false, PauseLockType.AllowCutscene);

            AudioPlayer.ClearMusic(MusicSlot.Event);

            GameState.Instance.CampaignState.SetQuestStage("AncMainQuest", 500);
            SaveUtils.DoAutoSave();
            BeginNextSequence();
        }

        private IEnumerator CoMorningScene()
        {
            var slideshow = SlideshowControllerEx.GetInstance();
            slideshow.ClearImage();

            ScreenFader.SetColor(Color.black, true, false);
            slideshow.ShowImage("Polytechnic");

            //music
            AudioPlayer.PlayMusic("school", MusicSlot.Event, 1.0f, true, false);

            ScreenFader.FadeFrom(Color.black, 5f, PauseLockType.AllowCutscene);

            yield return new WaitForSecondsEx(5.1f, false, PauseLockType.AllowCutscene);

            yield return DialogueInitiator.RunDialogueCoroutine("AncMorning", false, null);

            //fades?
            ScreenFader.FadeTo(Color.black, 3f, PauseLockType.AllowCutscene);
            //MusicFader.FadeOut(MusicSlot.Event, 5f, false, false);
            yield return new WaitForSecondsEx(3.1f, false, PauseLockType.AllowCutscene);

            GameState.Instance.CampaignState.SetQuestStage("AncMainQuest", 600);
            SaveUtils.DoAutoSave();
            BeginNextSequence();
        }

        private IEnumerator CoLunchScene()
        {
            var slideshow = SlideshowControllerEx.GetInstance();
            slideshow.ClearImage();

            ScreenFader.SetColor(Color.black, true, false);
            slideshow.ShowImage("Cafeteria");

            //music
            AudioPlayer.PlayMusic("school", MusicSlot.Event, 1.0f, true, false);

            ScreenFader.FadeFrom(Color.black, 3f, PauseLockType.AllowCutscene);

            yield return new WaitForSecondsEx(3.1f, false, PauseLockType.AllowCutscene);

            yield return DialogueInitiator.RunDialogueCoroutine("AncLunch", false, null);

            //fades?
            ScreenFader.FadeTo(Color.black, 3f, PauseLockType.AllowCutscene);
            MusicFader.FadeOut(MusicSlot.Event, 3f, false, false);
            yield return new WaitForSecondsEx(3.1f, false, PauseLockType.AllowCutscene);

            GameState.Instance.CampaignState.SetQuestStage("AncMainQuest", 700);
            SaveUtils.DoAutoSave();
            BeginNextSequence();
        }

        private IEnumerator CoBathroomScene()
        {
            var slideshow = SlideshowControllerEx.GetInstance();
            slideshow.ClearImage();

            ScreenFader.SetColor(Color.black, true, false);
            slideshow.ShowImage("BathroomDefault");

            // music
            AudioPlayer.PlayMusic("sadness", MusicSlot.Event, 0.01f, true, false);

            ScreenFader.FadeFrom(Color.black, 3f, PauseLockType.AllowCutscene);
            MusicFader.FadeIn(MusicSlot.Event, 1.0f, 3f);

            yield return new WaitForSecondsEx(3.1f, false, PauseLockType.AllowCutscene);

            //first dialogue

            yield return DialogueInitiator.RunDialogueCoroutine("AncBathroomMonologue", false, null);

            yield return new WaitForSecondsEx(0.5f, false, PauseLockType.AllowCutscene, true);

            //animation

            slideshow.ShowImage("BathroomDefault");
            yield return new WaitForSecondsEx(0.15f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("BathroomAnim1");
            yield return new WaitForSecondsEx(0.12f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("BathroomAnim2");
            yield return new WaitForSecondsEx(0.12f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("BathroomAnim3");
            yield return new WaitForSecondsEx(0.12f, false, PauseLockType.AllowCutscene, true);
            AudioPlayer.PlaySound("MirrorBreak", SoundType.Sound, false);
            slideshow.ShowImage("BathroomAnim4");
            yield return new WaitForSecondsEx(0.12f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("BathroomAnim5");
            yield return new WaitForSecondsEx(0.12f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("BathroomAnim6");
            yield return new WaitForSecondsEx(0.12f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("BathroomAnim7");
            yield return new WaitForSecondsEx(0.12f, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("BathroomSmashed");
            yield return new WaitForSecondsEx(2f, false, PauseLockType.AllowCutscene, true);

            slideshow.ShowImage("BathroomWide");
            yield return DialogueInitiator.RunDialogueCoroutine("AncBathroom2", false, null);

            //fades?
            ScreenFader.FadeTo(Color.black, 3f, PauseLockType.AllowCutscene);
            MusicFader.FadeOut(MusicSlot.Event, 3f, false, false);
            yield return new WaitForSecondsEx(3.1f, false, PauseLockType.AllowCutscene);

            GameState.Instance.CampaignState.SetQuestStage("AncMainQuest", 800);
            SaveUtils.DoAutoSave();
            BeginNextSequence();
        }

        private IEnumerator CoEveningScene1() //pre-transform conversation
        {
            var slideshow = SlideshowControllerEx.GetInstance();
            slideshow.ClearImage();

            ScreenFader.SetColor(Color.black, true, false);
            slideshow.ShowImage("Park");

            AudioPlayer.PlayMusic("park", MusicSlot.Event, 0.01f, true, false);
            MusicFader.FadeIn(MusicSlot.Event, 1.0f, 3f);
            ScreenFader.FadeFrom(Color.black, 3f, PauseLockType.AllowCutscene);

            yield return new WaitForSecondsEx(3.1f, false, PauseLockType.AllowCutscene);

            yield return DialogueInitiator.RunDialogueCoroutine("AncEvening1", false, null);

            GameState.Instance.CampaignState.SetQuestStage("AncMainQuest", 820);
            //SaveUtils.DoAutoSave();
            BeginNextSequence();
        }

        private IEnumerator CoEveningScene2() //transformation sequence?
        {
            var slideshow = SlideshowControllerEx.GetInstance();
            slideshow.ClearImage();

            slideshow.ShowImage("Park");

            AudioPlayer.StopMusic(MusicSlot.Event);

            yield return null;

            float t = 0.15f;
            slideshow.ShowImage("EveningTransform1");
            AudioPlayer.PlaySound("Transform2", SoundType.Sound, false);
            yield return new WaitForSecondsEx(0.2f, false, PauseLockType.AllowCutscene, true);

            slideshow.ShowImage("EveningTransform2");
            yield return new WaitForSecondsEx(t, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("EveningTransform3");
            yield return new WaitForSecondsEx(t, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("EveningTransform4");
            yield return new WaitForSecondsEx(t, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("EveningTransform5");
            yield return new WaitForSecondsEx(t, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("EveningTransform6");
            yield return new WaitForSecondsEx(t, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("EveningTransform7");
            yield return new WaitForSecondsEx(t, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("EveningTransform8");
            yield return new WaitForSecondsEx(t, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("EveningTransform9");
            yield return new WaitForSecondsEx(t, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("EveningTransform10");
            yield return new WaitForSecondsEx(t, false, PauseLockType.AllowCutscene, true);
            slideshow.ShowImage("EveningTransform11");
            yield return new WaitForSecondsEx(t, false, PauseLockType.AllowCutscene, true);

            slideshow.ShowImage("EveningTransform12");
            yield return new WaitForSecondsEx(2f, false, PauseLockType.AllowCutscene, true);

            //total time is 10 * t + 0.2 + 2
            //so 1.5 + 0.2 + 2 = 3.7s total
            // 0.2s windup, 1.5s transition, 2s hold

            yield return null;

            GameState.Instance.CampaignState.SetQuestStage("AncMainQuest", 830);
            //SaveUtils.DoAutoSave();
            BeginNextSequence();
        }

        private IEnumerator CoEveningScene3() //Tiff and Nguyen come back
        {
            var slideshow = SlideshowControllerEx.GetInstance();
            slideshow.ClearImage();

            slideshow.ShowImage("Park");

            //music
            AudioPlayer.PlayMusic("magic", MusicSlot.Event, 0.75f, true, false);
            yield return null;

            yield return DialogueInitiator.RunDialogueCoroutine("AncEvening3", false, null);
            slideshow.ShowImage("ParkShades");

            yield return null;

            //NOP for now

            GameState.Instance.CampaignState.SetQuestStage("AncMainQuest", 900);
            SaveUtils.DoAutoSave();
            BeginNextSequence();
        }

        private IEnumerator CoBattleMontage()
        {
            var slideshow = SlideshowControllerEx.GetInstance();
            slideshow.ClearImage();

            //slideshow.ShowImage("Park");

            //music            
            AudioPlayer.PlayMusic("montage", MusicSlot.Event, 0.75f, false, false);

            yield return null;

            slideshow.ShowImage("BattleComicTiff");
            ScreenFader.FadeFrom(Color.black, 1.0f, PauseLockType.AllowCutscene);
            yield return new WaitForSecondsEx(1f, false, PauseLockType.AllowCutscene);
            AudioPlayer.PlaySound("MontageTiff", SoundType.Sound, false);
            yield return new WaitForSecondsEx(10f, false, PauseLockType.AllowCutscene);

            slideshow.ShowImage("BattleComicNguyen");
            AudioPlayer.PlaySound("MontageNguyen2", SoundType.Sound, false);
            yield return new WaitForSecondsEx(10f, false, PauseLockType.AllowCutscene);

            slideshow.ShowImage("BattleComicSteph");
            AudioPlayer.PlaySound("MontageSteph", SoundType.Sound, false);
            yield return new WaitForSecondsEx(10f, false, PauseLockType.AllowCutscene);
            ScreenFader.FadeTo(Color.black, 1.0f, PauseLockType.AllowCutscene);

            yield return new WaitForSecondsEx(1.1f, false, PauseLockType.AllowCutscene);
            slideshow.ClearImage();
            ScreenFader.ClearFade();

            AudioPlayer.StopMusic(MusicSlot.Event);

            GameState.Instance.CampaignState.SetQuestStage("AncMainQuest", 1000);
            SaveUtils.DoAutoSave();
            BeginNextSequence();
        }

        private IEnumerator CoEveningPostFight()
        {
            var slideshow = SlideshowControllerEx.GetInstance();
            slideshow.ClearImage();

            slideshow.ShowImage("Park");

            //back to the overused park music
            AudioPlayer.PlayMusic("park", MusicSlot.Event, 1.0f, true, false);

            yield return DialogueInitiator.RunDialogueCoroutine("AncEveningPostFight", false, null);

            ScreenFader.FadeTo(Color.black, 5.0f, PauseLockType.AllowCutscene);
            MusicFader.FadeOut(MusicSlot.Event, 5.0f);
            yield return new WaitForSecondsEx(5.1f, false, PauseLockType.AllowCutscene);
            slideshow.ClearImage();
            ScreenFader.ClearFade();

            GameState.Instance.CampaignState.SetQuestStage("AncMainQuest", 2000);
            
            //SaveUtils.DoAutoSave();
            SetEndgameState();
            BeginNextSequence(); //we are now in this weird state where we are technically still in a game session but the game is effectively over
        }

        private void SetEndgameState()
        {
            GameState.Instance.CampaignState.SetQuestStage("AncPostgameState", 1);

            SaveUtils.DoFinalSave();

            PersistState.Instance.LastCampaignIdentifier = null;
            PersistState.Save();

            GameState.Instance.SaveLocked = true;
        }

        private IEnumerator CoPostGame()
        {
            var slideshow = SlideshowControllerEx.GetInstance();
            slideshow.ClearImage();

            AudioPlayer.ClearAllMusic();
            AudioPlayer.ClearAllSounds();

            yield return null;

            VideoUtils.SetupVideoPlayer(VideoPlayer, "credits");

            yield return VideoUtils.WaitForPlayback(VideoPlayer, () => LockPauseModule.IsPaused());

            yield return null;

            VideoPlayer.gameObject.SetActive(false);

            yield return new WaitForSecondsEx(1f, false, PauseLockType.AllowCutscene);

            ScreenFader.SetColor(Color.black);
            slideshow.ShowImage("DeloraFarm");            

            yield return null;
            AudioPlayer.PlayMusic("school", MusicSlot.Event, 0.01f, true, false);

            ScreenFader.FadeFrom(Color.black, 5f, PauseLockType.AllowCutscene);
            MusicFader.FadeIn(MusicSlot.Event, 1f, 5f, false, false);
            yield return new WaitForSecondsEx(5.1f, false, PauseLockType.AllowCutscene);

            yield return DialogueInitiator.RunDialogueCoroutine("AncStinger", false, null);

            ScreenFader.FadeTo(Color.black, 5f, PauseLockType.AllowCutscene);
            MusicFader.FadeOut(MusicSlot.Event, 5f, false, false);
            yield return new WaitForSecondsEx(5.1f, false, PauseLockType.AllowCutscene);

            AudioPlayer.ClearMusic(MusicSlot.Event);

            SharedUtils.EndGame();
        }

    }
}