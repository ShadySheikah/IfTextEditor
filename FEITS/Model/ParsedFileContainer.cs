﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace FEITS.Model
{
    /// <summary>
    /// Container for the contents of a parsed script file.
    /// </summary>
    public class ParsedFileContainer
    {
        public string FilePathAndName;
        public string Header;
        public List<MessageBlock> MessageList;

        public ParsedFileContainer()
        {
            EmptyFileData();
        }

        /// <summary>
        /// Removes any previous message data
        /// and returns the file to an empty state.
        /// </summary>
        public void EmptyFileData()
        {
            FilePathAndName = string.Empty;
            Header = string.Empty;
            MessageList = new List<MessageBlock>();
        }

        /// <summary>
        /// Reads lines from specified file
        /// and parses the information
        /// </summary>
        /// <param name="fileName">Specified file name</param>
        public bool LoadFromFile(string fileName)
        {
            if(fileName != string.Empty)
            {
                //Store the file path
                FilePathAndName = fileName;

                //Store text into an array split by line breaks
                string[] fileLines = File.ReadAllLines(fileName);

                //Parse for header and message blocks
                if(fileLines[0].StartsWith("MESS_"))
                {
                    //Find where the header ends
                    //Should be after "Message Name: Message"
                    int headerEndIndex = 0;
                    for(int i = 0; i < fileLines.Length; i++)
                    {
                        if(fileLines[i].Contains(":"))
                        {
                            if (headerEndIndex != 0)
                            {
                                headerEndIndex = i;
                                break;
                            }
                            else
                            {
                                headerEndIndex = i;
                                continue;
                            }
                        }
                    }

                    //If we found the header, add it to Header
                    if(headerEndIndex != 0)
                    {
                        string newHeader = string.Empty;
                        for(int i = 0; i < headerEndIndex; i++)
                        {
                            newHeader += fileLines[i];
                        }

                        Header = newHeader;

                        //Create messages from the rest of the lines
                        for(int i = headerEndIndex; i < fileLines.Length; i++)
                        {
                            //Separate the prefix from the message itself
                            if(fileLines[i].Contains(":"))
                            {
                                MessageBlock newMessage = new MessageBlock();
                                int prefixIndex = fileLines[i].IndexOf(":");
                                newMessage.Prefix = fileLines[i].Substring(0, prefixIndex);

                                //Get the message by itself
                                string message = fileLines[i].Substring(prefixIndex + 2);

                                //Make sure we didn't leave any prefix stuff behind
                                if(message.StartsWith(":"))
                                {
                                    message = message.Remove(0, 2);

                                    return false;
                                }
                                else if(char.IsWhiteSpace(message[0]))
                                {
                                    message = message.Remove(0, 1);
                                }

                                //Have the message parse the rest
                                newMessage.ParseMessage(message);

                                //Add it to our list
                                MessageList.Add(newMessage);
                            }
                            else
                            {
                                MessageBox.Show("Error: Message lines don't appear to be formatted correctly. Please make sure each message is preceeded with a Message Name.");
                                return false;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error: File header doesn't appear to be formatted correctly. Please make sure the formatting is correct.");
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("Error: File contents don't appear to be formatted correctly. Please make sure the formatting is correct.");
                    return false;
                }
            }
            else
            {
                EmptyFileData();
                FilePathAndName = fileName;
                Console.WriteLine("Opened file was empty; treating as a new object and keeping the file path.");
                return true;
            }

            return true;
        }

        /// <summary>
        /// Compiles the message list and
        /// writes contents with header to file.
        /// </summary>
        /// <param name="fileName">File to save as</param>
        public bool SaveToFile(string fileName)
        {
            if(fileName != string.Empty)
            {
                //Update file path in case different
                FilePathAndName = fileName;

                //Start compiling a string to make up the new file
                string newFileText = Header + Environment.NewLine;

                foreach (MessageBlock msg in MessageList)
                {
                    string compiledMsg = msg.CompileMessage();
                    newFileText += (compiledMsg + Environment.NewLine);
                }

                File.WriteAllText(fileName, newFileText);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}