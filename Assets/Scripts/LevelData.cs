using System;

[Serializable]
public class LevelData
{
    public string levelFilename, size;
    public int number;
    public bool isLevelClean;
}

public class UserEditorLevelData
{
    public string levelFilename, size, crossName, crossAuthor, createFileDate, changeFileDate, deviceID;
    public int number;
    public bool isLevelClean;
}