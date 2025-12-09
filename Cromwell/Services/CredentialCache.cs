using Avalonia.Collections;
using Cromwell.Models;
using Gaia.Services;
using Inanna.Helpers;
using Turtle.Contract.Models;

namespace Cromwell.Services;

public interface ICredentialCache : ICache<TurtleGetResponse>
{
    IEnumerable<CredentialNotify> Roots { get; }
}

public class CredentialCache : Cache<TurtleGetResponse, CredentialNotify>,
    ICredentialCache
{
    private readonly AvaloniaList<CredentialNotify> _roots = [];

    public IEnumerable<CredentialNotify> Roots
    {
        get => _roots;
    }

    public override void Update(TurtleGetResponse source)
    {
        var updatedIds = new HashSet<Guid>();

        if (source.Roots is not null)
        {
            _roots.UpdateOrder(source.Roots.OrderBy(x => x.OrderIndex)
               .Select(x => UpdateCredential(x, updatedIds)).ToArray());
        }

        foreach (var (id, items) in source.Children)
        {
            var item = GetItem(id);
            item.Children.UpdateOrder(items.OrderBy(x => x.OrderIndex)
               .Select(x => UpdateCredential(x, updatedIds)).ToArray());
        }

        foreach (var (id, items) in source.Parents)
        {
            var item = GetItem(id);
            item.Parents.UpdateOrder(items.Select(x =>
                UpdateCredential(x, updatedIds)).ToArray());
        }
    }

    private CredentialNotify UpdateCredential(Credential credential,
        HashSet<Guid> updatedIds)
    {
        var item = GetItem(credential.Id);

        if (updatedIds.Contains(credential.Id))
        {
            return item;
        }

        item.CustomAvailableCharacters = credential.CustomAvailableCharacters;
        item.Name = credential.Name;
        item.Login = credential.Login;
        item.Key = credential.Key;
        item.IsAvailableUpperLatin = credential.IsAvailableUpperLatin;
        item.IsAvailableLowerLatin = credential.IsAvailableLowerLatin;
        item.IsAvailableNumber = credential.IsAvailableNumber;
        item.IsAvailableSpecialSymbols = credential.IsAvailableSpecialSymbols;
        item.Length = credential.Length;
        item.Regex = credential.Regex;
        item.Type = credential.Type;


        return item;
    }
}