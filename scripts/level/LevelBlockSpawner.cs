using Godot;
using static Helpers.Nullable;
using static Helpers.NodeExtentions;
using static Godot.GD;
using System.Collections.Generic;
using System.Linq;

public class LevelBlockSpawner : Spatial
{
    RoadBlock? _StartBlock;
    RoadBlock StartBlock => ReturnIfNotNull(_StartBlock);

    Node? _Blocks;
    Node Blocks => ReturnIfNotNull(_Blocks);

    List<RoadBlock> BlockPool = new List<RoadBlock>();
    int CarProgression = 0;
    int BackOffset = 5;
    int FrontOffset = 20;
    int TotalBlocks = 0;
    int BlockCount = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _StartBlock = GetNodeOrNull<RoadBlock>("Start");
        _Blocks = GetNodeOrNull<Node>("Blocks");

        BlockCount = Blocks.GetChildren().OfType<RoadBlock>().Count();

        CreateRoad();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
    }

    private void CreateRoad()
    {
        StartBlock.Name = BlockName(0);
        StartBlock.Enable();
        BlockPool.Add(StartBlock);
        AddRoadBlock();
    }

    private void onLoadZoneEntered(Node body, int roadIndex)
    {
        if (roadIndex >= BackOffset + 1)
        {
            RemoveRoadBlock(roadIndex - BackOffset);
        }
        AddRoadBlock();
    }

    private void AddRoadBlock()
    {
        while (BlockPool.Count < FrontOffset)
        {
            TotalBlocks++;
            RoadBlock newRoadBlock = (RoadBlock)GetRandomBlock().Duplicate();

            // Moves the block at the end of the last one
            newRoadBlock.Transform = newRoadBlock.Transform.Translated(Vector3.Forward * TotalBlocks);
            // Toggle the visibility on
            newRoadBlock.Show();
            // Trigger the custom Enable function wich probably enable the collisions depending on the type
            newRoadBlock.Enable();
            // Connect the loading zone of the new block
            newRoadBlock.Connect("LoadZoneEntered", this, nameof(onLoadZoneEntered), new Godot.Collections.Array { TotalBlocks });
            // Give an ID to the block
            newRoadBlock.Name = BlockName(TotalBlocks);

            AddChild(newRoadBlock);
            BlockPool.Add(newRoadBlock);
        }
    }

    private RoadBlock GetRandomBlock()
    {
        var chosenIndex = System.Convert.ToInt32(Randi() % BlockCount);
        return Blocks.GetChildren().OfType<RoadBlock>().ElementAt(chosenIndex);
    }

    private void RemoveRoadBlock(int i)
    {
        RoadBlock? roadBlock = BlockPool.Find(block => block.Name == BlockName(i));
        if (roadBlock != null)
        {
            roadBlock.QueueFree();
            BlockPool.RemoveAll(block => block.Name == roadBlock.Name);
        }
    }

    private string BlockName(int index) => "Block_" + index;
}
