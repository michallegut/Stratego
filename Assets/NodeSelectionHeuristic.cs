using System.Collections.Generic;

public interface NodeSelectionHeuristic
{
    void sortNodes(List<int[]> nodes, int boardSize);
}