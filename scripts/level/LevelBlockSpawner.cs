using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using static Helpers.Nullable;
using static Godot.GD;
using Convert = System.Convert;

public class LevelBlockSpawner : Spatial
{
    private const int BackOffset = 5;
    private const int FrontOffset = 20;

    private readonly List<RoadBlock> BlockPool = new List<RoadBlock>();

    private Node? _blocks;
    private RoadBlock? _startBlock;
    private int BlockCount;
    private int TotalBlocks;
    private RoadBlock StartBlock => ReturnIfNotNull(_startBlock);
    private Node Blocks => ReturnIfNotNull(_blocks);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _startBlock = GetNodeOrNull<RoadBlock>("Start");
        _blocks = GetNodeOrNull<Node>("Blocks");

        BlockCount = Blocks.GetChildren().OfType<RoadBlock>().Count();

        CreateRoad();
    }

    private void CreateRoad()
    {
        StartBlock.Name = BlockName(0);
        StartBlock.Enable();
        BlockPool.Add(StartBlock);
        AddRoadBlock();
    }

    private void OnLoadZoneEntered(Node body, int roadIndex)
    {
        if (roadIndex >= BackOffset + 1) RemoveRoadBlock(roadIndex - BackOffset);

        AddRoadBlock();
    }

    private void AddRoadBlock()
    {
        while (BlockPool.Count < FrontOffset)
        {
            TotalBlocks++;
            RoadBlock newRoadBlock = (RoadBlock) GetRandomBlock().Duplicate();

            // Moves the block at the end of the last one
            newRoadBlock.Transform = newRoadBlock.Transform.Translated(Vector3.Forward * TotalBlocks);
            // Toggle the visibility on
            newRoadBlock.Show();
            // Trigger the custom Enable function which probably enable the collisions depending on the type
            newRoadBlock.Enable();
            // Connect the loading zone of the new block
            newRoadBlock.Connect("LoadZoneEntered", this, nameof(OnLoadZoneEntered),
                new Array {TotalBlocks});
            // Give an ID to the block
            newRoadBlock.Name = BlockName(TotalBlocks);

            AddChild(newRoadBlock);
            BlockPool.Add(newRoadBlock);
        }
    }

    private RoadBlock GetRandomBlock()
    {
        var chosenIndex = Convert.ToInt32(Randi() % BlockCount);
        return Blocks.GetChildren().OfType<RoadBlock>().ElementAt(chosenIndex);
    }

    private void RemoveRoadBlock(int i)
    {
        var roadBlock = BlockPool.Find(block => block.Name == BlockName(i));
        if (roadBlock != null)
        {
            roadBlock.QueueFree();
            BlockPool.RemoveAll(block => block.Name == roadBlock.Name);
        }
    }

    private static string BlockName(int index)
    {
        return "Block_" + index;
    }
}