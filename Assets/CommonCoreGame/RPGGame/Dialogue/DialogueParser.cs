﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using CommonCore.DelayedEvents;
using CommonCore.RpgGame.State;

namespace CommonCore.RpgGame.Dialogue
{
    
    /// <summary>
    /// Parser for dialogue JSON files
    /// </summary>
    public static class DialogueParser
    {

        //TODO split out non-dialogue parsing from the DIALOGUE parser

        public static DialogueScene LoadDialogue(string dialogueName)
        {
            TextAsset ta = CoreUtils.LoadResource<TextAsset>("Data/Dialogue/" + dialogueName);
            return LoadDialogueFromString(dialogueName, ta.text);
        }

        public static DialogueScene LoadDialogueFromString(string dialogueName, string text)
        {
            JObject jo = JObject.Parse(text);
            //Debug.Log(jo);

            //parse root node (scene)
            string sBackground = string.Empty;
            string sImage = string.Empty;
            string sMusic = null;
            string sNext = string.Empty;
            string sText = string.Empty;
            string sName = string.Empty;
            string sNextText = string.Empty;
            string sCameraDir = string.Empty;
            FrameImagePosition sPosition = FrameImagePosition.Center;
            FrameOptions sOptions = new FrameOptions();
            FrameScripts sScripts = new FrameScripts();
            IReadOnlyDictionary<string, object> sExtraData = null;
            if (jo["background"] != null)
                sBackground = jo["background"].Value<string>();
            if (jo["image"] != null)
                sImage = jo["image"].Value<string>();
            if (jo["music"] != null && jo["music"].Type != JTokenType.Null)
            {
                sMusic = jo["music"].Value<string>();
                if (sMusic == null)
                    sMusic = string.Empty;
            }
            if (jo["default"] != null)
                sNext = jo["default"].Value<string>();
            if (jo["text"] != null)
                sText = jo["text"].Value<string>();
            if (jo["nameText"] != null)
                sName = jo["nameText"].Value<string>();
            if (jo["nextText"] != null)
                sNextText = jo["nextText"].Value<string>();
            if (jo["position"] != null)
            {
                if (Enum.TryParse(jo["position"].Value<string>(), true, out FrameImagePosition pos))
                    sPosition = pos;
            }
            if(!jo["options"].IsNullOrEmpty())
            {
                sOptions = ParseFrameOptions(jo["options"], null);
            }
            if (!jo["scripts"].IsNullOrEmpty())
            {
                sScripts = ParseFrameScripts(jo["scripts"], null);
            }
            if (!jo["ExtraData"].IsNullOrEmpty())
            {
                sExtraData = ParseExtraData(jo["ExtraData"], null);
            }

            Dictionary<string, Frame> frames = new Dictionary<string, Frame>();
            var scene = new DialogueScene(frames, sNext, sMusic);

            Frame baseFrame = new Frame(sBackground, sImage, sNext, sMusic, sName, sText, sNextText, sCameraDir, sPosition, null, null, sOptions, sScripts, scene, null, jo, sExtraData);

            //parse frames            
            frames.Add(DialogueScene.BaseFrameName, baseFrame);
            JObject jf = (JObject)jo["frames"];
            foreach (var x in jf)
            {
                try
                {
                    string key = x.Key;
                    JToken value = x.Value;
                    Frame f = DialogueParser.ParseSingleFrame(value, baseFrame, scene);
                    if (key.StartsWith("_"))
                        Debug.LogWarning($"Frame {key} starts with \"_\" and may collide with special names");
                    frames.Add(key, f);
                }
                catch (Exception e)
                {
                    Debug.Log($"Failed to parse frame \"{x.Key}\" in scene \"{dialogueName}\"!");
                    Debug.LogException(e);
                }
            }

            return scene;
        }

        public static Frame ParseSingleFrame(JToken jt, Frame baseFrame, DialogueScene parentScene)
        {
            string background = baseFrame.Background;
            string image = baseFrame.Image;
            string next = baseFrame.Next;
            string music = baseFrame.Music;
            string nameText = baseFrame.NameText;
            string text = baseFrame.Text;
            string nextText = baseFrame.NextText;
            string type = null;
            string cameraDir = baseFrame.CameraDirection;
            FrameScripts scripts = new FrameScripts();
            FrameOptions options = new FrameOptions();
            FrameImagePosition position = GameParams.UseDialoguePositionInheritance ? baseFrame.ImagePosition : FrameImagePosition.Center;
            IReadOnlyDictionary<string, object> extraData = null;

            if (jt["background"] != null)
                background = jt["background"].Value<string>();
            if (jt["image"] != null)
                image = jt["image"].Value<string>();
            if (jt["next"] != null)
                next = jt["next"].Value<string>();
            if (jt["music"] != null)
            {
                if (jt["music"].Type == JTokenType.Null)
                {
                    music = null;
                }
                else
                {
                    music = jt["music"].Value<string>();
                    if (music == null)
                        music = string.Empty;
                }
            }
            if (jt["nameText"] != null)
                nameText = jt["nameText"].Value<string>();
            if (jt["text"] != null)
                text = jt["text"].Value<string>();
            if (jt["nextText"] != null)
                nextText = jt["nextText"].Value<string>();
            if (jt["type"] != null)
                type = jt["type"].Value<string>();
            if (jt["cameraDir"] != null)
                cameraDir = jt["cameraDir"].Value<string>();

            if (jt["position"] != null)
            {
                if (Enum.TryParse(jt["position"].Value<string>(), true, out FrameImagePosition pos))
                    position = pos;
            }
                

            //load/parse conditionals and microscripts
            ConditionNode[] conditional = null;
            MicroscriptNode[] microscript = null;

            if (jt["conditional"] != null)
            {
                List<ConditionNode> cList = new List<ConditionNode>();
                JArray ja = (JArray)jt["conditional"];
                foreach (var x in ja)
                {
                    try
                    {
                        cList.Add(ParseConditionNode(x));
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
                conditional = cList.ToArray();
            }

            if (jt["microscript"] != null)
            {
                //TODO parse microscripts
                List<MicroscriptNode> nList = new List<MicroscriptNode>();
                JArray ja = (JArray)jt["microscript"];
                foreach (var x in ja)
                {
                    try
                    {
                        nList.Add(MicroscriptNode.Parse((JObject)x));
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e);
                    }
                }
                microscript = nList.ToArray();
            }

            //load/parse options and scripts nodes
            if (!jt["options"].IsNullOrEmpty())
            {
                options = ParseFrameOptions(jt["options"], baseFrame.Options);
            }
            else
            {
                options = new FrameOptions(baseFrame.Options); //because FrameOptions implements IReadOnlyDictionary this actually works
            }

            if (!jt["scripts"].IsNullOrEmpty())
            {
                scripts = ParseFrameScripts(jt["scripts"], baseFrame.Scripts);
            }
            else
            {
                scripts = new FrameScripts(baseFrame.Scripts); //TODO correctness?
            }

            if(!jt["ExtraData"].IsNullOrEmpty())
            {
                extraData = ParseExtraData(jt["ExtraData"], baseFrame.ExtraData);
            }
            else
            {
                extraData = baseFrame.ExtraData;
            }

            if (type == "choice")
            {
                //parse choices if choice frame
                List<ChoiceNode> choices = new List<ChoiceNode>();
                JArray ja = (JArray)jt["choices"];
                foreach (var x in ja)
                {
                    choices.Add(ParseChoiceNode(x));
                }
                return new ChoiceFrame(background, image, next, music, nameText, text, nextText, cameraDir, position, choices.ToArray(), conditional, microscript, options, scripts, parentScene, baseFrame, jt, extraData);
            }
            else if (type == "image")
            {
                bool allowSkip = true, hideSkip = false, useTimer = false;
                float timeToShow = 0;

                if (!jt["allowSkip"].IsNullOrEmpty() && jt["allowSkip"].Type == JTokenType.Boolean)
                    allowSkip = jt["allowSkip"].ToObject<bool>();

                if (!jt["hideSkip"].IsNullOrEmpty() && jt["hideSkip"].Type == JTokenType.Boolean)
                    hideSkip = jt["hideSkip"].ToObject<bool>();

                if (!jt["useTimer"].IsNullOrEmpty() && jt["useTimer"].Type == JTokenType.Boolean)
                    useTimer = jt["useTimer"].ToObject<bool>();

                if (!jt["timeToShow"].IsNullOrEmpty())
                    timeToShow = jt["timeToShow"].ToObject<float>();

                return new ImageFrame(background, image, next, music, nameText, text, nextText, cameraDir, position, allowSkip, hideSkip, timeToShow, useTimer, conditional, microscript, options, scripts, parentScene, baseFrame, jt, extraData);
            }
            else if (type == "text")
            {
                bool allowSkip = true, useTimer = false;
                float timeToShow = 0;

                if (!jt["allowSkip"].IsNullOrEmpty() && jt["allowSkip"].Type == JTokenType.Boolean)
                    allowSkip = jt["allowSkip"].ToObject<bool>();

                if (!jt["useTimer"].IsNullOrEmpty() && jt["useTimer"].Type == JTokenType.Boolean)
                    useTimer = jt["useTimer"].ToObject<bool>();

                if (!jt["timeToShow"].IsNullOrEmpty())
                    timeToShow = jt["timeToShow"].ToObject<float>();

                return new TextFrame(background, image, next, music, nameText, text, nextText, cameraDir, position, allowSkip, timeToShow, useTimer, conditional, microscript, options, scripts, parentScene, baseFrame, jt, extraData);
            }
            else if (type == "blank")
            {
                return new BlankFrame(background, image, next, music, nameText, text, nextText, cameraDir, position, conditional, microscript, options, scripts, parentScene, baseFrame, jt, extraData);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static ChoiceNode ParseChoiceNode(JToken jt)
        {
            string text = jt["text"].Value<string>();
            string next = jt["next"].Value<string>();

            MicroscriptNode[] microscripts = null;
            if (jt["microscript"] != null)
            {
                //parse microscripts
                List<MicroscriptNode> nList = new List<MicroscriptNode>();
                JArray ja = (JArray)jt["microscript"];
                foreach (var x in ja)
                {
                    try
                    {
                        nList.Add(MicroscriptNode.Parse((JObject)x));
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e);
                    }
                }
                microscripts = nList.ToArray();
            }

            ConditionNode[] conditionals = null;
            if (jt["conditional"] != null)
            {
                List<ConditionNode> cList = new List<ConditionNode>();
                JArray ja = (JArray)jt["conditional"];
                foreach (var x in ja)
                {
                    try
                    {
                        cList.Add(ParseConditionNode(x));
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e);
                    }

                }
                conditionals = cList.ToArray();
            }

            Conditional showCondition = null;
            if (jt["showCondition"] != null)
            {
                showCondition = Conditional.Parse((JObject)jt["showCondition"]);
            }

            Conditional hideCondition = null;
            if (jt["hideCondition"] != null)
            {
                hideCondition = Conditional.Parse((JObject)jt["hideCondition"]);
            }

            SkillCheckNode skillCheck = null;
            if(jt["skillCheck"] != null)
            {
                skillCheck = ParseSkillCheck(jt["skillCheck"]);
            }

            string traceSpeaker = null;
            if (jt["traceSpeaker"] != null)
                traceSpeaker = jt["traceSpeaker"].ToString();

            string traceText = null;
            if (jt["traceText"] != null)
                traceText = jt["traceText"].ToString();

            bool traceIgnore = false;
            if (jt["traceIgnore"] != null)
                traceIgnore = jt.Value<bool>("traceIgnore");

            bool traceShow = false;
            if (jt["traceShow"] != null)
                traceShow = jt.Value<bool>("traceShow");

            return new ChoiceNode(next, text, showCondition, hideCondition, microscripts, conditionals, skillCheck, traceSpeaker, traceText, traceIgnore, traceShow);
        }

        public static ConditionNode ParseConditionNode(JToken jt)
        {
            //next and list of conditions
            string next = jt["next"].Value<string>();
            JArray ja = (JArray)jt["conditions"];
            List<Conditional> conditions = new List<Conditional>();
            foreach (var x in ja)
            {
                conditions.Add(Conditional.Parse((JObject)x));
            }

            return new ConditionNode(next, conditions.ToArray());
        }

        public static SkillCheckNode ParseSkillCheck(JToken jt)
        {
            //parse check type
            string checkTypeString = jt.Value<string>("checkType");
            SkillCheckType checkType = (SkillCheckType)Enum.Parse(typeof(SkillCheckType), checkTypeString, true);

            //parse next values
            string passNext = jt.Value<string>("passNext");
            string failNext = jt.Value<string>("failNext");

            //parse appendCheckText
            bool appendCheckText = false;
            if (!jt["appendCheckText"].IsNullOrEmpty())
                appendCheckText = jt["appendCheckText"].ToObject<bool>();

            //parse target type and target
            SkillCheckTarget targetType = default;
            string target = null;
            if (!jt["stat"].IsNullOrEmpty())
            {
                targetType = SkillCheckTarget.Stat;
                target = jt["stat"].ToString();
            }
            else if (!jt["skill"].IsNullOrEmpty())
            {
                targetType = SkillCheckTarget.Skill;
                target = jt["skill"].ToString();
            }
            else if (!jt["av"].IsNullOrEmpty())
            {
                targetType = SkillCheckTarget.ActorValue;
                target = jt["av"].ToString();
            }
            else if (!jt["actorvalue"].IsNullOrEmpty())
            {
                targetType = SkillCheckTarget.ActorValue;
                target = jt["actorvalue"].ToString();
            }
            else
            {
                throw new NotSupportedException("Unsupported or unrecognized skill check type");
            }

            //parse comparison type and value
            SkillCheckComparison comparisonType = default;
            string comparisonFieldName = null;

            if(!jt["greater"].IsNullOrEmpty())
            {
                comparisonType = SkillCheckComparison.Greater;
                comparisonFieldName = "greater";
            }
            else if (!jt["greaterEqual"].IsNullOrEmpty())
            {
                comparisonType = SkillCheckComparison.GreaterEqual;
                comparisonFieldName = "greaterEqual";
            }
            else if (!jt["equal"].IsNullOrEmpty())
            {
                comparisonType = SkillCheckComparison.Equal;
                comparisonFieldName = "equal";
            }
            else if (!jt["less"].IsNullOrEmpty())
            {
                comparisonType = SkillCheckComparison.Less;
                comparisonFieldName = "less";
            }
            else if (!jt["lessEqual"].IsNullOrEmpty())
            {
                comparisonType = SkillCheckComparison.LessEqual;
                comparisonFieldName = "lessEqual";
            }
            else
            {
                throw new NotSupportedException("Unacceptable or unrecognized comparison for skill check");
            }

            string valueString = jt.Value<string>(comparisonFieldName);
            IComparable value = (IComparable)TypeUtils.StringToNumericAuto(valueString); //ouch

            return new SkillCheckNode(checkType, comparisonType, targetType, target, value, passNext, failNext, appendCheckText);
        }

        public static KeyValuePair<string, string> ParseLocation(string loc)
        {
            if (!loc.Contains("."))
                return new KeyValuePair<string, string>(null, loc);

            var arr = loc.Split('.');
            return new KeyValuePair<string, string>(arr[0], arr[1]);
        }

        public static FrameOptions ParseFrameOptions(JToken jToken, FrameOptions baseOptions)
        {
            if (jToken.Type != JTokenType.Object)
                throw new InvalidOperationException("JToken must be an object!");

            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            using (var sr = jToken.CreateReader())
            {
                dictionary = JsonSerializer.Create(CoreParams.DefaultJsonSerializerSettings).Deserialize<Dictionary<string, object>>(sr);
            }

            if (baseOptions != null)
                return new FrameOptions(baseOptions, dictionary);
            return new FrameOptions(dictionary);
        }

        public static FrameScripts ParseFrameScripts(JToken jToken, FrameScripts baseScripts)
        {
            if (jToken.Type != JTokenType.Object)
                throw new InvalidOperationException("JToken must be an object!");

            //need to handle base/overrides
            FrameScripts fs = new FrameScripts(baseScripts);

            using (var sr = jToken.CreateReader())
            {
                JsonSerializer.Create(CoreParams.DefaultJsonSerializerSettings).Populate(sr, fs);
            }

            return fs;
        }

        public static IReadOnlyDictionary<string, object> ParseExtraData(JToken jToken, IReadOnlyDictionary<string, object> baseExtraData)
        {
            Dictionary<string, object> newExtraData = jToken.ToObject<Dictionary<string, object>>(JsonSerializer.CreateDefault(CoreParams.DefaultJsonSerializerSettings));

            if(baseExtraData != null && baseExtraData.Count > 0)
            {
                newExtraData.AddRangeKeepExisting(baseExtraData);
            }

            return newExtraData;
        }
    }
}