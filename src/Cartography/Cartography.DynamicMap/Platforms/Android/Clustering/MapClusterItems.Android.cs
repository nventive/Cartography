#if __ANDROID__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cartography.DynamicMap;

/// <summary>
/// Represent an array of clusterItems.
/// </summary>
public class MapClusterItems
{
    /// <summary>
    /// Initiate the array of cluster Items.
    /// </summary>
    /// <param name="mapClusterItems">.</param>
    public MapClusterItems(MapClusterItem[] mapClusterItems)
    {
        ClusterItems = mapClusterItems;
    }

    /// <summary>
    /// Array of Map Cluster Item.
    /// </summary>
    public MapClusterItem[] ClusterItems { get ; set; }

    /// <summary>
    /// Add a array of items to existing array of MapClusterItems.
    /// </summary>
    /// <param name="items">.</param>
    public void AddItems(MapClusterItem[] items)
    {
        var listItems = ClusterItems.ToList();

        foreach (var item in items)
        {
            listItems.Add(item);
        }

        ClusterItems = listItems.ToArray();
    }

    /// <summary>
    /// Add a single item to MapClusterItems.
    /// </summary>
    /// <param name="item">.</param>
    public void AddItem(MapClusterItem item)
    {
        var listItems = ClusterItems.ToList();
        
        listItems.Add(item);

        ClusterItems = listItems.ToArray();
    }

    /// <summary>
    /// Remove a array of items to existing array of MapClusterItems.
    /// </summary>
    /// <param name="items">.</param>
    public void RemoveItems(MapClusterItem[] items)
    {
        var listItems = ClusterItems.ToList();

        foreach (var item in items)
        {
            listItems.Remove(item);
        }

        ClusterItems = listItems.ToArray();
    }

    /// <summary>
    /// Remove a single item to MapClusterItems.
    /// </summary>
    /// <param name="item">.</param>
    public void RemoveItem(MapClusterItem item)
    {
        var listItems = ClusterItems.ToList();

        listItems.Remove(item);

        ClusterItems = listItems.ToArray();
    }

    /// <summary>
    /// Send one item to another clusterItems and remove it from intial clusterItems.
    /// </summary>
    /// <param name="item">.</param>
    /// <param name="otherClusterItems">.</param>
    public void TransferTo(MapClusterItem item, MapClusterItems otherClusterItems)
    {
        RemoveItem(item);
        
        otherClusterItems.AddItem(item);
    }

    /// <summary>
    /// Add and remove MapClusterItem from MapClusterItems.
    /// </summary>
    /// <param name="itemToAdd">.</param>
    /// <param name="itemToRemove">.</param>
    public void UpdateItems(MapClusterItem[] itemToAdd, MapClusterItem[] itemToRemove)
    {
        RemoveItems(itemToRemove);
        AddItems(itemToAdd);
    }
}
#endif