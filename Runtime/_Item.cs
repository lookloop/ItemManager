using System;

namespace Lookloop.ItemManager
{
[Serializable]
public class Item
{
    //唯一id
    public int Id;
    //类型
    public int Type;
    //等级
    public int Tier;
    //数量
    public int Count;
    //数据
    public int[] Data;

    public Item()
    {
    }

    public Item(int id, int type, int tier, int count, int[] data)
    {
        Id = id;
        Type = type;
        Tier = tier;
        Count = count;
        Data = data;
    }
}
}
