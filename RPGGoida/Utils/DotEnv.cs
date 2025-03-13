using System;
using System.IO;
using System.Windows;

public static class DotEnv
{
    public static void Load(string envFilePath)
    {
        if (File.Exists(envFilePath))
        {
            LoadEnvFromFileCustom(envFilePath);
        }
        else
        {
            MessageBox.Show("Файл .env не найден.");
        }
    }


    private static void LoadEnvFromFileCustom(string filePath)
    {
        foreach (var line in File.ReadAllLines(filePath))
        {

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;

            var parts = line.Split(new[] { '=' }, 2);

            if (parts.Length == 2)
            {

                var key = parts[0].Trim();
                var value = parts[1].Trim();

                value = value.Replace(@"\\", @"\");


                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }
}
