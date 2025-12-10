using Avalonia.Collections;
using Cromwell.Models;
using Gaia.Services;
using Inanna.Helpers;
using Turtle.Contract.Models;

namespace Cromwell.Services;

public interface ICredentialCache : ICache<TurtleGetResponse>,
    ICache<TurtlePostRequest>
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
            item.UpdateParents(
                items.Select(x => UpdateCredential(x, updatedIds)));
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
        item.Parent = credential.ParentId.HasValue
            ? GetItem(credential.ParentId.Value)
            : null;


        return item;
    }

    public void Update(TurtlePostRequest source)
    {
        foreach (var create in source.CreateCredentials)
        {
            var item = GetItem(create.Id);
            item.Name = create.Name;
            item.Login = create.Login;
            item.Key = create.Key;
            item.IsAvailableUpperLatin = create.IsAvailableUpperLatin;
            item.IsAvailableLowerLatin = create.IsAvailableLowerLatin;
            item.IsAvailableNumber = create.IsAvailableNumber;
            item.IsAvailableSpecialSymbols =
                create.IsAvailableSpecialSymbols;
            item.Length = create.Length;
            item.Regex = create.Regex;
            item.Type = create.Type;

            item.Parent = create.ParentId.HasValue
                ? GetItem(create.ParentId.Value)
                : null;

            if (item.Parent is not null)
            {
                item.Parent.Children.Add(item);
            }
            else
            {
                _roots.Add(item);
            }
        }

        foreach (var edit in source.EditCredentials)
        {
            var items = Find(edit.Ids);

            if (edit.IsEditName)
            {
                foreach (var item in items)
                {
                    item.Name = edit.Name;
                }
            }

            if (edit.IsEditLogin)
            {
                foreach (var item in items)
                {
                    item.Login = edit.Login;
                }
            }

            if (edit.IsEditKey)
            {
                foreach (var item in items)
                {
                    item.Key = edit.Key;
                }
            }

            if (edit.IsEditIsAvailableUpperLatin)
            {
                foreach (var item in items)
                {
                    item.IsAvailableUpperLatin = edit.IsAvailableUpperLatin;
                }
            }

            if (edit.IsEditIsAvailableLowerLatin)
            {
                foreach (var item in items)
                {
                    item.IsAvailableLowerLatin = edit.IsAvailableLowerLatin;
                }
            }

            if (edit.IsEditIsAvailableNumber)
            {
                foreach (var item in items)
                {
                    item.IsAvailableNumber = edit.IsAvailableNumber;
                }
            }

            if (edit.IsEditIsAvailableSpecialSymbols)
            {
                foreach (var item in items)
                {
                    item.IsAvailableSpecialSymbols =
                        edit.IsAvailableSpecialSymbols;
                }
            }

            if (edit.IsEditCustomAvailableCharacters)
            {
                foreach (var item in items)
                {
                    item.CustomAvailableCharacters =
                        edit.CustomAvailableCharacters;
                }
            }

            if (edit.IsEditLength)
            {
                foreach (var item in items)
                {
                    item.Length = edit.Length;
                }
            }

            if (edit.IsEditRegex)
            {
                foreach (var item in items)
                {
                    item.Regex = edit.Regex;
                }
            }

            if (edit.IsEditType)
            {
                foreach (var item in items)
                {
                    item.Type = edit.Type;
                }
            }

            if (edit.IsEditParentId)
            {
                foreach (var item in items)
                {
                    ChangeParent(item, edit.ParentId);
                }
            }
        }

        foreach (var changeOrder in source.ChangeOrders)
        {
            var item = Find(changeOrder.StartId);

            if (item is null)
            {
                continue;
            }
            
            var siblings = item.Parent is not null ? item.Children : _roots;
            var index = siblings.IndexOf(item);

            if (index == -1)
            {
                continue;
            }
            
            var insertItems = Find(changeOrder.InsertIds);

            foreach (var insertItem in insertItems)
            {
                siblings.Insert(index, insertItem);
            }
        }

        foreach (var deleteId in source.DeleteIds)
        {
            Items.Remove(deleteId);
            var deleteItem = Find(deleteId);

            if (deleteItem is null)
            {
                continue;
            }

            if (deleteItem.Parent is not null)
            {
                deleteItem.Parent.Children.Remove(deleteItem);
            }
            else
            {
                _roots.Remove(deleteItem);
            }
        }
    }

    private void ChangeParent(CredentialNotify item, Guid? newParentId)
    {
        if (newParentId == item.Parent?.Id)
        {
            return;
        }
        
        if (item.Parent is not null)
        {
            item.Parent.Children.Remove(item);
        }
        else
        {
            _roots.Remove(item);
        }

        item.Parent = newParentId.HasValue ? GetItem(newParentId.Value) : null;

        if (item.Parent is not null)
        {
            item.Parent.Children.Add(item);
        }
        else
        {
            _roots.Add(item);
        }
    }

    private List<CredentialNotify> Find(Guid[] ids)
    {
        var result = new List<CredentialNotify>();

        foreach (var root in _roots)
        {
            if (ids.Contains(root.Id))
            {
                result.Add(root);

                if (ids.Length == result.Count)
                {
                    return result;
                }
            }

            Find(ids, root, result);

            if (ids.Length == result.Count)
            {
                return result;
            }
        }

        return result;
    }

    private void Find(Guid[] ids, CredentialNotify credential,
        List<CredentialNotify> result)
    {
        foreach (var child in credential.Children)
        {
            if (ids.Contains(child.Id))
            {
                result.Add(child);

                if (ids.Length == result.Count)
                {
                    return;
                }
            }

            Find(ids, child, result);

            if (ids.Length == result.Count)
            {
                return;
            }
        }
    }

    private CredentialNotify? Find(Guid id)
    {
        foreach (var root in _roots)
        {
            if (root.Id == id)
            {
                return root;
            }

            var item = Find(id, root);

            if (item is not null)
            {
                return item;
            }
        }

        return null;
    }

    private CredentialNotify? Find(Guid id, CredentialNotify credential)
    {
        foreach (var child in credential.Children)
        {
            if (child.Id == id)
            {
                return child;
            }

            return Find(id, child);
        }

        return null;
    }
}