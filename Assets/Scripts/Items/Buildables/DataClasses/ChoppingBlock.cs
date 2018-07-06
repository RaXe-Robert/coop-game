using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChoppingBlock : BuildableWorldObject
{
    protected override void Start()
    {        
        Actions = new List<UnityAction>();

        if (buildable.Recoverable)
            Actions.Add(new UnityAction(Pickup));

        Actions.AddRange(InitializeActions());
    }

    protected override UnityAction[] InitializeActions()
    {
        return new UnityAction[]
        {
            new UnityAction(ChopLog)
        };
    }

    public override void Interact(GameObject invoker, Item item)
    {
        if (!InRange(invoker.transform.position))
            return;

        if (item == null)
        {
            FeedUI.Instance.AddFeedItem("You can't chop nothing", feedType: FeedItem.Type.Fail);
            return;
        }

        if (item?.GetType() == typeof(Tool))
        {
            Tool tool = item as Tool;
            if (tool.ToolType == ToolType.Hammer)
                Actions[0].Invoke();
        }
        else if (item.Id == "pickupitem_log_small")
            Actions[1].Invoke();
        else
            FeedUI.Instance.AddFeedItem($"You can't chop '{item.Name}'", item.Sprite, FeedItem.Type.Fail);
    }

    private void ChopLog()
    {
        Inventory inv = PlayerNetwork.LocalPlayer.GetComponent<Inventory>();
        for(int i = 0; i < 5; i++)
            ItemFactory.CreateWorldObject(transform.position + new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2)), "pickupitem_sticks_small", 1);
        inv.RemoveItemById("pickupitem_log_small", 1);
    }

}
