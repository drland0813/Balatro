using System.Collections;
using System.Collections.Generic;

public interface IListPool<T>
{
    ListPoolElement<T> GetList();
    void StoreListElement(ListPoolElement<T> element);
}

public class ListPoolElement<T>
{
    public List<T> List { get; private set; }

    public void Create(List<T> list)
    {
        List = list;
    }

    public void Clear()
    {
        List.Clear();
    }
}
