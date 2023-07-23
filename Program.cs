using System;
using System.Diagnostics;
using System.IO;
using System.Text;

class FlexBuilder {
    string GetLine(string text, int lineNo)
    {
        string[] lines = text.Replace("\r", "").Split('\n');
        return lines.Length >= lineNo ? lines[lineNo - 1] : null;
    }

    static void Main(string[] args)
    {
        Console.WriteLine("FlexBuilder");
        Console.WriteLine("Made by Syclon");
        Console.WriteLine("");

        if (args.Length == 0)
        {
            Console.WriteLine("ERROR!");
            Console.WriteLine("You did not drag and drop a .vta file!");
            Console.WriteLine("");
            Console.WriteLine("Please press enter or close this application to try again!");
            Console.ReadLine();
            return;
        }
        else if (!args[0].EndsWith(".vta"))
        {
            Console.WriteLine("ERROR!");
            Console.WriteLine("You dragged and dropped a file that isn't a .vta file! The file you're going to drag and drop MUST be a .vta file!");
            Console.WriteLine("");
            Console.WriteLine("Please press enter or close this application to try again!");
            Console.ReadLine();
            return;
        }

        // Read the VTA file
        string path = args[0];

        string[] vta = File.ReadAllLines(path);

        int firstIndex = Array.IndexOf(vta, "time 0");
        int lastIndex = Array.IndexOf(vta, "end", firstIndex);

        // Print every line of the VTA flexes, from "time 0" to the last flex
        int amountOfFlexes = 0;
        for (int i = firstIndex; i < lastIndex; i++)
        {
            Console.WriteLine("{0}.     {1}", i - firstIndex, vta[i]);
            amountOfFlexes++;
        }
        Console.WriteLine("");
        Console.WriteLine("The VTA file has {0} flexes", amountOfFlexes - 1);

        // Create a text file, and if it exists, clear it.
        string textPath = args[0].Replace(".vta", "_flexes.txt");
        if (!File.Exists(textPath))
        {
            File.Create(textPath).Close();
        }
        else
        {
            File.WriteAllText(textPath, string.Empty);
        }

        // Write the first line (flexline "something.vta" {)
        List<string> newText = new List<string>();
        newText.Add("{");
        newText.Add(string.Format("    flexfile \"{0}\" {{", Path.GetFileName(path)));

        // Write the second line (defaultflex frame 0)
        newText.Add("       defaultflex frame 0");

        // Write all the other flexes
        for (int i = firstIndex + 1; i < lastIndex; i++)
        {
            newText.Add(string.Format("       flex {0} frame {1}", vta[i].Substring(vta[i].IndexOf("# ") + 2), i - firstIndex));
        }

        // Write the closing bracket and flex controllers (and also ask the user what the flexes should be called ingame)
        newText.Add("   }");

        List<string> flex = new List<string>();
        for (int i = firstIndex + 1; i < lastIndex; i++)
        {
            Console.WriteLine("What should \"{0}\" be called ingame? (DO NOT USE SPACES!)", vta[i].Substring(vta[i].IndexOf("# ") + 2), i - firstIndex);
            string response = Console.ReadLine();
            response = response.Replace(" ", "");
            flex.Add(string.Format("\"{0}\"", response));
        }

        string frameFlexText = string.Join(", ", flex);
        newText.Add(string.Format("    flexcontroller flex {0}", frameFlexText.Replace(',', ' ')));

        for (int i = firstIndex + 1; i < lastIndex; i++)
        {
            newText.Add(string.Format("    %{0} = {1}", vta[i].Substring(vta[i].IndexOf("# ") + 2), flex[i - firstIndex - 1].Trim('\"')));
        }

        newText.Add("}");

        // Write all the lines to the text file
        File.WriteAllLines(textPath, newText);

        Console.WriteLine("Your flexes have been built! Press enter to view!");
        Console.ReadLine();
        Process.Start("notepad.exe", textPath);
    }
}