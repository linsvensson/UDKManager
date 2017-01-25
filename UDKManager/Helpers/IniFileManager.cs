using System;
using System.Collections.Generic;
using System.IO;
using ZerO.Helpers;

namespace ZerO
{
    /// <summary>
    /// Class for easier handling the .ini files
    /// </summary>
    public static class IniFileManager
    {
        public static string Path;

        public static void SetFile(string path)
        {
            Path = path;
        }

        public static void ReplaceValue(string oldText, string newText)
        {
            // Read in a file line-by-line, and store it all in a List.
            List<string> list = new List<string>();

            using (StreamReader reader = new StreamReader(Path))
            {
                try
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        list.Add(line); // Add to list.
                    }
                }

                catch
                {
                    // ignored
                }

                finally
                {
                    reader.Close();
                }
            }

            // Find and replace the right line
            if (list.Contains(oldText))
            {
                var index = list.IndexOf(oldText);

                if (index > 0)
                {
                    list.RemoveAt(index);
                    list.Insert(index, newText);
                }
            }

            else
            {
                Globals.Logger.Error("This value does not exist in " + Path);
                return;
            }

            // Rewrite the file
            using (StreamWriter writer = new StreamWriter(Path, false))
            {
                try
                {
                    for (int i = 0; i < list.Count; i++)
                        writer.WriteLine(list[i]);
                }

                catch
                {
                    // ignored
                }

                finally
                {
                    writer.Flush();
                    writer.Close();
                }
            }
        }

        public static void RemoveValue(string text)
        {
            // Read in a file line-by-line, and store it all in a List.
            List<string> list = new List<string>();

            using (StreamReader reader = new StreamReader(Path))
            {
                try
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        list.Add(line); // Add to list.
                    }
                }

                catch
                {
                    // ignored
                }

                finally
                {
                    reader.Close();
                }
            }

            // Find and remove the right line
            if (list.Contains(text))
            {
                var index = list.IndexOf(text);

                if (index > 0)
                    list.RemoveAt(index);
            }

            else
            {
                Globals.Logger.Error("This value does not exist in " + Path);
                return;
            }

            // Rewrite the file
            using (StreamWriter writer = new StreamWriter(Path, false))
            {
                try
                {
                    for (int i = 0; i < list.Count; i++)
                        writer.WriteLine(list[i]);
                }

                catch
                {
                    // ignored
                }

                finally
                {
                    writer.Flush();
                    writer.Close();
                }
            }
        }

        public static void AddValue(string section, string text)
        {
	        // Read in a file line-by-line, and store it all in a List.
	        List<string> list = new List<string>();

                using (StreamReader reader = new StreamReader(Path))
                {
                    try
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            list.Add(line); // Add to list.
                        }
                    }

                    catch
                    {
                        // ignored
                    }

                    finally
                    {
                        reader.Close();
                    }
                }

            var end = 0;

            // Find the right line
            for(int i = 0; i < list.Count; i++)
                if (list[i].Contains(section))
                {
                    // Count forwards to find the first empty line
                    for (var index = i; index < list.Count; index++)
                        if (string.IsNullOrWhiteSpace(list[index]))
                        {
                            end = index++;
                            break;
                        }
                }

            // Insert the text on the right line
            if (end != 0)
            {
                if (!list.Contains(text))
                    list.Insert(end, text);

                else
                {
                    Globals.Logger.Error("This value already exists in " + Path);
                    return;
                }
            }

                // Rewrite the file
                using (StreamWriter writer = new StreamWriter(Path, false))
                {
                    try
                    {
                        for (int i = 0; i < list.Count; i++)
                            writer.WriteLine(list[i]);
                    }

                    catch
                    {
                        // ignored
                    }

                    finally
                    {
                        writer.Flush();
                        writer.Close();
                    }
                }
        }
    }
}
