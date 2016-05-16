﻿using System;

namespace FEITS.Model
{
    /// <summary>
    /// Contains a single line of dialogue and its commands.
    /// </summary>
    public class MessageLine
    {
        public string RawLine = string.Empty;
        public string SpokenText = string.Empty;
        public int SpeechIndex = -1;

        public ConversationTypes ConversationType = ConversationTypes.Type1;

        public void UpdateRawWithNewDialogue()
        {
            if(SpokenText != string.Empty)
            {
                SpokenText = SpokenText.Replace('’', '\'').Replace('~', '～').Replace("  ", " ");

                int lineIndex;
                if(RawLine.Contains("$Nu"))
                {
                    lineIndex = RawLine.IndexOf("$", RawLine.IndexOf("$Nu") + 2);
                }
                else
                {
                    lineIndex = RawLine.IndexOf("$", SpeechIndex);
                }

                string oldDialogue = string.Empty;
                if (lineIndex > SpeechIndex)
                    oldDialogue = RawLine.Substring(SpeechIndex, lineIndex - SpeechIndex);
                else
                    oldDialogue = RawLine.Substring(SpeechIndex);

                RawLine = RawLine.Replace(oldDialogue, SpokenText);
            }

            RawLine = RawLine.Replace(Environment.NewLine, "\\n");
        }
    }
}