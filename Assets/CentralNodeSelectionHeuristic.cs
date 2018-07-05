using System;
using System.Collections.Generic;

public class CentralNodeSelectionHeuristic : NodeSelectionHeuristic
{
    private double boardSize;

    public void sortNodes(List<int[]> nodes, int boardSize)
    {
        this.boardSize = boardSize;
        nodes.Sort(compareNodes);
    }

    private int compareNodes(int[] node1, int[] node2)
    {
        return Convert.ToInt32((Math.Abs(node1[0] - boardSize / 2) + Math.Abs(node1[1] - boardSize / 2)) - (Math.Abs(node2[0] - boardSize / 2) + Math.Abs(node2[1] - boardSize / 2)));
    }
}