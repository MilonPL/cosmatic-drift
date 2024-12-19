using Content.Client.UserInterface.Controls;
using Content.Shared._CD.CryoSleep;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.XAML;

namespace Content.Client._CD.CryoSleep;

[GenerateTypedNameReferences]
public sealed partial class LostAndFoundWindow : FancyWindow
{
    private readonly Dictionary<string, LostAndFoundPlayerEntry> _entries = new();

    public event Action<string, string>? OnRetrieveItem;

    public LostAndFoundWindow()
    {
        RobustXamlLoader.Load(this);
        Title = Loc.GetString("lost-and-found-title");
    }

    public void Populate(Dictionary<string, List<LostItemData>> players)
    {
        EmptyLabel.Visible = players.Count == 0;

        var toRemove = new HashSet<string>(_entries.Keys);

        foreach (var (playerName, items) in players)
        {
            toRemove.Remove(playerName);

            if (_entries.TryGetValue(playerName, out var existing))
            {
                existing.UpdateItems(items);
                continue;
            }

            var entry = new LostAndFoundPlayerEntry(playerName, items);
            entry.OnRetrieveItem += (slot) => OnRetrieveItem?.Invoke(playerName, slot);
            _entries.Add(playerName, entry);
            PlayersContainer.AddChild(entry);
        }

        // Remove entries for players who no longer have items
        foreach (var playerName in toRemove)
        {
            if (_entries.Remove(playerName, out var entry))
            {
                PlayersContainer.RemoveChild(entry);
            }
        }
    }
}