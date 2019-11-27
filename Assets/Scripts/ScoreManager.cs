using System;
using System.Collections.Generic;
using System.IO;

public static class ScoreManager {
    // get list of records from file
    public static List<string> ReadRecords(string path) {
        if (File.Exists(path)) {
            string[] records = File.ReadAllLines(path);
            return new List<string>(records);
        }

        File.Create(path);
        return new List<string>();
    }

    // write list to file
    public static void SaveRecords(String path, List<string> records) {
        if (records.Count > 10) {
            records.RemoveRange(10, records.Count - 10);
        }
        File.WriteAllLines(path, records);
    }
}