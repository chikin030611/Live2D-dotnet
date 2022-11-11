using ColorMC.Core.Objs;
using Newtonsoft.Json;

namespace ColorMC.Core.Path;

public static class InstancesPath
{
    private const string Name = "instances";
    private const string Name1 = "game.json";
    private const string Name2 = ".minecraft";

    private static Dictionary<string, GameSetting> Games = new();

    public static string BaseDir { get; private set; }

    public static void Init(string dir)
    {
        BaseDir = dir + "/" + Name;

        Directory.CreateDirectory(BaseDir);

        var list = Directory.GetDirectories(BaseDir);
        foreach (var item in list)
        {
            var data = new DirectoryInfo(item);
            var list1 = data.GetFiles();
            var list2 = list1.Where(a => a.Name == Name1).ToList();
            if (list2.Any())
            {
                var item1 = list2.First();
                var data1 = File.ReadAllText(item1.FullName);
                var game = JsonConvert.DeserializeObject<GameSetting>(data1);
                if (game != null)
                {
                    Games.Add(game.Name, game);
                }
            }
        }
    }

    public static GameSetting? GetGame(string name)
    {
        if (Games.TryGetValue(name, out var item))
        {
            return item;
        }

        return null;
    }

    public static GameSetting? CreateVersion(string name, string version, Loaders loader, LoaderInfo info)
    {
        if (Games.ContainsKey(name))
        {
            return null;
        }

        var dir = BaseDir + "/" + name;
        if (Directory.Exists(dir))
        {
            return null;
        }

        Directory.CreateDirectory(dir);
        Directory.CreateDirectory(dir + "/" + Name2);

        var game = new GameSetting()
        {
            Dir = dir,
            Name = name,
            Version = version,
            JvmArgs = "",
            Loader = loader,
            LoaderInfo = info
        };

        var file = dir + "/" + Name1;
        File.WriteAllText(file, JsonConvert.SerializeObject(game));
        Games.Add(name, game);

        return game;
    }

    public static string GetDir(GameSetting obj)
    {
        return obj.Dir + "/" + Name2;
    }
}